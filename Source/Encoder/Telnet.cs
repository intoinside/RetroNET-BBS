using System.Text;
using System.Text.RegularExpressions;
using Common;

namespace Encoder
{
    /// <summary>
    /// Provides an encoder for Telnet connection. Screen size is 80x25 and import files are not supported.
    /// </summary>
    public class Telnet : IEncoder
    {
        /// <summary>
        /// Map to convert ASCII to Telnet escape sequences
        /// </summary>
        private static Dictionary<string, string> ConversionMap = new Dictionary<string, string>()
        {
            { Constants.Colors.White, "\x1B[1;37m" },
            { Constants.Colors.Red, "\x1B[31m" },
            { Constants.Colors.Green, "\x1B[32m" },
            { Constants.Colors.Blue, "\x1B[34m" },
            { Constants.Colors.Orange, "\x1B[33m" },
            { Constants.Colors.Black, "\x1B[30m" },
            { Constants.Colors.Brown, "\x1B[31m" },
            { Constants.Colors.LightRed, "\x1B[1;31m" },
            { Constants.Colors.Pink, "\x1B[1;35m" },
            { Constants.Colors.DarkGray, "\x1B[1;30m" },
            { Constants.Colors.DarkGrey, "\x1B[1;30m" },
            { Constants.Colors.Gray, "\x1B[1;30m" },
            { Constants.Colors.Grey, "\x1B[1;30m" },
            { Constants.Colors.LightGreen, "\x1B[1;32m" },
            { Constants.Colors.LightBlue, "\x1B[1;34m" },
            { Constants.Colors.LightGray, "\x1B[37m" },
            { Constants.Colors.LightGrey, "\x1B[37m" },
            { Constants.Colors.Purple,  "\x1B[35m" },
            { Constants.Colors.Yellow, "\x1B[1;33m" },
            { Constants.Colors.Cyan, "\x1B[36m" },
            { Constants.Colors.RevOn, "\x1B[7m" },
            { Constants.Colors.RevOff, "\x1B[27m" },
  
            // Convert position tags
            { Constants.Cursor.Home, "\x1B[1;1H" },
            { Constants.Cursor.Down, "\x1B[B" },
            { Constants.Cursor.Right, "\x1B[C"},
            { Constants.Cursor.Up, "\x1B[A" },
            { Constants.Cursor.Left, "\x1B[D" },
        };

        /// <summary>
        /// Constructor
        /// </summary>
        public Telnet()
        {
        }

        public string Cleaner(string input)
        {
            var stream = input.Replace("“", "\"");
            stream = stream.Replace("”", "\"");
            stream = stream.Replace("’", "'");
            stream = stream.Replace("‘", "'");

            return stream;
        }

        public int NumberOfRows()
        {
            return 25;
        }

        public int NumberOfColumns()
        {
            return 80;
        }

        private string ClearImport(string stream)
        {
            string pattern = @"<import\s+path=""([^""]+)"">";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(stream);

            foreach (Match match in matches)
            {
                stream = regex.Replace(stream, string.Empty);
            }

            return stream;
        }

        public byte[] FromAscii(string stream, bool clearPage = false)
        {
            // Clear page if requested
            if (clearPage)
            {
                stream = "\x1B[2J" + stream;
            }

            stream = ClearImport(stream);

            foreach (var entry in ConversionMap)
            {
                stream = stream.Replace(entry.Key, entry.Value, true, null);
            }

            var output = new byte[stream.Length];
            for (int i = 0; i < stream.Length; i++)
            {
                var charToConvert = (int)stream[i];

                output[i] = (byte)charToConvert;
            }

            return output;
        }

        public string ToAscii(byte[] buffer, int bytesRead)
        {
            var stream = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            foreach (var entry in ConversionMap)
            {
                stream = stream.Replace(entry.Value, entry.Key, true, null);
            }

            return stream;
        }
    }
}
