using Encoder;
using RetroNET_BBS.ContentProvider;
using System.Net.Sockets;

namespace RetroNET_BBS.Client
{
    /// <summary>
    /// User connection for Petscii
    /// </summary>
    public class PetsciiUser : User
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">Client connected</param>
        /// <param name="onlineUsers">Number of online users</param>
        /// <param name="callback">Callback when user disconnects</param>
        public PetsciiUser(TcpClient client, int onlineUsers, OnUserDisconnectCallback callback) : base(client, callback)
        {
            encoder = new Petscii(PageContainer.Imports);

            // Not clear why, Petscii requires a stream.Read in order to clear
            // buffer before using it
            NetworkStream stream = client.GetStream();
            {
                var buffer = new byte[1024];
                stream.Read(buffer, 0, buffer.Length);
            }

            HandleConnection(onlineUsers).Wait();
        }
    }
}
