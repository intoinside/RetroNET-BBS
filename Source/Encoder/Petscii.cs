using System.Text;
using System.Text.RegularExpressions;
using Common;

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
            { Constants.Colors.White, new String((char)5, 1) },
            { Constants.Colors.Red, new String((char)28, 1) },
            { Constants.Colors.Green, new String((char)30, 1) },
            { Constants.Colors.Blue, new String((char)31, 1) },
            { Constants.Colors.Orange, new String((char)129, 1) },
            { Constants.Colors.Black, new String((char)144, 1) },
            { Constants.Colors.Brown, new String((char)149, 1) },
            { Constants.Colors.LightRed, new String((char)150, 1) },
            { Constants.Colors.Pink, new String((char)150, 1) },
            { Constants.Colors.DarkGray, new String((char)151, 1) },
            { Constants.Colors.DarkGrey, new String((char)151, 1) },
            { Constants.Colors.Gray, new String((char)152, 1) },
            { Constants.Colors.Grey, new String((char)152, 1) },
            { Constants.Colors.LightGreen, new String((char)153, 1) },
            { Constants.Colors.LightBlue, new String((char)154, 1) },
            { Constants.Colors.LightGray, new String((char)155, 1) },
            { Constants.Colors.LightGrey, new String((char)155, 1) },
            { Constants.Colors.Purple, new String((char)156, 1) },
            { Constants.Colors.Yellow, new String((char)158, 1) },
            { Constants.Colors.Cyan, new String((char)159, 1) },
            { Constants.Colors.RevOn, new String((char)18, 1) },
            { Constants.Colors.RevOff, new String((char)146, 1) },

            // Convert position tags
            { Constants.Cursor.Home,  new String((char)19, 1) },
            { Constants.Cursor.Down,  new String((char)17, 1) },
            { Constants.Cursor.Right,  new String((char)29, 1) },
            { Constants.Cursor.Up,  new String((char)145, 1) },
            { Constants.Cursor.Left,  new String((char)157, 1) },
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
            stream = stream.Replace("‘", "\x27", false, null);
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
