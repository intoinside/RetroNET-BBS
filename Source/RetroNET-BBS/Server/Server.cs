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

        private readonly IUserFactory userFactory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host"></param>
        public Server(string host, int port, ConnectionType type, IUserFactory userFactory)
        {
            listener = null;
            IpAddress = host;
            Port = port;
            connectionType = type;
            this.userFactory = userFactory;
        }

        /// <summary>
        /// Creates a new server instance and starts listening for incoming connections.
        /// </summary>
        /// <returns>Task</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <summary>
        /// Creates a new server instance and starts listening for incoming connections.
        /// </summary>
        /// <returns>Task</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task Start(CancellationToken stoppingToken)
        {
            if (listener != null)
            {
                throw new InvalidOperationException("Server is already running!");
            }

            IPAddress localAddr = IPAddress.Parse(IpAddress);
            listener = new TcpListener(localAddr, Port);
            listener.Start();

            OnMessageReceived("Server started on port  " + Port.ToString().PadLeft(5) + ". Waiting for a connection...");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var client = await listener.AcceptTcpClientAsync(stoppingToken);
                        _ = HandleClientConnection(client);
                    }
                    catch (OperationCanceledException)
                    {
                        // Stop requested
                        break;
                    }
                    catch (Exception ex)
                    {
                        OnMessageReceived($"Error accepting client: {ex.Message}");
                    }
                }
            }
            finally
            {
                Stop();
            }
        }

        /// <summary>
        /// Accepts a new client connection and starts a new task to handle the client.
        /// </summary>
        /// <param name="client">Client connected</param>
        /// <returns>Task</returns>
        /// <summary>
        /// Accepts a new client connection and starts a new task to handle the client.
        /// </summary>
        /// <param name="client">Client connected</param>
        /// <returns>Task</returns>
        private async Task HandleClientConnection(TcpClient client)
        {
            Interlocked.Increment(ref clientConnectedCount);

            OnMessageReceived("New connection on port  " + Port.ToString().PadLeft(5) + ": " + clientConnectedCount.ToString() + " clients");

            await Task.Yield();
            try 
            {
                await HandleClientAsync(client, connectionType);
            }
            catch (Exception ex)
            {
                OnMessageReceived($"Error handling client: {ex.Message}");
                client.Close();
            }
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
            User? user = userFactory.Create(connectionType, client, clientConnectedCount, OnUserDisconnect);
        }

        /// <summary>
        /// Callback when user disconnects
        /// </summary>
        public void OnUserDisconnect()
        {
            Interlocked.Decrement(ref clientConnectedCount);
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
