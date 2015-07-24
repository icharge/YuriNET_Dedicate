using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Serialization;

namespace YuriNET.CoreServer {
    [Serializable()]
    class Client : IDisposable {
        // Has Dispose() already been called?
        Boolean isDisposed = false;
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

        private short id;
        private IPEndPoint connection;
        private String name;
        private DateTime timestamp;
        private IList<short> friends;
        public enum Game {
            Non,
            RA2,
            YR
        }
        private Game game;

        public Client(short id, IList<short> friends) {
            this.id = id;
            this.setName("Unknown");
            this.setTimestamp();
            this.friends = friends;
        }

        public override string ToString() {
            return string.Format("#{0} {1} ({2})", getId(), getIpPort(), getName());
        }

        public bool isKnownClient(short otherId) {
            return friends.Contains(otherId);
        }

        public short getId() {
            return id;
        }

        public IPEndPoint getConnection() {
            return connection;
        }

        public void setConnection(IPEndPoint v) {
            connection = v;
        }

        public String getIp() {
            return connection.Address.ToString();
        }

        public int getPort() {
            return connection.Port;
        }

        public String getIpPort() {
            return null != getConnection() ? connection.Address.ToString() + ':' + connection.Port.ToString() : "Not connected";
        }

        public String getName() {
            return name;
        }

        public void setName(String v) {
            name = v;
        }

        public void setTimestamp() {
            timestamp = DateTime.Now;
        }

        public DateTime getTimestamp() {
            return timestamp;
        }

        public void setGame(Game v) {
            game = v;
        }

        public Game getGame() {
            return game;
        }
    }
}
