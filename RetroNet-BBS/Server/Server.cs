using System.Net.Sockets;
using System.Net;
using System.Text;
using RetroNet_BBS.Encoders;
using RetroNet_BBS.Pages;

namespace RetroNet_BBS.Server
{
    public class Server
    {
        private TcpListener listener;
        private readonly string IpAddress;
        private readonly int Port;

        private int clientConnectedCount;

        public Server(string host)
        {
            IpAddress = "192.168.1.2";
            Port = 8502;
        }

        /// <summary>
        /// Creates a new server instance and starts listening for incoming connections.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task Start()
        {
            if (listener != null)
            {
                throw new InvalidOperationException("Server is already running!");
            }

            IPAddress localAddr = IPAddress.Parse(IpAddress);
            listener = new TcpListener(localAddr, Port);
            listener.Start();

            clientConnectedCount = 0;

            OnMessageReceived("Server started. Waiting for a connection...");

            while (true)
            {
                try
                {
                    while (true)
                    {
                        Accept(await listener.AcceptTcpClientAsync());
                    }
                }
                finally
                {
                    listener.Stop();
                }
            }
        }

        /// <summary>
        /// Accepts a new client connection and starts a new task to handle the client.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task Accept(TcpClient client)
        {
            clientConnectedCount++;

            await Task.Yield();
            await HandleClientAsync(client);
        }

        public void Stop()
        {
            listener.Stop();
            listener = null;
        }

        /// <summary>
        /// Handle a single connection
        /// </summary>
        /// <param name="client">Client handled</param>
        /// <returns>Task</returns>
        private async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            StringBuilder messageBuilder = new StringBuilder();

            byte[] response = new Petscii(Pages.Pages.ShowWelcome(clientConnectedCount)).FromAscii();

            await stream.WriteAsync(response, 0, response.Length);

            // Receive data in a loop until the client disconnects
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    OnMessageReceived($"Client {client.Client.RemoteEndPoint} disconnected.");

                    clientConnectedCount--;
                    break; // Client disconnected
                }

                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                messageBuilder.Append(data);

                if (data.Length > 0)
                {
                    string receivedMessage = messageBuilder.ToString().TrimEnd();

                    // Raise the MessageReceived event
                    OnMessageReceived($"{client.Client.RemoteEndPoint}: {receivedMessage}");

                    messageBuilder.Clear();
                }
            }
        }

        protected virtual void OnMessageReceived(string message)
        {
            Console.WriteLine(message);
        }
    }
}
