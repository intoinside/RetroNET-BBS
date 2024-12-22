using RetroNET_BBS.Encoders;
using System.Net.Sockets;
using System.Text;

namespace RetroNET_BBS.Client
{
    public class PetsciiUser : User
    {
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
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    OnUserDisconnect();

                    break; // Client disconnected
                }

                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                messageBuilder.Append(data);

                if (data.Length > 0)
                {
                    string receivedMessage = messageBuilder.ToString().TrimEnd();

                    // Raise the MessageReceived event
                    //OnMessageReceived($"{client.Client.RemoteEndPoint}: {receivedMessage}");

                    messageBuilder.Clear();
                }
            }
        }
    }
}
