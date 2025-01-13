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

            // Convert color tags
            stream = stream.Replace("<white>", "\x1B[1;37m", true, null);
            stream = stream.Replace("<red>", "\x1B[31m", true, null);
            stream = stream.Replace("<green>", "\x1B[32m", true, null);
            stream = stream.Replace("<blue>", "\x1B[34m", true, null);
            stream = stream.Replace("<orange>", "\x1B[33m", true, null);
            stream = stream.Replace("<black>", "\x1B[30m", true, null);
            stream = stream.Replace("<brown>", "\x1B[31m", true, null);
            stream = stream.Replace("<lightred>", "\x1B[1;31m", true, null);
            stream = stream.Replace("<pink>", "\x1B[1;35m", true, null);
            stream = stream.Replace("<darkgray>", "\x1B[1;30m", true, null);
            stream = stream.Replace("<darkgrey>", "\x1B[1;30m", true, null);
            stream = stream.Replace("<gray>", "\x1B[1;30m", true, null);
            stream = stream.Replace("<grey>", "\x1B[1;30m", true, null);
            stream = stream.Replace("<lightgreen>", "\x1B[1;32m", true, null);
            stream = stream.Replace("<lightblue>", "\x1B[1;34m", true, null);
            stream = stream.Replace("<lightgray>", "\x1B[37m", true, null);
            stream = stream.Replace("<lightgrey>", "\x1B[37m", true, null);
            stream = stream.Replace("<purple>", "\x1B[35m", true, null);
            stream = stream.Replace("<yellow>", "\x1B[1;33m", true, null);
            stream = stream.Replace("<cyan>", "\x1B[36m", true, null);

            stream = stream.Replace("<revon>", "\x1B[7m", true, null);
            stream = stream.Replace("<revoff>", "\x1B[7m", true, null);

            // Convert position tags
            stream = stream.Replace("<home>", "\x1B[1;1H", true, null);
            stream = stream.Replace("<crsrdown>", "\x1B[B", true, null);
            stream = stream.Replace("<crsrright>", "\x1B[C", true, null);
            stream = stream.Replace("<crsrup>", "\x1B[A", true, null);
            stream = stream.Replace("<crsrleft>", "\x1B[D", true, null);

            var output = new byte[stream.Length];
            for (int i = 0; i < stream.Length; i++)
            {
                var charToConvert = (int)stream[i];

                output[i] = (byte)charToConvert;
            }

            return output;
        }
    }
}
