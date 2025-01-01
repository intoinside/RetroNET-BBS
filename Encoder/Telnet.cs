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
            // Fix upper/lower case
            var output = new byte[stream.Length];
            for (int i = 0; i < stream.Length; i++)
            {
                var charToConvert = (int)stream[i];

                //if (charToConvert >= 65 && charToConvert <= 90)
                //{
                //    output[i] = (byte)(charToConvert + 32);
                //    continue;
                //}

                //if (charToConvert >= 97 && charToConvert <= 122)
                //{
                //    output[i] = (byte)(charToConvert - 32);
                //    continue;
                //}

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
