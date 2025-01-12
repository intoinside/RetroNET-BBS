using System.Text;

namespace PetsciiRawConverter
{
    public static class Converter
    {
        public static string FromPetscii(string path)
        {
            var stream = Encoding.GetEncoding(28591).GetString(File.ReadAllBytes(path));

            var builder = new StringBuilder();
            for (int i = 0; i < stream.Length; i++)
            {
                switch ((byte)stream[i])
                {
                    case 18: builder.Append("<revon>"); break;
                    case 146: builder.Append("<revoff>"); break;

                    case 19: builder.Append("<home>"); break;
                    case 145: builder.Append("<crsrup>"); break;
                    case 17: builder.Append("<crsrdown>"); break;
                    case 29: builder.Append("<crsrright>"); break;
                    case 157: builder.Append("<crsrleft>"); break;

                    case 5: builder.Append("<white>"); break;
                    case 28: builder.Append("<red>"); break;
                    case 30: builder.Append("<green>"); break;
                    case 31: builder.Append("<blue>"); break;
                    case 129: builder.Append("<orange>"); break;
                    case 144: builder.Append("<black>"); break;
                    case 149: builder.Append("<brown>"); break;
                    case 150: builder.Append("<lightred>"); break;
                    case 151: builder.Append("<darkgray>"); break;
                    case 152: builder.Append("<gray>"); break;
                    case 153: builder.Append("<lightgreen>"); break;
                    case 154: builder.Append("<lightblue>"); break;
                    case 155: builder.Append("<lightgray>"); break;
                    case 156: builder.Append("<purple>"); break;
                    case 158: builder.Append("<yellow>"); break;
                    case 159: builder.Append("<cyan>"); break;
                    default: builder.Append(stream[i]); break;
                }
            }

            return builder.ToString();
        }
    }
}
