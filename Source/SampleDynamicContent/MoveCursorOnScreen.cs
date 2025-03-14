using Encoder;
using Parser;
using System.Text;

namespace SampleDynamicContent
{
    public class MoveCursorOnScreen : ConnectionHandler
    {
        public override string Content(string navigationOptions)
        {
            var output = new StringBuilder();
            output.Append("<white>This is dynamic content!! <blue>So proud!");

            return output.ToString();
        }

        public override string Footer(string navigationOptions)
        {
            var output = new StringBuilder();

            // Cursor home
            output.Append("<home>");

            // Cursor down
            for (int i = 0; i < 24; i++)
            {
                output.Append("<crsrdown>"); ;
            }

            output.Append(navigationOptions);
            output.Append("FOOTER");

            return output.ToString();
        }

        public override char HandleInput(string receivedMessage, IEncoder encoder)
        {
            //if (string.Equals(receivedMessage, QuitCommand.ToString(), StringComparison.InvariantCultureIgnoreCase))
            //{
            //}
            return (char)0;
        }
    }
}
