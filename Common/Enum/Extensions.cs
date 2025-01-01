using System.ComponentModel;

namespace Common.Enum
{
    /// <summary>
    /// Extensions for Enum
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Get the description of the enum in string format
        /// </summary>
        /// <param name="enumValue">Enum to get</param>
        /// <returns>String description</returns>
        public static string GetDescription(this System.Enum enumValue)
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
