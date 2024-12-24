namespace RetroNET_BBS.ContentProvider
{
    public class RssDataSource
    {
        Parser.Rss rss;

        public static RssDataSource Instance => _instance.Value;

        private static readonly Lazy<RssDataSource> _instance =
            new Lazy<RssDataSource>(() => new RssDataSource());

        RssDataSource()
        {
            rss = new Parser.Rss("https://www.punto-informatico.it/feed/");
        }

        public string Home()
        {
            return rss.Home().Content;
        }

        public string GetPage(string pageId)
        {
            return string.Empty;
        }
    }
}
