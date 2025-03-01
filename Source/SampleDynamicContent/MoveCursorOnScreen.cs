using Encoder;
using Parser;
using System.Net.Sockets;
using System.Text;

namespace SampleDynamicContent
{
    public class MoveCursorOnScreen : IDynamicContent
    {
        public async Task<string> HandleConnectionFlow(NetworkStream stream, IEncoder encoder)
        {
            var buffer = new byte[1024];
            var output = "<white>This is dynamic content!! <blue>So proud!";
            byte[] response = encoder.FromAscii(output, true);
            await stream.WriteAsync(response, 0, response.Length);

            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            HandleInput(data, encoder);

            return data;
        }

        private char HandleInput(string receivedMessage, IEncoder encoder)
        {
            //if (string.Equals(receivedMessage, QuitCommand.ToString(), StringComparison.InvariantCultureIgnoreCase))
            //{
            //}
            return (char)0;
        }
    }
}
