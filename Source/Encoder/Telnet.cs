using System.Text;
using System.Text.RegularExpressions;

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
            { "<white>", "\x1B[1;37m" },
            { "<red>", "\x1B[31m" },
            { "<green>", "\x1B[32m" },
            { "<blue>", "\x1B[34m" },
            { "<orange>", "\x1B[33m" },
            { "<black>", "\x1B[30m" },
            { "<brown>", "\x1B[31m" },
            { "<lightred>", "\x1B[1;31m" },
            { "<pink>", "\x1B[1;35m" },
            { "<darkgray>", "\x1B[1;30m" },
            { "<darkgrey>", "\x1B[1;30m" },
            { "<gray>", "\x1B[1;30m" },
            { "<grey>", "\x1B[1;30m" },
            { "<lightgreen>", "\x1B[1;32m" },
            { "<lightblue>", "\x1B[1;34m" },
            { "<lightgray>", "\x1B[37m" },
            { "<lightgrey>", "\x1B[37m" },
            { "<purple>",  "\x1B[35m" },
            { "<yellow>", "\x1B[1;33m" },
            { "<cyan>", "\x1B[36m" },
            { "<revon>", "\x1B[7m" },
            { "<revoff>", "\x1B[27m" },
  
            // Convert position tags
            { "<home>", "\x1B[1;1H" },
            { "<crsrdown>", "\x1B[B" },
            { "<crsrright>", "\x1B[C"},
            { "<crsrup>", "\x1B[A" },
            { "<crsrleft>", "\x1B[D" },
        };

        /// <summary>
        /// Constructor
        /// </summary>
        public Telnet()
        {
        }

        public string Cleaner(string input)
        {
            return input;
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

            stream = stream.Replace("’", "'");
            stream = stream.Replace("“", "\"");
            stream = stream.Replace("”", "\"");

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
