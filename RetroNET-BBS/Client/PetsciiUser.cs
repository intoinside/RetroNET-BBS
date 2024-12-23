using RetroNET_BBS.ContentProvider;
using RetroNET_BBS.Encoders;
using System.Net.Sockets;
using System.Text;

namespace RetroNET_BBS.Client
{
    public class PetsciiUser : User
    {
        bool connectionDone = false;

        public PetsciiUser(TcpClient client, int onlineUsers) : base(client)
        {
            HandleConnection(onlineUsers);
        }

        public async Task HandleConnection(int onlineUsers)
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];

            var output = Pages.Pages.ShowWelcome(onlineUsers);
            byte[] response = new Petscii(output).FromAscii();
            await stream.WriteAsync(response, 0, response.Length);

            // Not clear why this is needed
            await stream.ReadAsync(buffer, 0, buffer.Length);
            string input;
            do
            {
                input = await HandleConnectionFlow(stream, buffer);
            } while (input.Length == 0);

            output = MarkdownDataSource.Instance.Home();
            output += Pages.Footer.ShowFooter("Navigation options", Colors.Lightgrey);
            response = new Petscii(output, true).FromAscii();
            await stream.WriteAsync(response, 0, response.Length);

            // Receive data in a loop until the client disconnects
            while (!connectionDone)
            {
                input = await HandleConnectionFlow(stream, buffer);

                HandleInput(input);
            }
        }

        private async Task<string> HandleConnectionFlow(NetworkStream stream, byte[] buffer)
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

        private void HandleInput(string receivedMessage)
        {
            if (string.Equals(receivedMessage, "q", StringComparison.InvariantCultureIgnoreCase))
            {
                Disconnect();
            }
        }

        private void Disconnect()
        {
            OnUserDisconnect();
            connectionDone = true;
            client.Close();
        }
    }
}
