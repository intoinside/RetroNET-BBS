using System.Net.Sockets;
using System.Net;
using System.Text;

namespace RetroNet_BBS.Server
{
    public class Server
    {
        private TcpListener Listener;
        private readonly string IpAddress;
        private readonly int Port;

        public Server(string host)
        {
            IpAddress = "192.168.1.2";
            Port = 8502;
        }

        public async Task Start()
        {
            // Abort operation if server is already running
            if (Listener != null)
            {
                throw new InvalidOperationException("Server is already running!");
            }

            IPAddress localAddr = IPAddress.Parse(IpAddress);
            Listener = new TcpListener(localAddr, Port);
            Listener.Start();

            OnMessageReceived("Server started. Waiting for a connection...");

            while (true)
            {
                // Handle new client connection
                TcpClient client = await Listener.AcceptTcpClientAsync();
                OnMessageReceived($"Connected! Client IP: {client.Client.RemoteEndPoint}");
                _ = HandleClientAsync(client);
            }
        }

        public void Stop()
        {
            Listener?.Stop();
            Listener = null;
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            StringBuilder messageBuilder = new StringBuilder();

            // Receive data in a loop until the client disconnects
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    OnMessageReceived($"Client {client.Client.RemoteEndPoint} disconnected.");
                    break; // Client disconnected
                }

                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                messageBuilder.Append(data);

                if (data.Length > 0)
                {
                    string receivedMessage = messageBuilder.ToString().TrimEnd();

                    // Raise the MessageReceived event
                    OnMessageReceived($"{client.Client.RemoteEndPoint}: {receivedMessage}");

                    byte[] response = Encoders.Petscii.FromAscii("<yellow>Hello <green>world! <white>0<orange>1<red>2<green>3<lightgreen>4<blue>5<lightblue>6789");
                    await stream.WriteAsync(response, 0, response.Length);

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
