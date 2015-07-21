using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using YuriNET.Utils;

namespace YuriNET.CoreServer {
    [Serializable()]
    class Server {
        // Debug console
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

        private Server() {
        }

        public class Holder {
            private static Server server = new Server();
            public static Server getServer() {
                return server;
            }
        }

        private Thread thread;
        private Thread threadClearTimeout;

        private static ConcurrentDictionary<short, Client> clients = new ConcurrentDictionary<short, Client>();
        private static ConcurrentQueue<short> pool;

        private static UdpClient udpClient;
        private bool isStarted;
        private bool isRestored;
        private DateTime launchedOn;
        private int timeout = 10;
        private int maxClients = 60;
        private int peekClients = 0;
        private int socketPort = 9000;
        private string serverName = "YuriNET Dedicated";

        public int Timeout {
            get {
                return timeout;
            }
            set {
                if (!isStarted) {
                    timeout = value;
                }
            }
        }
        public int MaxClients {
            get {
                return maxClients;
            }
            set {
                if (!isStarted) {
                    maxClients = value;
                }
            }
        }
        public int SocketPort {
            get {
                return socketPort;
            }
            set {
                if (!isStarted) {
                    socketPort = value;
                }
            }
        }

        //[Conditional("DEBUG")]
        private void runOnDebugging() {
            AllocConsole();
            Console.Clear();

            var companyName = fvi.CompanyName;
            var productNAme = fvi.ProductName;
            var productVersion = fvi.ProductVersion;
            Console.Title = string.Format("{0} ({1}) Alpha 1", productNAme, productVersion);
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

            Logger.info("Took {0} secs to initialize pool.", DateTimeUtil.TimeDiffBySec(DateTime.Now, launchedOn));
        }

        public void setServerName(string v) {
            serverName = v;
        }

        public string getServerName() {
            return serverName;
        }

        public string getServerVersion() {
            return fvi.ProductVersion;
        }

        public string getLaunchedOn() {
            return launchedOn.ToShortDateString() + " " + launchedOn.ToShortTimeString();
        }

        public int getPeekClients() {
            return peekClients; //TODO: Not implement yet
        }

        public int getClientsCount() {
            return clients.Where(kvp => kvp.Value != null).ToList().Count;
        }

        private string getJsonClients() {
            var clientsList = clients.Where(kvp => kvp.Value != null).ToList();
            var sb = new StringBuilder("");
            foreach (var kvp in clientsList) {
                sb.Append("\"").Append(kvp.Value.getId()).Append("\": { ")
                  .Append("\"name\": \"").Append(kvp.Value.getName()).Append("\",")
                  .Append("\"ip\": \"").Append(kvp.Value.getIp()).Append("\",")
                  .Append("\"port\": ").Append(kvp.Value.getPort()).Append(",")
                  .Append("\"game\": \"").Append(kvp.Value.getGame().ToString()).Append("\" ")
                  .Append(" },");
            }
            if (clientsList.Count > 0)
                sb.Length -= 1;
            return sb.ToString();
        }

        private Client getClient(short clientId) {
            Client client;
            clients.TryGetValue(clientId, out client);
            return client;
        }

        private bool isEqualEndpoint(IPEndPoint ip1, IPEndPoint ip2) {
            return (ip1.Address.ToString() + ip1.Port).Equals(ip2.Address.ToString() + ip2.Port);
        }

        public void startServer() {
            runOnDebugging();
            Logger.info("Attemping to start server...");

            Logger.info("Initializing Server...");
            //Clients = new ConcurrentDictionary<int, Client>();
            //initClients();
            if (!isStarted) {
                Logger.info("Reading configurations...");
                isStarted = true;
                launchedOn = DateTime.Now;
                // Initialize
                if (!isRestored) {
                    initClients();
                    peekClients = 0;
                } else {
                    Logger.info(" ** No need to reinitialize after restoring state **");
                }

                if (udpClient != null) {
                    Logger.info("Closing Old UDP...");
                    udpClient.Close();
                }

                Logger.info("Creating new SOCKET... on port : {0}", socketPort);
                udpClient = new UdpClient(socketPort);
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                
                Logger.info("Creating Socket Thread...");
                thread = new Thread(ReceiveCallback);
                thread.Start();

                Logger.info("Creating Timeout Kicker Thread...");
                threadClearTimeout = new Thread(timeoutKicker);
                threadClearTimeout.Priority = ThreadPriority.Lowest;
                threadClearTimeout.Start();

                //Logger.info("Starting InfoServer...");
                //infoServer = new InfoServer(this);
                //infoServer.start();

                Logger.info("Server is started.");
            } else {

            }
        }

        public void stopServer() {
            Logger.info("Attemping to stop server...");
            if (isStarted) {
                isStarted = false;
                udpClient.Close();
                udpClient = null;
                thread.Abort();
                threadClearTimeout.Abort();

                Logger.info("Server stopped.");
            } else {

            }
        }

        private void ReceiveCallback() {
            Logger.info("ReceiveCallback() is on the way...");
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (isStarted) {
                byte[] buf;

                try {
                    buf = udpClient.Receive(ref remoteEndPoint);
                } catch (SocketException ex) {
                    Logger.warn(ex.ToString());
                    continue;
                }
                Logger.debug("=========== NEW Received ===========");
                // Logger.debug("Received data from " + ipPort);

                // Get header from packet
                short hdrFrom = BytesUtil.ToShort(buf[0], buf[1]);
                short hdrTo = BytesUtil.ToShort(buf[2], buf[3]);

                // Check some condition
                if (buf.Length < 2) {
                    continue;
                }

                // Get info in Client class type
                Client clientFrom = getClient(hdrFrom);
                Client clientTo = getClient(hdrTo);

                if (null != clientFrom) {
                    if (null == clientFrom.getConnection()) {
                        clientFrom.setConnection(remoteEndPoint);
                    } else {
                        // Don't allow faking client id
                        if (! isEqualEndpoint(remoteEndPoint, clientFrom.getConnection())) {
                            clientFrom = null;
                        }
                    }
                }

                if (null == clientFrom || null == clientTo || hdrFrom == hdrTo || !clientTo.isKnownClient(clientFrom.getId())) {
                    Logger.debug("Ignoring packet from " + hdrFrom + " to " + hdrTo + " (" + remoteEndPoint.Address.ToString() + ":" + remoteEndPoint.Port + "), was " + buf.Length + " bytes");
                } else {
                    clientFrom.setTimestamp();

                    if (clientTo.getConnection() != null) {
                        udpClient.Send(buf, buf.Length, clientTo.getConnection());
                    }
                }

            }
        }

        private void timeoutKicker() {
            if (isRestored) {
                Logger.info("[Timeout Kicker] : Waiting after restored state...");
                Thread.Sleep(20 * 1000);
            }
            Logger.info("Started Timeout Kicker thread...");
            while (isStarted) {
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
