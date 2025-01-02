using Encoder;
using System.Net.Sockets;

namespace RetroNET_BBS.Client
{
    public class TelnetUser : User
    {
        public TelnetUser(TcpClient client, int onlineUsers) : base(client)
        {
            encoder = new Telnet();

            HandleConnection(onlineUsers);
        }
    }
}
