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

        public async Task HandleConnection(int onlineUsers)
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];

            // Show welcome page
            //var output = WelcomePage.ShowWelcome(onlineUsers);
            //byte[] response = encoder.FromAscii(output);
            //await stream.WriteAsync(response, 0, response.Length);
            var output = await ShowWelcomePage(onlineUsers, stream);

            byte[] response;

            //// Not clear why this is needed but whatever
            //await stream.ReadAsync(buffer, 0, buffer.Length);
            //string input;
            //do
            //{
            //    input = await HandleConnectionFlow(stream, buffer);
            //} while (input.Length == 0);

            string acceptedNavigationOptions = string.Empty;
            char commandArrived = (char)0;
        }
    }
}
