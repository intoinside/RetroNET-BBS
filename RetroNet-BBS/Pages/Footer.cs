using RetroNET_BBS.Encoders;
using System.ComponentModel;

namespace RetroNET_BBS.Pages
{
    public class Footer
    {
        public static string ShowFooter(string navigationOptions, Colors? color = null)
        {
            var output = "\x13";
            for (int i = 0; i < 24; i++)
            {
                output += "\x11";
            }

            if (color != null && Enum.IsDefined(typeof(Colors), color))
            {
                output += "<" + color.GetDescription() + ">";
            }

            output += navigationOptions;
            return output;
        }

    }

    public static class Extensions
    {
        static public string GetDescription(this Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            if (field == null)
                return enumValue.ToString();

            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }

            return enumValue.ToString();
        }
    }

}
