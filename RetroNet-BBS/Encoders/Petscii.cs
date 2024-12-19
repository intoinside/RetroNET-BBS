namespace RetroNet_BBS.Encoders
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Petscii"/> class.
    /// </summary>
    /// <param name="streamToConvert">The ASCII string to be converted to PETSCII.</param>
    public class Petscii : Encoder
    {
        private string stream;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="streamToConvert">Stream</param>
        public Petscii(string streamToConvert)
        {
            stream = streamToConvert;
        }

        /// <summary>
        /// Converts an ASCII string to a PETSCII byte array.
        /// </summary>
        /// <returns>A PETSCII encoded byte array representing the encoded ASCII string.</returns>
        public byte[] FromAscii()
        {
            stream = stream.Replace("<white>", new String((char)5, 1), true, null);
            stream = stream.Replace("<red>", new String((char)28, 1), true, null);
            stream = stream.Replace("<green>", new String((char)30, 1), true, null);
            stream = stream.Replace("<blue>", new String((char)31, 1), true, null);
            stream = stream.Replace("<orange>", new String((char)129, 1), true, null);
            stream = stream.Replace("<black>", new String((char)144, 1), true, null);
            stream = stream.Replace("<brown>", new String((char)144, 1), true, null);
            stream = stream.Replace("<lightred>", new String((char)150, 1), true, null);
            stream = stream.Replace("<pink>", new String((char)150, 1), true, null);
            stream = stream.Replace("<darkgrey>", new String((char)151, 1), true, null);
            stream = stream.Replace("<grey>", new String((char)152, 1), true, null);
            stream = stream.Replace("<lightgreen>", new String((char)153, 1), true, null);
            stream = stream.Replace("<lightblue>", new String((char)154, 1), true, null);
            stream = stream.Replace("<lightgrey>", new String((char)155, 1), true, null);
            stream = stream.Replace("<purple>", new String((char)156, 1), true, null);
            stream = stream.Replace("<yellow>", new String((char)158, 1), true, null);
            stream = stream.Replace("<cyan>", new String((char)159, 1), true, null);

            stream = stream.Replace("<revon>", new String((char)18, 1), true, null);
            stream = stream.Replace("<revoff>", new String((char)146, 1), true, null);

            stream = stream.Replace("<home>", new String((char)19, 1), true, null);
            stream = stream.Replace("<crsrdown>", new String((char)17, 1), true, null);
            stream = stream.Replace("<crsrright>", new String((char)29, 1), true, null);
            stream = stream.Replace("<crsrup>", new String((char)145, 1), true, null);
            stream = stream.Replace("<crsrleft>", new String((char)157, 1), true, null);

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
