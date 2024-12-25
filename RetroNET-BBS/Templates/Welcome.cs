namespace RetroNET_BBS.Templates
{
    public class WelcomePage
    {
        private const string Welcome = "<lightred>" +
            "***                      *  * **** ****\r" +
            "*  *                     ** * *     *\r" +
            "*  *  **   *   *     **  ** * *     *\r" +
            "***  *  * ***  ***  *  * * ** ***   *\r" +
            "**   ***   *   *  * *  * * ** *     *\r" +
            "* *  *     * * *    *  * *  * *     *\r" +
            "*  *  **    *  *     **  *  * ****  *\r\r\r" +
            "<yellow>Welcome to RetroNET!\r\r" +
            "https://bit.ly/RetroNET-BBS\r" +
            "by Raffaele Intorcia\r\r" +
            "<green>Current date is: {0}\r" +
            "Online users: {1}\r";

        public static string ShowWelcome(int onlineUsers)
        {
            return string.Format(Welcome, DateTime.Now.ToString(), onlineUsers);
        }
    }
}
