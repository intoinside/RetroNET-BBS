using System.Net.Sockets;
using System.Net;
using RetroNET_BBS.Client;

namespace RetroNET_BBS.Server
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
            var user = new PetsciiUser(client, clientConnectedCount);
            user.OnUserDisconnect = () =>
            {
                clientConnectedCount--;
            };
        }

        protected virtual void OnMessageReceived(string message)
        {
            Console.WriteLine(message);
        }
    }
}
