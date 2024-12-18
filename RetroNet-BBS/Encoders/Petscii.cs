using System.Text;

namespace RetroNet_BBS.Encoders
{
    public static class Petscii
    {
        public static byte[] FromAscii(string s)
        {
            s = s.Replace("<white>", new String((char)5, 1), true, null);
            s = s.Replace("<red>", new String((char)28, 1), true, null);
            s = s.Replace("<green>", new String((char)30, 1), true, null);
            s = s.Replace("<blue>", new String((char)31, 1), true, null);
            s = s.Replace("<orange>", new String((char)129, 1), true, null);
            s = s.Replace("<black>", new String((char)144, 1), true, null);
            s = s.Replace("<lightred>", new String((char)150, 1), true, null);
            s = s.Replace("<lightgreen>", new String((char)153, 1), true, null);
            s = s.Replace("<lightblue>", new String((char)154, 1), true, null);
            s = s.Replace("<purple>", new String((char)156, 1), true, null);
            s = s.Replace("<yellow>", new String((char)158, 1), true, null);
            s = s.Replace("<cyan>", new String((char)159, 1), true, null);

            var output = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                var charToConvert = (int)s[i];

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
