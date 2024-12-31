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
        const char QuitCommand = 'q';
        const char HomeCommand = ',';
        const char BackCommand = '.';

        Stack<Page> history = new Stack<Page>();
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

            string acceptedNavigationOptions = string.Empty;
            char commandArrived = (char)0;

            Page currentPage = PageContainer.Pages.First();
            do
            {
                if (commandArrived == HomeCommand)
                {
                    history.Clear();
                    currentPage = PageContainer.Pages.First();
                    history.Push(currentPage);
                }

                if (commandArrived != BackCommand)
                {
                    history.Push(currentPage);

                    var nextPage = currentPage.LinkedContentsType.Where(x => x.BulletItem == commandArrived);
                    if (nextPage.Any())
                    {
                        currentPage = PageContainer.FindPageFromPath(nextPage.Single().Path);
                    }
                }
                else
                {
                    if (history.Count > 0)
                    {
                        currentPage = history.Pop();
                    }
                }

                //if (commandArrived == (char)0 || commandArrived == 'i')
                //{
                //    pages = RssDataSource.Instance.GetHome(url, petsciiEncoder);
                //}
                //else
                //{
                //    pages = RssDataSource.Instance.GetPage(url, commandArrived, petsciiEncoder);
                //}

                switch (currentPage.Source)
                {
                    case Sources.Markdown:
                        output = MarkdownStatic.GetHome(currentPage.Content, petsciiEncoder);
                        break;
                        //case Sources.Rss:
                        //    pages = RssDataSource.Instance.GetHome(pages.Source, commandArrived, petsciiEncoder);
                        //    break;
                }

                //output = pages.Content;

                output += Footer.ShowFooter(QuitCommand + "] quit " + HomeCommand + "] home " + BackCommand + "] back ", Colors.Yellow);
                response = petsciiEncoder.FromAscii(output, true);
                await stream.WriteAsync(response, 0, response.Length);

                commandArrived = (char)0;

                // Receive data in a loop until the client disconnects
                while (!connectionDone && commandArrived == (char)0)
                {
                    input = await HandleConnectionFlow(stream, buffer);

                    commandArrived = HandleInput(input, currentPage.AcceptedDetailIndex);
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
            if (string.Equals(receivedMessage, QuitCommand.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                Disconnect();
                return (char)0;
            }

            switch (receivedMessage[0])
            {
                case HomeCommand:
                case BackCommand:
                    return receivedMessage[0];
            }

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
