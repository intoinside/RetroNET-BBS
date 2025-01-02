using Encoder;
using System.Net.Sockets;

namespace RetroNET_BBS.Client
{
    public class PetsciiUser : User
    {
        public PetsciiUser(TcpClient client, int onlineUsers) : base(client)
        {
            encoder = new Petscii();

            HandleConnection(onlineUsers);
        }
    }
}
