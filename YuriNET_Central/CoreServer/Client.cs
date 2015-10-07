using System;
using System.Net.Sockets;

namespace YuriNET.Central.CoreServer {

    [Serializable()]
    internal class Client : IDisposable {

        #region "Disposing"

        // Has Dispose() already been called?
        private Boolean isDisposed = false;

        // Implement IDisposable.
        public void Dispose() {
            Console.WriteLine("Client #" + id + " Disposing");
            ReleaseResources(true); // cleans both unmanaged and managed resources
            GC.SuppressFinalize(this); // supress finalization
        }

        protected void ReleaseResources(bool isFromDispose) {
            // Try to release resources only if they have not been previously released.
            if (!isDisposed) {
                if (isFromDispose) {
                    // TODO: Release managed resources here
                    // GC will automatically release Managed resources by calling the destructor,
                    // but Dispose() need to release managed resources manually
                }
                //TODO: Release unmanaged resources here
            }
            isDisposed = true; // Dispose() can be called numerous times
        }

        // Use C# destructor syntax for finalization code, invoked by GC only.
        ~Client() {
            // cleans only unmanaged stuffs
            ReleaseResources(false);
        }

        #endregion "Disposing"

        private short id;
        private int channelId;
        private int roomId;
        private String userId;
        private String name;
        private ClientStatus clientStatus;
        private DateTime timestamp;
        private DateTime joined;

        private Socket socket;
        //private IList<short> friends; // maybe room client list?

        public Client(Socket socket, String name, String userId = null) {
            this.socket = socket;
            this.name = name;
            this.userId = userId;
        }

        public short getId() {
            return id;
        }

        public int getChannelId() {
            return channelId;
        }

        public Client setChannelId(int channelId) {
            this.channelId = channelId;
            return this;
        }

        public int getRoomId() {
            return roomId;
        }

        public Client setRoomId(int roomId) {
            this.roomId = roomId;
            return this;
        }

        public String getUserId() {
            return userId;
        }

        public String getName() {
            return name;
        }

        public Client setName(string name) {
            this.name = name;
            return this;
        }

        public ClientStatus getClientStatus() {
            return clientStatus;
        }

        public Client setClientStatud(ClientStatus cs) {
            this.clientStatus = cs;
            return this;
        }

        public void setTimestamp() {
            timestamp = DateTime.Now;
        }

        public DateTime getTimestamp() {
            return timestamp;
        }

        public void setJoined(DateTime joined) {
            this.joined = joined;
        }

        public DateTime getJoined() {
            return joined;
        }
    }

    internal enum ClientStatus {
        Idle,
        Lobby,
        Channel,
        Room,
        Playing,
    }
}