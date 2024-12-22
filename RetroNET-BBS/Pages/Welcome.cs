namespace RetroNET_BBS.Pages
{
    public class Pages
    {
        private const string Welcome = "<red>" +
            "000  0000 0000 000   00  0  0 0000 0000\r" +
            "0  0 0     0   0  0 0  0 00 0 0     0\r" +
            "0  0 0     0   0  0 0  0 00 0 0     0\r" +
            "000  000   0   000  0  0 0 00 000   0\r" +
            "00   0     0   00   0  0 0 00 0     0\r" +
            "0 0  0     0   0 0  0  0 0  0 0     0\r" +
            "0  0 0000  0   0  0  00  0  0 0000  0\r\r\r" +
            "<yellow>Welcome to RetroNET!\r\r" +
            "<green>Current date is: {0}\r" +
            "Online users: {1}\r";

        public static string ShowWelcome(int onlineUsers)
        {
            return string.Format(Welcome, DateTime.Now.ToString(), onlineUsers);
        }
    }
}
