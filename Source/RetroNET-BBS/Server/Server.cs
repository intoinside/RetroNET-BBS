using System.Net.Sockets;
using System.Net;
using RetroNET_BBS.Client;

namespace RetroNET_BBS.Server
{
    /// <summary>
    /// Server class
    /// </summary>
    public class Server
    {
        private TcpListener? listener;
        private readonly string IpAddress;
        private readonly int Port;
        private ConnectionType connectionType;

        private static int clientConnectedCount = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host"></param>
        public Server(string host, int port, ConnectionType type)
        {
            listener = null;
            IpAddress = host;
            Port = port;
            connectionType = type;
        }

        /// <summary>
        /// Creates a new server instance and starts listening for incoming connections.
        /// </summary>
        /// <returns>Task</returns>
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

            OnMessageReceived("Server started on port  " + Port.ToString().PadLeft(5) + ". Waiting for a connection...");

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
        /// <param name="client">Client connected</param>
        /// <returns>Task</returns>
        private async Task Accept(TcpClient client)
        {
            clientConnectedCount++;

            OnMessageReceived("New connection on port  " + Port.ToString().PadLeft(5) + ": " + clientConnectedCount.ToString() + " clients");

            await Task.Yield();
            await HandleClientAsync(client, connectionType);
        }

        /// <summary>
        /// Stop the current server instance
        /// </summary>
        public void Stop()
        {
            if (listener == null)
            {
                return;
            }

            listener.Stop();
            listener = null;
        }

        /// <summary>
        /// Handle a single connection
        /// </summary>
        /// <param name="client">Client handled</param>
        /// <returns>Task</returns>
        private async Task HandleClientAsync(TcpClient client, ConnectionType connectionType)
        {
            User? user = null;
            switch (connectionType)
            {
                case ConnectionType.Petscii:
                    user = new PetsciiUser(client, clientConnectedCount, OnUserDisconnect);
                    break;
                case ConnectionType.Telnet:
                    user = new TelnetUser(client, clientConnectedCount, OnUserDisconnect);
                    break;
                default:
                    throw new NotSupportedException("Connection type not supported");
            }
        }

        /// <summary>
        /// Callback when user disconnects
        /// </summary>
        public void OnUserDisconnect()
        {
            clientConnectedCount--;
            OnMessageReceived("Connection lost on port " + Port.ToString().PadLeft(5) + ": " + clientConnectedCount.ToString() + " clients");
        }

        /// <summary>
        /// Callback when a message is received
        /// </summary>
        /// <param name="message"></param>
        protected virtual void OnMessageReceived(string message)
        {
            Console.WriteLine(message);
        }
    }
}
