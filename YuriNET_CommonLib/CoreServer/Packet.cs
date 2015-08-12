namespace YuriNET.Common.CoreServer {

    public class Packet {

        public enum PacketType {
            Null,               // For null
            Ping,               // Ping to server/client
            List,               // Refresh users or rooms
            Message,            // Send messages to lobby or room
            PMessage,           // Send private messages to other client
            Announce,           // Receive announce from Server
            JoinLobby,          // User join Lobby
            QuitLobby,          // User quit Lobby
            JoinChannel,        // User join Channel
            QuitChannel,        // User quit Channel
            Invite,             // User invite to join room
            JoinRoom,           // User join Room
            QuitRoom,           // User quit Room
            OptionGame,         // User [Host] changes game options
            Ready,              // User mark ready
            UpdateState,        // User change self options
            StartGame,          // User [Host] starting game
            Comment,            // User comment other user
            Report,             // User report other user
            ReqFriendList,      // User Request friend list
            AddFriend,          // User add friend
            AddFriendConfirm,   // User send confirm friend request
            RemoveFriend,       // User remove friend
        }

        private byte[] byteData;

        public Packet() {
        }

        public Packet(byte[] byteData) {
            this.byteData = byteData;
        }
    }
}