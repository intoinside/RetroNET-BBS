using Encoder;
using RetroNET_BBS.ContentProvider;
using System.Net.Sockets;

namespace RetroNET_BBS.Client
{
    public class PetsciiUser : User
    {
        public PetsciiUser(TcpClient client, int onlineUsers) : base(client)
        {
            encoder = new Petscii(PageContainer.Imports);

            HandleConnection(onlineUsers);
        }
    }
}
