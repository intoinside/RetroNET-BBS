using System.Text;

namespace Encoder
{
    public class Telnet : IEncoder
    {
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

        public byte[] FromAscii(string stream, bool clearPage = false)
        {
            // Clear page if requested
            if (clearPage)
            {
                stream = new String((char)147, 1) + stream;
            }

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

            var output = new byte[stream.Length];
            for (int i = 0; i < stream.Length; i++)
            {
                var charToConvert = (int)stream[i];

                output[i] = (byte)charToConvert;
            }

            return output;
        }

        public int NumberOfColumns()
        {
            return 80;
        }

        public int NumberOfRows()
        {
            return 25;
        }
    }
}
