using Common.Enum;
using RetroNET_BBS.Encoders;

namespace RetroNET_BBS.Templates
{
    public class Footer
    {
        public static string ShowFooter(string navigationOptions, Colors? color = null)
        {
            // Cursor home
            var output = "<home>";

            // Cursor down
            for (int i = 0; i < 24; i++)
            {
                output += "<crsrdown>";
            }

            if (color != null && Enum.IsDefined(typeof(Colors), color))
            {
                output += "<" + color.GetDescription() + ">";
            }

            output += navigationOptions;

            return output;
        }
    }
}
