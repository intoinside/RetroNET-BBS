using Common.Dto;
using Encoder;
using RetroNET_BBS.ContentProvider;
using RetroNET_BBS.Encoders;
using RetroNET_BBS.Templates;
using System.Net.Sockets;
using System.Text;

namespace RetroNET_BBS.Client
{
    public abstract class User
    {
        public OnUserDisconnectCallback OnUserDisconnect;

        public delegate void OnUserDisconnectCallback();

        protected const char QuitCommand = 'q';
        protected const char HomeCommand = ',';
        protected const char BackCommand = '.';
        protected TcpClient client;
        protected IEncoder encoder;

        protected bool connectionDone = false;
        protected Stack<Page> history = new Stack<Page>();

        public User(TcpClient client)
        {
            this.client = client;
        }

        public async Task<string> ShowWelcomePage(int onlineUsers, NetworkStream stream)
        {
            var output = WelcomePage.ShowWelcome(onlineUsers);
            byte[] response = encoder.FromAscii(output, true);
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

        public async Task SendGoodbye(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            var output = GoodbyePage.ShowGoodbye();
            byte[] response = encoder.FromAscii(output, true);
            await stream.WriteAsync(response, 0, response.Length);
        }

        protected async Task HandleConnection(int onlineUsers)
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];

            // Show welcome page
            var output = await ShowWelcomePage(onlineUsers, stream);

            byte[] response;

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
                        currentPage = PageContainer.FindPageFromLink(nextPage.Single().Link);
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

                output = PageContainer.GetPage(currentPage.Content, encoder);

                output += Footer.ShowFooter(QuitCommand + "] quit " + HomeCommand + "] home " + BackCommand + "] back ", Colors.Yellow);

                response = encoder.FromAscii(output, true);
                await stream.WriteAsync(response, 0, response.Length);

                commandArrived = (char)0;

                // Receive data in a loop until the client disconnects
                while (!connectionDone && commandArrived == (char)0)
                {
                    string input = await HandleConnectionFlow(stream, buffer);

                    commandArrived = HandleInput(input, currentPage.AcceptedDetailIndex);
                }
            } while (!connectionDone);
        }

        protected async Task<string> HandleConnectionFlow(NetworkStream stream, byte[] buffer)
        {
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead == 0)
            {
                Disconnect();

                connectionDone = true;
            }

            string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            return data;
        }

        protected char HandleInput(string receivedMessage, string acceptedNavigationOptions)
        {
            if (string.Equals(receivedMessage, QuitCommand.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                SendGoodbye(client).Wait();
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

        protected void Disconnect()
        {
            OnUserDisconnect();
            connectionDone = true;
            client.Close();
        }
    }
}
