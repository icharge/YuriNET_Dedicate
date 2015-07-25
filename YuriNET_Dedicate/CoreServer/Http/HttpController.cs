using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using YuriNET.Utils;

namespace YuriNET.CoreServer.Http {
    class HttpController : HttpServer {

        private ConcurrentDictionary<short, Client> clients = new ConcurrentDictionary<short, Client>();
        private IProducerConsumerCollection<short> pool;
        private Server myServer;

        private Thread runnerThread;

        private bool maintenace = false;
        private int timeout = 60;
        private int maxClients = 90;
        private int peekClients = 0;
        private string password = null;

        public HttpController(int port)
            : base(port) {
        }

        public HttpController(Server server)
            : base(server.SocketPort) {
            myServer = server;
            timeout = server.Timeout;
            maxClients = server.MaxClients;
            Logger.info("Timeout : {0} secs", timeout);
            Logger.info("Max Client : {0}", maxClients);

            initClients();


            Logger.info("Creating Timeout Kicker Thread...");
            runnerThread = new Thread(runner);
            runnerThread.Start();
        }

        private void initClients() {
            Logger.info("Allocating Client Pool...");
            // Clear
            clients.Clear();

            IList<short> allShort = new List<short>();
            for (short i = short.MinValue; i < short.MaxValue; i++) {
                allShort.Add(i);
            }
            allShort.Shuffle();
            pool = new ConcurrentQueue<short>(allShort);

            Logger.info("Took {0} secs to initialize pool.", DateTimeUtil.TimeDiffBySec(DateTime.Now, myServer.getLaunchedOnDate()));
        }

        public override void stop() {
            base.stop();
            runnerThread.Abort();

        }

        public Client getClient(short clientId) {
            Client client;
            Logger.debug("getClient({0})", clientId);
            clients.TryGetValue(clientId, out client);
            Logger.debug("client : {0}", client);
            return client;
        }

        public int getClientsCount() {
            return clients.Where(kvp => kvp.Value != null).ToList().Count;
        }

        public int getPeekClients() {
            return peekClients;
        }

        public override void handleGETRequest(HttpProcessor p) {
            // Preparing
            int requestedAmount = 0;
            bool pwOk = (password == null);
            string param = p.http_url;
            if (param.StartsWith("/")) {
                param = param.Substring(1);
            }
            Logger.debug("request: {0}", p.http_url);
            String[] parameters = param.Split('/');
            foreach (String parameter in parameters) {
                // Input URL Queries
                if ("request".Equals(parameter)) {
                    if (p.http_url.Contains("clients")) {
                        if (!Int32.TryParse(p.http_query["clients"], out requestedAmount)) {
                            Logger.debug("Invalid number");
                            p.write400();
                            p.outputStream.WriteLine("Invalid request");
                            return;
                        }
                    }
                }
            }

            if (!pwOk) {
                Logger.debug("Request was unauthorized.");
                p.write401();
                p.outputStream.WriteLine("401 Unauthorized");
                return;
            }

            if (requestedAmount < 2 || requestedAmount > 8) {
                // Bad Request
                Logger.debug("Request had invalid requested amount (" + requestedAmount + ").");
                p.write500();
                p.outputStream.WriteLine("500 Request had invalid requested amount");
                return;
            }

            if (maintenace) {
                Logger.debug("Server Maintenace !");
                p.write503();
                p.outputStream.WriteLine("503 Server Temporarily Unavailable");
                return;
            }

            // TODO: Locking request

            // Preparing
            StringBuilder ret = new StringBuilder();

            if (requestedAmount + clients.Count > maxClients) {
                // Reach max clients
                Logger.info("Server reached max clients : {0}", maxClients);
                p.write503();
                p.outputStream.WriteLine("503 Server full");
                return;
            }

            ret.Append("[");

            // for thread safety, we just try to reserve slots (actually we are
            // double synchronized right now, makes little sense)
            IList<short> reserved = new List<short>();
            for (int i = 0; i < requestedAmount; i++) {
                short clientId;
                if (pool.TryTake(out clientId)) {
                    reserved.Add(clientId);
                }
            }

            if (reserved.Count == requestedAmount) {
                foreach (short clientId in reserved) {
                    clients.TryAdd(clientId, new Client(clientId, reserved));
                    Logger.debug("Client {0} allocated.", clientId);
                    ret.Append(clientId);
                    ret.Append(",");
                }
            } else {
                // return our reservations if any
                foreach (short clientId in reserved) {
                    pool.TryAdd(clientId);
                }
                Logger.debug("Request wanted more than we could provide and we also exhausted our queue.");
                p.write500();
                p.outputStream.WriteLine("500 Reserved and Request not equal");
                return;
            }

            // TODO: IP Limitation

            ret.Length = ret.Length - 1; // remove 1 comma
            ret.Append("]");

            // Send clients ID
            p.writeSuccess();
            p.outputStream.WriteLine(ret.ToString());

        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData) {
            Logger.debug("POST request: {0}", p.http_url);
            string data = inputData.ReadToEnd();

            p.writeSuccess();
            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("<a href=/test>return</a><p>");
            p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data);
        }

        private void runner() {
            Logger.info("Controller started...");

            //long lastHeartbeat = 0;
            //bool connected = false;

            while (myServer.isServerStarted()) {
                // Shutdown on maintenace mode
                if (maintenace && clients.IsEmpty) {
                    Logger.info("Tunnel empty, doing maintenace quit.");
                    myServer.stopServer();
                    return;
                }

                // TODO: Send heartbeat to master server.

                // Removing timeout clients
                var timeouts = clients
                   .Where(kvp => {
                        return DateTimeUtil.checkTimeout(kvp.Value.getTimestamp(), (long)timeout * 1000);
                   }).ToList();
                if (timeouts.Count > 0) {
                    foreach (var kvp in timeouts) {
                        Logger.info("Disconnect client {0} timed out.", kvp.Value.ToString());
                        Client client;
                        clients.TryRemove(kvp.Key, out client);
                        pool.TryAdd(kvp.Key);
                        client.Dispose();
                    }
                }

                // TODO: Locks Unlocking clients

                // Set peek
                var clientscount = getClientsCount();
                if (clientscount > peekClients) {
                    peekClients = clientscount;
                }

                Logger.info("Clients : {0} / {1} players online.", clients.Count, maxClients);
                Thread.Sleep(5000);
            }
        }
    }
}
