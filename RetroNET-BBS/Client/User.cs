﻿using Common.Dto;
using Encoder;
using Parser;
using RetroNET_BBS.ContentProvider;
using RetroNET_BBS.Encoders;
using RetroNET_BBS.Templates;
using SampleDynamicContent;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace RetroNET_BBS.Client
{
    public abstract class User
    {
        public OnUserDisconnectCallback OnUserDisconnect;

        public delegate void OnUserDisconnectCallback();

        protected const char QuitCommand = 'q';
        protected const char HomeCommand = ',';
        protected const char BackCommand = '.';
        protected const char PrevScreenCommand = '-';
        protected const char NextScreenCommand = '+';

        protected TcpClient client;
        protected IEncoder encoder;

        protected bool connectionDone = false;
        protected Stack<Page> history = new Stack<Page>();

        private OnUserDisconnectCallback callback;

        public User(TcpClient client, OnUserDisconnectCallback callback)
        {
            this.client = client;
            this.callback = callback;
        }

        public async Task<string> ShowWelcomePage(int onlineUsers, NetworkStream stream)
        {
            var output = WelcomePage.ShowWelcome(onlineUsers);
            byte[] response = encoder.FromAscii(output, true);
            await stream.WriteAsync(response, 0, response.Length);

            //var buffer = new byte[1024];
            //int bytesRead = stream.Read(buffer, 0, buffer.Length);

            string input;
            do
            {
                input = await HandleConnectionFlow(stream, encoder);
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

            // Show welcome page
            var output = await ShowWelcomePage(onlineUsers, stream);

            byte[] response;

            string acceptedNavigationOptions = string.Empty;
            char commandArrived = (char)0;
            int currentScreen = 1;

            Page indexPage = PageContainer.Pages.Where(x => x.Link.Contains("index.md")).First();

            Page currentPage = indexPage;
            while (!connectionDone)
            {
                Type type = Type.GetType("SampleDynamicContent.MoveCursorOnScreen, SampleDynamicContent"); //target type
                object o = Activator.CreateInstance(type); // an instance of target type
                IDynamicContent your = (IDynamicContent)o;

                your.HandleConnectionFlow(stream, encoder);



                // Draws the page
                output = PageContainer.GetPage(currentPage.Content, encoder, ref currentScreen);

                // Add footer to output stream
                output += Footer.ShowFooter(QuitCommand
                    + "] quit "
                    + HomeCommand + "] home "
                    + BackCommand + "] back "
                    + PrevScreenCommand + "] -Scr "
                    + NextScreenCommand + "] +Scr"
                    , Colors.Yellow);

                // Encode the output stream correctly
                response = encoder.FromAscii(output, true);

                // Send the output stream to the client
                await stream.WriteAsync(response, 0, response.Length);

                string input = await HandleConnectionFlow(stream, encoder);

                commandArrived = HandleInput(input, currentPage.AcceptedDetailIndex);

                if (commandArrived == HomeCommand)
                {
                    // Home command, clear history and start from the first page
                    history.Clear();
                    currentPage = indexPage;
                    history.Push(currentPage);
                    currentScreen = 1;
                }
                else if (commandArrived == BackCommand)
                {
                    //Back command, go back to the previous page
                    currentScreen = 1;
                    if (history.Count > 0)
                    {
                        currentPage = history.Pop();
                    }
                }
                else if (commandArrived == PrevScreenCommand)
                {
                    // Go to the previous screen on the current page
                    currentScreen--;
                }
                else if (commandArrived == NextScreenCommand)
                {
                    // Go to the next screen on the current page
                    currentScreen++;
                }
                else
                {
                    // It's not a navigation command, so it's a link to another page
                    history.Push(currentPage);

                    var nextPage = currentPage.LinkedContentsType.Where(x => x.BulletItem == commandArrived);
                    if (nextPage.Any())
                    {
                        currentPage = PageContainer.FindPageFromLink(nextPage.Single().Link);
                        currentScreen = 1;
                    }
                }

                output = PageContainer.GetPage(currentPage.Content, encoder, ref currentScreen);
            }
        }

        /// <summary>
        /// Reads the stream and decodes it to ASCII.
        /// </summary>
        /// <param name="stream">Client stream</param>
        /// <param name="encoder">Encoder to decode into ASCII</param>
        /// <returns></returns>
        protected async Task<string> HandleConnectionFlow(NetworkStream stream, IEncoder encoder)
        {
            var buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead == 0)
            {
                Disconnect();

                connectionDone = true;
            }

            string data = encoder.ToAscii(buffer, bytesRead);

            return data;
        }

        /// <summary>
        /// Check input stream if is valid for the current page or if is a navigation command.
        /// </summary>
        /// <param name="receivedMessage">Message receviced</param>
        /// <param name="acceptedNavigationOptions">Accepted options for the current page</param>
        /// <returns></returns>
        protected char HandleInput(string receivedMessage, string acceptedNavigationOptions)
        {
            if (string.Equals(receivedMessage, QuitCommand.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                SendGoodbye(client).Wait();
                Disconnect();
                return (char)0;
            }

            switch (receivedMessage.First())
            {
                case HomeCommand:
                case BackCommand:
                case PrevScreenCommand:
                case NextScreenCommand:
                    return receivedMessage.First();
            }

            if (acceptedNavigationOptions.Contains(receivedMessage))
            {
                return receivedMessage.First();
            }

            return (char)0;
        }

        protected void Disconnect()
        {
            callback();
            connectionDone = true;
            client.Close();
        }
    }
}
