using Encoder;
using System.Net.Sockets;
using System.Text;

namespace Parser
{
    public abstract class ConnectionHandler : IDynamicContent
    {
        public abstract string Content(string navigationOptions);
        public abstract string Footer(string navigationOptions);

        public abstract char HandleInput(string receivedMessage, IEncoder encoder);

        public async Task<string> HandleConnectionFlow(NetworkStream stream, IEncoder encoder)
        {
            var buffer = new byte[1024];
            var output = new StringBuilder();

            output.Append(Content(string.Empty));
            output.Append(Footer(string.Empty));

            byte[] response = encoder.FromAscii(output.ToString(), true);
            await stream.WriteAsync(response, 0, response.Length);

            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            HandleInput(data, encoder);

            return data;
        }
    }
}
