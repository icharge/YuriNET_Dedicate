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
using YuriNET.CoreServer.Http;
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

        private Thread tunnelThread;
        private static UdpClient udpClient;
        private HttpController controller;

        private bool isStarted;
        private DateTime launchedOn;
        private int timeout = 60;
        private int maxClients = 60;
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
        public static void runOnDebugging() {
            AllocConsole();
            Console.Clear();

            var companyName = fvi.CompanyName;
            var productNAme = fvi.ProductName;
            var productVersion = fvi.ProductVersion;
            Console.Title = string.Format("{0} ({1}) Alpha 1", productNAme, productVersion);
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

        public DateTime getLaunchedOnDate() {
            return launchedOn;
        }

        public int getClientsCount() {
            return controller.getClientsCount();
        }

        public bool isServerStarted() {
            return isStarted;
        }

        //private string getJsonClients() {
        //    var clientsList = clients.Where(kvp => kvp.Value != null).ToList();
        //    var sb = new StringBuilder("");
        //    foreach (var kvp in clientsList) {
        //        sb.Append("\"").Append(kvp.Value.getId()).Append("\": { ")
        //          .Append("\"name\": \"").Append(kvp.Value.getName()).Append("\",")
        //          .Append("\"ip\": \"").Append(kvp.Value.getIp()).Append("\",")
        //          .Append("\"port\": ").Append(kvp.Value.getPort()).Append(",")
        //          .Append("\"game\": \"").Append(kvp.Value.getGame().ToString()).Append("\" ")
        //          .Append(" },");
        //    }
        //    if (clientsList.Count > 0)
        //        sb.Length -= 1;
        //    return sb.ToString();
        //}

        private bool isEqualEndpoint(IPEndPoint ip1, IPEndPoint ip2) {
            return (ip1.Address.ToString() + ip1.Port).Equals(ip2.Address.ToString() + ip2.Port);
        }

        public void startServer() {
            Logger.info("Attemping to start server...");

            Logger.info("Initializing Server...");

            if (!isStarted) {
                Logger.info("Reading configurations...");
                isStarted = true;
                launchedOn = DateTime.Now;

                if (udpClient != null) {
                    Logger.info("Closing Old UDP...");
                    udpClient.Close();
                }

                Logger.info("Creating new SOCKET... on port : {0}", socketPort);
                udpClient = new UdpClient(socketPort);
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                Logger.info("Creating Socket Thread...");
                tunnelThread = new Thread(ReceiveCallback);
                tunnelThread.Start();

                Logger.info("Starting controller...");
                controller = new HttpController(this);
                controller.bind();

                Logger.info("Server is started.");
            }
        }

        public void stopServer() {
            Logger.info("Attemping to stop server...");
            if (isStarted) {
                isStarted = false;
                udpClient.Close();
                udpClient = null;
                tunnelThread.Abort();
                Logger.info("Server stopped.");

                controller.stop();
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
                Client clientFrom = controller.getClient(hdrFrom);
                Client clientTo = controller.getClient(hdrTo);

                if (null != clientFrom) {
                    if (null == clientFrom.getConnection()) {
                        clientFrom.setConnection(remoteEndPoint);
                    } else {
                        // Don't allow faking client id
                        if (!isEqualEndpoint(remoteEndPoint, clientFrom.getConnection())) {
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





    }
}
