using System.Net.Sockets;
using RetroNET_BBS.Server;

namespace RetroNET_BBS.Client
{
    public interface IUserFactory
    {
        User Create(ConnectionType type, TcpClient client, int onlineUsers, User.OnUserDisconnectCallback callback);
    }

    public class UserFactory : IUserFactory
    {
        public User Create(ConnectionType type, TcpClient client, int onlineUsers, User.OnUserDisconnectCallback callback)
        {
            switch (type)
            {
                case ConnectionType.Petscii:
                    return new PetsciiUser(client, onlineUsers, callback);
                case ConnectionType.Telnet:
                    return new TelnetUser(client, onlineUsers, callback);
                default:
                    throw new NotSupportedException($"Connection type {type} not supported");
            }
        }
    }
}
