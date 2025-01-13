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

            HandleConnection(onlineUsers);
        }
    }
}
