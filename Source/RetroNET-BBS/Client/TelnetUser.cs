using Encoder;
using System.Net.Sockets;

namespace RetroNET_BBS.Client
{
    public class TelnetUser : User
    {
        public TelnetUser(TcpClient client, int onlineUsers, OnUserDisconnectCallback callback) : base(client, callback)
        {
            encoder = new Telnet();

            HandleConnection(onlineUsers).Wait();
        }
    }
}
