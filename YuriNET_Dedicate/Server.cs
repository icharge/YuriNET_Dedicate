using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using YuriNET.Utils;

namespace YuriNET.Server {
    [Serializable()]
    class Server : ISerializable {
        // Debug console
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

        private Server() { }

        // Deserialization Constructor
        public Server(SerializationInfo info, StreamingContext ctxt) {
            Clients = (ConcurrentDictionary<int, Client>)info.GetValue("clients",
                typeof(ConcurrentDictionary<int, Client>));
            timeout = info.GetInt16("timeout");
            maxClients = info.GetInt16("maxclients");
            peekClients = info.GetInt16("peekclients");
            socketPort = info.GetInt16("socketport");
            serverName = info.GetString("servername");

            isRestored = true;
        }

        // Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
            //You can use any custom name for your name-value pair. But make sure you
            // read the values with the same name. For ex:- If you write EmpId as "EmployeeId"
            // then you should read the same with "EmployeeId"
            info.AddValue("clients", Clients);
            info.AddValue("timeout", timeout);
            info.AddValue("maxclients", maxClients);
            info.AddValue("peekclients", peekClients);
            info.AddValue("socketport", socketPort);
            info.AddValue("servername", serverName);

        }

        private static ConcurrentDictionary<int, Client> Clients = new ConcurrentDictionary<int, Client>();
        private static UdpClient udpClient;

        public class Holder {
            private static Server server = new Server();
            public static Server getServer() {
                return server;
            }
        }

        //private Socket udpClient;
        private Thread thread;
        private Thread threadClearTimeout;

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

        private enum command_cmd {
            CMD_CONTROL = 0xFE,
            CMD_BROADCAST = 0xFF,
        }

        private enum command_ctl {
            CTL_PING = 0x0,
            CTL_QUERY = 0x1,
            CTL_RESET = 0x2,
            CTL_DISCONNECT = 0x3,
            CTL_PROXY = 0x4,
            CTL_PROXY_DISCONNECT = 0x5,
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
            Clients.Clear();
            for (int i = 0; i < maxClients; i++) {
                Clients.TryAdd(i, null);
            }
            Logger.info("Allocated size : {0}", Clients.Count);
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
            return peekClients; // Not implement yet
        }

        public int getClientsCount() {
            return Clients.Where(kvp => kvp.Value != null).ToList().Count;
        }

        private string getJsonClients() {
            var clientsList = Clients.Where(kvp => kvp.Value != null).ToList();
            var sb = new StringBuilder("");
            foreach (var kvp in clientsList) {
                sb.Append("\"").Append(kvp.Value.getId()).Append("\": { ")
                  .Append("\"name\": \"").Append(kvp.Value.getName()).Append("\",")
                  .Append("\"ip\": \"").Append(kvp.Value.getIp()).Append("\",")
                  .Append("\"port\": ").Append(kvp.Value.getPort()).Append(",")
                  .Append("\"game\": \"").Append(kvp.Value.getGame().ToString()).Append("\" ")
                  .Append(" },");
            }
            if (clientsList.Count > 0) sb.Length -= 1;
            return sb.ToString();
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
                }
                else {
                    Logger.info(" ** No need to reinitialize after restoring state **");
                }

                if (udpClient != null) {
                    Logger.info("Closing Old UDP...");
                    udpClient.Close();
                }

                Logger.info("Creating new SOCKET... on port : {0}", socketPort);
                udpClient = new UdpClient(socketPort);
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                // udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpClient);
                //udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //udpClient.Bind(new IPEndPoint(IPAddress.Any, socketPort));

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
            }
            else {

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
            }
            else {

            }
        }

        private void ReceiveCallback() {
            Logger.info("ReceiveCallback() is on the way...");
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            //EndPoint remote = (EndPoint)remoteEndPoint;
            while (isStarted) {
                byte[] receivedBytes; // = new byte[1024];

                try {
                    //udpClient.ReceiveFrom(receivedBytes, ref remote);
                    receivedBytes = udpClient.Receive(ref remoteEndPoint);
                }
                catch (SocketException ex) {
                    Logger.warn(ex.ToString());
                    continue;
                }
                var ipPort = remoteEndPoint.Address.ToString() + ':' + remoteEndPoint.Port;
                Logger.debug("=========== NEW Received ===========");
                Logger.debug("Received data from " + ipPort);
                Client client;
                int cmdCommand;
                int ctlCommand;

                //Stopwatch sw1 = Stopwatch.StartNew();

                // Check some condition
                if (receivedBytes.Length > 1) {
                    cmdCommand = receivedBytes[0];
                    ctlCommand = receivedBytes[1];
                }
                else {
                    // New loop
                    continue;
                }
                var dataSend = receivedBytes;

                Logger.debug("Packet size: {0}", receivedBytes.Length);

                if (cmdCommand == (int)command_cmd.CMD_CONTROL) {
                    // Modes
                    Logger.info("{0} Request CMD {1} / CTL {2}", ipPort, cmdCommand, ctlCommand);
                    switch (ctlCommand) {
                        case (int)command_ctl.CTL_PING:
                            // Ping Pong
                            udpClient.Send(dataSend, dataSend.Length, remoteEndPoint);
                            //udpClient.SendTo(dataSend, dataSend.Length, SocketFlags.None, remote);

                            Logger.info("Sent PONG to {0}", ipPort);
                            continue;

                        case (int)command_ctl.CTL_QUERY:
                            Logger.info("Request Query");
                            // Query clients info
                            StringBuilder query = new StringBuilder("");
                            query.Append("  { \"serverstate\": \"online\",")
                                  .Append("\"servername\": \"").Append(getServerName()).Append("\",")
                                  .Append("\"serverver\": \"").Append(getServerVersion()).Append("\",")
                                  .Append("\"serverport\": ").Append(SocketPort).Append(",")
                                  .Append("\"launchedon\": \"").Append(getLaunchedOn()).Append("\",")
                                  .Append("\"maxclients\": ").Append(MaxClients).Append(",")
                                  .Append("\"peekclients\": ").Append(getPeekClients()).Append(",")
                                  .Append("\"clientcount\": ").Append(getClientsCount()).Append(",")
                                  .Append("\"clients\": { ")
                                  .Append(getJsonClients())
                                  .Append(" }")
                                  .Append(" }");
                            var queryb = Encoding.ASCII.GetBytes(query.ToString()); //Program.GetBytes(query.ToString());
                            udpClient.Send(queryb, queryb.Length, remoteEndPoint);
                            break;

                        case (int)command_ctl.CTL_RESET:
                            // Reset server ???
                            // Not implement yet
                            break;

                        case (int)command_ctl.CTL_DISCONNECT:
                            // Disconnecting
                            Logger.info("CTL_Disconnect");
                            /*
                             * How?
                             * 1. Give ID to this IP:PORT that connected
                             * 2. removeClient by ID
                             */

                            //removeClient(client);

                            continue;

                        case (int)command_ctl.CTL_PROXY:
                            // Proxy ?
                            // Not implement yet
                            break;

                        case (int)command_ctl.CTL_PROXY_DISCONNECT:
                            // Proxy Disconnect ?
                            // Not implement yet
                            break;
                    }
                    continue;
                }

                // Check max users
                if (getClientsCount() >= maxClients) {
                    // Send packet to tell client
                    // Number clients reach max
                    Logger.info("Server reach Max of clients. !!");

                    continue;
                }

                // Add user to Clients Pool
                // Query exist client from IP : PORT
                Logger.debug("Giving ID for client");
                var queryClientId = Clients
                    .Where(kvp => {
                        if (kvp.Value != null) {
                            return kvp.Value.getIpPort() == ipPort;
                        }
                        return false;
                    })
                    .Select(kvp => kvp.Value)
                    .FirstOrDefault();

                if (queryClientId != null) //
                {
                    // Found client
                    client = queryClientId;
                    client.setTimestamp();

                    Logger.debug("Found {0}", client.ToString());
                }
                else {
                    // Not found, Assign this one on empty slot

                    var newId = Clients
                        .Where(kvp => kvp.Value == null)
                        .Select(kvp => kvp.Key)
                        .FirstOrDefault();
                    client = new Client();
                    client.setConnection(remoteEndPoint);
                    client.setId(newId);
                    client.setTimestamp();
                    //client.setGame();
                    Clients.TryUpdate(newId, client, null);

                    Logger.debug("New {0} accepted", client.ToString());
                    continue;
                }




                if (cmdCommand == (int)command_cmd.CMD_BROADCAST) {
                    // Broadcast to all clients
                    Logger.debug("BROADCAST by {0}", client.getId());
                    dataSend[0] = (byte)client.getId();

                    var clientsToBroadcast = Clients.Where(kvp => {
                        if (kvp.Value != null) {
                            if (kvp.Value.getId() != client.getId()) {
                                return true;
                            }
                        }
                        return false;
                    });
                    foreach (var kvp in clientsToBroadcast) {
                        Logger.debug("Broadcast to : {0} => {1}", client.ToString(), kvp.Value.ToString());
                        udpClient.Send(dataSend, dataSend.Length, kvp.Value.getIp(), kvp.Value.getPort());
                        //udpClient.SendTo(dataSend, kvp.Value.getConnection());
                    }

                    // Set name & game to current client
                    if (client.getName() == "" || client.getName() == null) {
                        try {
                            string clientName = Encoding.ASCII
                                .GetString(ArrayUtils.SubArray(receivedBytes, 25, 16)).TrimEnd('\0');
                            client.setName(clientName);
                        }
                        catch (Exception ex) {
                            Logger.debug("Can't get name from bytes !");
                            Logger.debug("{0}", ex.ToString());
                        }
                    }

                    if (client.getGame() == Client.Game.Non) {
                        Client.Game clientGame;

                        if (receivedBytes[19] == 3) {
                            clientGame = Client.Game.RA2;
                        }
                        else if (receivedBytes[19] == 4) {
                            clientGame = Client.Game.YR;
                        }
                        else {
                            clientGame = Client.Game.Non;
                        }

                        client.setGame(clientGame);
                    }


                }
                else if (cmdCommand != client.getId()) //cmdCommand != client.getId()
                {
                    // Send to specify ID
                    var idSend = cmdCommand;
                    dataSend[0] = (byte)client.getId();
                    Logger.debug("Send to specify ID #{0} => #{1}", client.getId(), idSend);

                    Client c;
                    if (Clients.TryGetValue(idSend, out c)) {
                        if (c != null) {
                            Logger.debug("Send to : {0} => {1}", client.ToString(), c.ToString());
                            udpClient.Send(dataSend, dataSend.Length, c.getIp(), c.getPort());
                            //udpClient.SendTo(dataSend, c.getConnection());
                        }
                        else {
                            Logger.error("Error can't send to {0}", idSend);
                        }

                    }
                    else {
                        Logger.error("Error can't get to send to {0}", idSend);
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

                var timeouts = Clients

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
                    Logger.debug("(Timeout checker) Clients size: {0}", Clients.Count);

                    Logger.info("(Timeout checker) Clients count: {0}", clientscount);
                    Logger.info("(Timeout checker) Peek Clients: {0}", peekClients);
                }
                Thread.Sleep(400);
            }
        }

        private void removeClient(Client c) {
            removeClient(c.getId(), c);
        }

        private void removeClient(int k, Client c) {
            //Clients.TryRemove(k, out c);
            Logger.debug("Remove Client : {0}", c.ToString());
            Clients.TryUpdate(k, null, c);
            c.Dispose();
        }


    }
}
