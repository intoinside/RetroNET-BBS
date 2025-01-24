using System.Text;
using System.Text.RegularExpressions;

namespace Encoder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Petscii"/> class.
    /// </summary>
    /// <param name="streamToConvert">The ASCII string to be converted for Telnet connection.</param>
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

        /// <summary>
        /// Provide conversion and cleaning of input string in order to
        /// be compliant to the selected encoding.
        /// </summary>
        /// <param name="input">Stream to clean</param>
        /// <returns>Stream cleaned</returns>
        public string Cleaner(string input)
        {
            return input;
        }

        /// <summary>
        /// Number of rows
        /// </summary>
        /// <returns>Number of rows for encoding</returns>
        public int NumberOfRows()
        {
            return 25;
        }

        /// <summary>
        /// Number of characters per row
        /// </summary>
        /// <returns>Number of column for encoding</returns>
        public int NumberOfColumns()
        {
            return 80;
        }

        /// <summary>
        /// Replacing import tags with empty string
        /// </summary>
        /// <remarks>Import tag is not supported on telnet connection</remarks>
        /// <param name="stream">Stream with import tags</param>
        /// <returns>Stream edited</returns>
        public string ClearImport(string stream)
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

        /// <summary>
        /// Converts an ASCII string to a byte array.
        /// </summary>
        /// <param name="stream">Stream to be converted</param>
        /// <param name="clearPage">Whether to clear the page before writing the stream</param>
        /// <returns>A byte array representing the encoded ASCII string.</returns>
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
