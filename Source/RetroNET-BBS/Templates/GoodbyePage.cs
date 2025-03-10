namespace RetroNET_BBS.Templates
{
    /// <summary>
    /// Goodbye page template
    /// </summary>
    public class GoodbyePage
    {
        private const string Goodbye = "<lightred>Goodbye!\r\n\r\n" +
            "<yellow>Thanks for visiting RetroNET!\r\n\r\n" +
            "https://bit.ly/RetroNET-BBS\r\n";

        public static string ShowGoodbye()
        {
            return Goodbye;
        }
    }
}
