namespace RetroNET_BBS.Templates
{
    public class WelcomePage
    {
        private const string Welcome = "<lightred>" +
            "***                      *  * **** ****\r\n" +
            "*  *                     ** * *     *\r\n" +
            "*  *  **   *   *     **  ** * *     *\r\n" +
            "***  *  * ***  ***  *  * * ** ***   *\r\n" +
            "**   ***   *   *  * *  * * ** *     *\r\n" +
            "* *  *     * * *    *  * *  * *     *\r\n" +
            "*  *  **    *  *     **  *  * ****  *\r\n\r\n\r\n" +
            "<yellow>Welcome to RetroNET!\r\n\r\n" +
            "https://bit.ly/RetroNET-BBS\r\n" +
            "by Raffaele Intorcia\r\n\r\n" +
            "<green>Current date is {0}\r\n" +
            "Online users: {1}\r\n";

        public static string ShowWelcome(int onlineUsers)
        {
            return string.Format(Welcome, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), onlineUsers);
        }
    }
}
