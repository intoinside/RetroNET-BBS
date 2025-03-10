using Encoder;
using System.Net.Sockets;

namespace RetroNET_BBS.Client
{
    /// <summary>
    /// User connection for Telnet
    /// </summary>
    public class TelnetUser : User
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">Client connected</param>
        /// <param name="onlineUsers">Number of online users</param>
        /// <param name="callback">Callback when user disconnects</param>
        public TelnetUser(TcpClient client, int onlineUsers, OnUserDisconnectCallback callback) : base(client, callback)
        {
            encoder = new Telnet();

            HandleConnection(onlineUsers).Wait();
        }
    }
}
