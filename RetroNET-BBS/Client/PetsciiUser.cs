using RetroNET_BBS.ContentProvider;
using RetroNET_BBS.Encoders;
using RetroNET_BBS.Templates;
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
            var petsciiEncoder = new Petscii();

            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];

            // Show welcome page
            var output = WelcomePage.ShowWelcome(onlineUsers);
            byte[] response = petsciiEncoder.FromAscii(output);
            await stream.WriteAsync(response, 0, response.Length);

            // Not clear why this is needed but whatever
            await stream.ReadAsync(buffer, 0, buffer.Length);
            string input;
            do
            {
                input = await HandleConnectionFlow(stream, buffer);
            } while (input.Length == 0);

            //output = MarkdownDataSource.Instance.Home();
            var url = "https://www.punto-informatico.it/feed/";
            var feed = RssDataSource.Instance.RequestFeed(url);


            var pages = RssDataSource.Instance.GetHome(url, petsciiEncoder);

            output = pages.Content;

            output += Footer.ShowFooter("Navigation options", Colors.Lightgrey);
            response = petsciiEncoder.FromAscii(output, true);
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
