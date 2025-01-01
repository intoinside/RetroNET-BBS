namespace Encoder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Petscii"/> class.
    /// </summary>
    /// <param name="streamToConvert">The ASCII string to be converted to PETSCII.</param>
    public class Petscii : IEncoder
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Petscii()
        {
        }

        /// <summary>
        /// Provide conversion and cleaning of input string in order to
        /// be compliant to the selected encoding.
        /// </summary>
        /// <param name="stream">Stream to clean</param>
        /// <returns>Stream cleaned</returns>
        public string Cleaner(string stream)
        {
            // Accented chars
            stream = stream.Replace("È", "E'", false, null);
            stream = stream.Replace("à", "a'", false, null);
            stream = stream.Replace("è", "e'", false, null);
            stream = stream.Replace("é", "e'", false, null);
            stream = stream.Replace("ì", "i'", false, null);
            stream = stream.Replace("ò", "o'", false, null);
            stream = stream.Replace("ù", "u'", false, null);
            stream = stream.Replace("“", "\x22", false, null);
            stream = stream.Replace("”", "\x22", false, null);
            stream = stream.Replace("’", "\x27", false, null);
            stream = stream.Replace("&#8217;", "\x27", false, null);
            stream = stream.Replace("–", "\x2D", false, null);

            // Foreign char
            stream = stream.Replace("ý", "y", false, null);
            stream = stream.Replace("č", "c", false, null);
            stream = stream.Replace("í", "i", false, null);
            stream = stream.Replace("ě", "e", false, null);
            
            return stream;
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
            return 40;
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
                stream = new String((char)147, 1) + stream;
            }

            // Convert color tags
            stream = stream.Replace("<white>", new String((char)5, 1), true, null);
            stream = stream.Replace("<red>", new String((char)28, 1), true, null);
            stream = stream.Replace("<green>", new String((char)30, 1), true, null);
            stream = stream.Replace("<blue>", new String((char)31, 1), true, null);
            stream = stream.Replace("<orange>", new String((char)129, 1), true, null);
            stream = stream.Replace("<black>", new String((char)144, 1), true, null);
            stream = stream.Replace("<brown>", new String((char)149, 1), true, null);
            stream = stream.Replace("<lightred>", new String((char)150, 1), true, null);
            stream = stream.Replace("<pink>", new String((char)150, 1), true, null);
            stream = stream.Replace("<darkgray>", new String((char)151, 1), true, null);
            stream = stream.Replace("<darkgrey>", new String((char)151, 1), true, null);
            stream = stream.Replace("<gray>", new String((char)152, 1), true, null);
            stream = stream.Replace("<grey>", new String((char)152, 1), true, null);
            stream = stream.Replace("<lightgreen>", new String((char)153, 1), true, null);
            stream = stream.Replace("<lightblue>", new String((char)154, 1), true, null);
            stream = stream.Replace("<lightgray>", new String((char)155, 1), true, null);
            stream = stream.Replace("<lightgrey>", new String((char)155, 1), true, null);
            stream = stream.Replace("<purple>", new String((char)156, 1), true, null);
            stream = stream.Replace("<yellow>", new String((char)158, 1), true, null);
            stream = stream.Replace("<cyan>", new String((char)159, 1), true, null);

            stream = stream.Replace("<revon>", new String((char)18, 1), true, null);
            stream = stream.Replace("<revoff>", new String((char)146, 1), true, null);

            // Convert position tags
            stream = stream.Replace("<home>", new String((char)19, 1), true, null);
            stream = stream.Replace("<crsrdown>", new String((char)17, 1), true, null);
            stream = stream.Replace("<crsrright>", new String((char)29, 1), true, null);
            stream = stream.Replace("<crsrup>", new String((char)145, 1), true, null);
            stream = stream.Replace("<crsrleft>", new String((char)157, 1), true, null);

            // Fix upper/lower case
            var output = new byte[stream.Length];
            for (int i = 0; i < stream.Length; i++)
            {
                var charToConvert = (int)stream[i];

                if (charToConvert >= 65 && charToConvert <= 90)
                {
                    output[i] = (byte)(charToConvert + 32);
                    continue;
                }

                if (charToConvert >= 97 && charToConvert <= 122)
                { 
                    output[i] = (byte)(charToConvert - 32);
                    continue;
                }

                output[i] = (byte)charToConvert;
            }

            return output;
        }
    }
}
