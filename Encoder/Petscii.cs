using System.Text;
using System.Text.RegularExpressions;

namespace Encoder
{
    /// <summary>
    /// Provides an encoder for Petscii connection. Screen size is 40x25 and import files are supported.
    /// </summary>
    public class Petscii : IEncoder
    {
        /// <summary>
        /// Map for replacing Import tags
        /// </summary>
        private Dictionary<string, string> imports { get; set; }

        /// <summary>
        /// Map to convert ASCII to PETSCII
        /// </summary>
        private static Dictionary<string, string> ConversionMap = new Dictionary<string, string>() { 
            // Convert color tags
            { "<white>", new String((char)5, 1) },
            { "<red>", new String((char)28, 1) },
            { "<green>", new String((char)30, 1) },
            { "<blue>", new String((char)31, 1) },
            { "<orange>", new String((char)129, 1) },
            { "<black>", new String((char)144, 1) },
            { "<brown>", new String((char)149, 1) },
            { "<lightred>", new String((char)150, 1) },
            { "<pink>", new String((char)150, 1) },
            { "<darkgray>", new String((char)151, 1) },
            { "<darkgrey>", new String((char)151, 1) },
            { "<gray>", new String((char)152, 1) },
            { "<grey>", new String((char)152, 1) },
            { "<lightgreen>", new String((char)153, 1) },
            { "<lightblue>", new String((char)154, 1) },
            { "<lightgray>", new String((char)155, 1) },
            { "<lightgrey>", new String((char)155, 1) },
            { "<purple>", new String((char)156, 1) },
            { "<yellow>", new String((char)158, 1) },
            { "<cyan>", new String((char)159, 1) },
            { "<revon>", new String((char)18, 1) },
            { "<revoff>", new String((char)146, 1) },

            // Convert position tags
            { "<home>",  new String((char)19, 1) },
            { "<crsrdown>",  new String((char)17, 1) },
            { "<crsrright>",  new String((char)29, 1) },
            { "<crsrup>",  new String((char)145, 1) },
            { "<crsrleft>",  new String((char)157, 1) },
        };

        /// <summary>
        /// Constructor
        /// </summary>
        public Petscii(Dictionary<string, string> imports) : base()
        {
            this.imports = imports;
        }

        public string Cleaner(string input)
        {
            // Accented chars
            var stream = input.Replace("È", "E'", false, null);
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

        public int NumberOfRows()
        {
            return 25;
        }

        public int NumberOfColumns()
        {
            return 40;
        }

        private string SetupImport(string stream)
        {
            string pattern = @"<import\s+path=""([^""]+)"">";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(stream);

            foreach (Match match in matches)
            {
                stream = regex.Replace(stream, imports[match.Groups[1].Value]);
            }

            return stream;
        }

        public byte[] FromAscii(string stream, bool clearPage = false)
        {
            // Clear page if requested
            if (clearPage)
            {
                stream = new String((char)147, 1) + stream;
            }

            stream = SetupImport(stream);

            foreach (var entry in ConversionMap)
            {
                stream = stream.Replace(entry.Key, entry.Value, true, null);
            }

            // Fix upper/lower case
            var output = new byte[stream.Length];
            for (int i = 0; i < stream.Length; i++)
            {
                var charToConvert = (byte)stream[i];

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

                output[i] = charToConvert;
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
