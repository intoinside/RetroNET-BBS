using Common.Dto;
using Common.Enum;
using Parser.Markdown;
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

            //var url = "https://www.apuliaretrocomputing.it/wordpress/feed/";
            //var feed = RssDataSource.Instance.RequestFeed(url);

            string acceptedNavigationOptions = string.Empty;
            char commandArrived = (char)0;

            Page pages = PageContainer.Pages.First();
            do
            {
                //if (commandArrived == (char)0 || commandArrived == 'i')
                //{
                //    pages = RssDataSource.Instance.GetHome(url, petsciiEncoder);
                //}
                //else
                //{
                //    pages = RssDataSource.Instance.GetPage(url, commandArrived, petsciiEncoder);
                //}

                //pages = MarkdownDataSource.Instance.GetHome(petsciiEncoder);

                switch (pages.Source)
                {
                    case Sources.Markdown:
                        output = MarkdownStatic.GetHome(pages.Content, petsciiEncoder);
                        break;
                    //case Sources.Rss:
                    //    pages = RssDataSource.Instance.GetHome(pages.Source, commandArrived, petsciiEncoder);
                    //    break;
                }

                //output = pages.Content;

                output += Footer.ShowFooter("q] quit i] index ", Colors.Yellow);
                response = petsciiEncoder.FromAscii(output, true);
                await stream.WriteAsync(response, 0, response.Length);

                commandArrived = (char)0;

                // Receive data in a loop until the client disconnects
                while (!connectionDone && commandArrived == (char)0)
                {
                    input = await HandleConnectionFlow(stream, buffer);

                    commandArrived = HandleInput(input, pages.AcceptedDetailIndex);
                }
            } while (!connectionDone);
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

        private char HandleInput(string receivedMessage, string acceptedNavigationOptions)
        {
            if (string.Equals(receivedMessage, "q", StringComparison.InvariantCultureIgnoreCase))
            {
                Disconnect();
                return (char)0;
            }

            if (string.Equals(receivedMessage, "i", StringComparison.InvariantCultureIgnoreCase))
            {
                return 'i';
            }

            //if (string.Equals(receivedMessage, acceptedNavigationOptions, StringComparison.InvariantCultureIgnoreCase))
            if (acceptedNavigationOptions.Contains(receivedMessage))
            {
                return receivedMessage.First();
            }

            return (char)0;
        }

        private void Disconnect()
        {
            OnUserDisconnect();
            connectionDone = true;
            client.Close();
        }
    }
}
