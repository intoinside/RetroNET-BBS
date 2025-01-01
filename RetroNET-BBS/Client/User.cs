using Common.Dto;
using Encoder;
using RetroNET_BBS.Templates;
using System.Net.Sockets;
using System.Text;

namespace RetroNET_BBS.Client
{
    public abstract class User
    {
        protected bool connectionDone = false;
        protected Stack<Page> history = new Stack<Page>();

        public User(TcpClient client)
        {
            this.client = client;
        }

        public async Task<string> ShowWelcomePage(int onlineUsers, NetworkStream stream)
        {
            var output = WelcomePage.ShowWelcome(onlineUsers);
            byte[] response = encoder.FromAscii(output);
            await stream.WriteAsync(response, 0, response.Length);

            byte[] buffer = new byte[1024];

            // Not clear why this is needed but whatever
            await stream.ReadAsync(buffer, 0, buffer.Length);
            string input;
            do
            {
                input = await HandleConnectionFlow(stream, buffer);
            } while (input.Length == 0);

            return output;
        }

        protected async Task<string> HandleConnectionFlow(NetworkStream stream, byte[] buffer)
        {
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

            if (bytesRead == 0)
            {
                Disconnect();

                connectionDone = true;
            }

            string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            return data;
        }

        protected void Disconnect()
        {
            OnUserDisconnect();
            connectionDone = true;
            client.Close();
        }


        protected TcpClient client;
        protected IEncoder encoder;

        public OnUserDisconnectCallback OnUserDisconnect;

        public delegate void OnUserDisconnectCallback();
    }
}
