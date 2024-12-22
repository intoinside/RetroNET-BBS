using System.Net.Sockets;

namespace RetroNET_BBS.Client
{
    public abstract class User
    {
        public User(TcpClient client)
        {
            this.client = client;
        }

        protected TcpClient client;
        public OnUserDisconnectCallback OnUserDisconnect;

        public delegate void OnUserDisconnectCallback();
    }
}
