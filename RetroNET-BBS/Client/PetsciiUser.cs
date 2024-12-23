using RetroNET_BBS.ContentProvider;
using RetroNET_BBS.Encoders;
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
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            StringBuilder messageBuilder = new StringBuilder();

            var output = Pages.Pages.ShowWelcome(onlineUsers);
            output += Pages.Footer.ShowFooter("Navigation options", Colors.Lightgrey);

            byte[] response = new Petscii(output).FromAscii();

            await stream.WriteAsync(response, 0, response.Length);

            // Receive data in a loop until the client disconnects
            while (!connectionDone)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    Disconnect();

                    break; // Client disconnected
                }

                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                messageBuilder.Append(data);

                if (data.Length > 0)
                {
                    string receivedMessage = messageBuilder.ToString().TrimEnd();

                    HandleInput(receivedMessage);

                    //response = Encoding.ASCII.GetBytes(receivedMessage);
                    //await stream.WriteAsync(response, 0, response.Length);

                    messageBuilder.Clear();
                }
            }
        }

        private void HandleInput(string receivedMessage)
        {
            if (String.Equals(receivedMessage, "q", StringComparison.InvariantCultureIgnoreCase))
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
