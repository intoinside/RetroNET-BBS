using Encoder;
using RetroNET_BBS.ContentProvider;
using System.Net.Sockets;

namespace RetroNET_BBS.Client
{
    public class PetsciiUser : User
    {
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

            HandleConnection(onlineUsers);
        }
    }
}
