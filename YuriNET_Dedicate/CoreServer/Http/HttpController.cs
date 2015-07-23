using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using YuriNET.Utils;

namespace YuriNET.CoreServer.Http {
    class HttpController : HttpServer {

        private ConcurrentDictionary<short, Client> clients = new ConcurrentDictionary<short, Client>();
        private IProducerConsumerCollection<short> pool;
        private Server myServer;

        private Thread threadClearTimeout;

        private int timeout = 10;
        private int peekClients = 0;
        private string password = "";

        public HttpController(int port)
            : base(port) {
        }

        public HttpController(Server server)
            : base(server.SocketPort) {
            myServer = server;
            timeout = server.Timeout;

            initClients();

            Logger.info("Creating Timeout Kicker Thread...");
            threadClearTimeout = new Thread(timeoutKicker);
            threadClearTimeout.Start();
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
            threadClearTimeout.Abort();

        }

        public Client getClient(short clientId) {
            Client client;
            clients.TryGetValue(clientId, out client);
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
            Logger.debug("request: {0}", param);

            string[] pairs = param.Split('&');
            for (int i = 0; i < pairs.Length; i++) {
                string[] kv = pairs[i].Split('=');
                if (kv.Length != 2)
                    continue;

                //kv[0] = URLDecoder.decode(kv[0], "UTF-8");
                //kv[1] = URLDecoder.decode(kv[1], "UTF-8");

                if (kv[0].Equals("clients")) {
                    requestedAmount = Int32.Parse(kv[1]);
                }

                if (kv[0].Equals("password") && !pwOk && kv[1].Equals(password)) {
                    pwOk = true;
                }
            }

            if (!pwOk) {
                Logger.info("Request was unauthorized.");
                p.writeFailure();
                p.outputStream.WriteLine("<html><body><h1>Unauthorized</h1></html>");
                return;
            }

            // Context Path here .......

            // No FavIcon
            if (p.http_url.Equals("/favicon.ico")) {
                p.writeFailure();
                return;
            }

            // Test respond resource
            if (p.http_url.Equals("/Test.png")) {
                Stream fs = File.Open("../../Test.png", FileMode.Open);

                p.writeSuccess("image/png");
                fs.CopyTo(p.outputStream.BaseStream);
                p.outputStream.BaseStream.Flush();
                fs.Close();
            }


            if (requestedAmount < 2 || requestedAmount > 8) {
                // Bad Request
                Logger.info("Request had invalid requested amount (" + requestedAmount + ").");
                p.writeFailure();
                return;
            }

            p.writeSuccess();
            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("Current Time: " + DateTime.Now.ToString());
            p.outputStream.WriteLine("url : {0}", p.http_url);

            // test respond data
            if (p.http_url.StartsWith("/?clients")) {
                p.outputStream.WriteLine("Clients : " + clients.Count);
                for (int i = 0; i < pool.Count; i++ ) {
                    short ii;
                    pool.TryTake(out ii);
                    p.outputStream.Write(ii + ", ");
                    pool.TryAdd(ii);
                }
                p.outputStream.WriteLine();
            }

            p.outputStream.WriteLine("<form method=post action=/form>");
            p.outputStream.WriteLine("<input type=text name=foo value=foovalue>");
            p.outputStream.WriteLine("<input type=submit name=bar value=barvalue>");
            p.outputStream.WriteLine("</form>");
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData) {
            Logger.debug("POST request: {0}", p.http_url);
            string data = inputData.ReadToEnd();

            p.writeSuccess();
            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("<a href=/test>return</a><p>");
            p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data);
        }

        private void timeoutKicker() {
            Logger.info("Started Timeout Kicker thread...");
            while (myServer.isServerStarted()) {
                var clientscount = getClientsCount();
                if (clientscount > peekClients) {
                    peekClients = clientscount;
                }

                var timeouts = clients

                   .Where(kvp => {
                       if (kvp.Value != null) {
                           int dayplus = (DateTime.Now - kvp.Value.getTimestamp()).Seconds;
                           return dayplus > timeout;
                       }
                       return false;
                   })
                   .ToList();
                if (timeouts.Count > 0) {
                    Logger.debug("Found timed out clients...");
                    foreach (var kvp in timeouts) {
                        removeClient(kvp.Value);
                        Logger.info("Disconnect client {0} by timed out.", kvp.Value.ToString());
                    }
                    Logger.debug("(Timeout checker) Clients size: {0}", clients.Count);

                    Logger.info("(Timeout checker) Clients count: {0}", clientscount);
                    Logger.info("(Timeout checker) Peek Clients: {0}", peekClients);
                }
                Thread.Sleep(400);
            }
        }

        private void removeClient(Client c) {
            removeClient(c.getId(), c);
        }

        private void removeClient(short k, Client c) {
            //Clients.TryRemove(k, out c);
            Logger.debug("Remove Client : {0}", c.ToString());
            clients.TryUpdate(k, null, c);
            c.Dispose();
        }


    }
}
