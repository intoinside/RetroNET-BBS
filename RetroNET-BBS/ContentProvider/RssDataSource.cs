using Common.Dto;
using Common.Enum;
using Common.Utils;
using Parser.Rss;
using Parser.Rss.Dto;
using RetroNET_BBS.Encoders;
using System.Text;

namespace RetroNET_BBS.ContentProvider
{
    public class RssDataSource
    {
        private Rss rss;

        public static RssDataSource Instance => _instance.Value;

        private static readonly Lazy<RssDataSource> _instance =
            new Lazy<RssDataSource>(() => new RssDataSource());

        private Dictionary<string, FeedDto> feeds = new Dictionary<string, FeedDto>();

        RssDataSource()
        {
        }

        public FeedDto RequestFeed(string url)
        {
            rss = new Rss(url);

            rss.Parse();

            var feed = rss.GetFeed();

            feeds.Add(url, feed);

            return feed;
        }

        public Pages GetHome(string url, IEncoder encoder)
        {
            var mainFeed = feeds[url];

            int i = 0;
            var content = new StringBuilder();
            foreach (var item in mainFeed.Articles)
            {
                var bulletNumber = i + (i < 9 ? 48 : 55);

                content.Append(StringUtils.CreateBulletNumber(bulletNumber + 1));
                content.Append(" ");

                var itemTitle = encoder.Cleaner(item.Title);
                content.AppendLine(StringUtils.SplitToLines(itemTitle, encoder.NumberOfColumn() - 8).First() + "...");

                content.AppendLine("    " + item.PublishDate.ToString("dd/MM/yyyy HH:mm"));

                if (i == 8)
                {
                    break;
                }

                i++;
            }

            return new Pages()
            {
                Source = Sources.Rss,
                Title = mainFeed.Title,
                Content = content.ToString(),
            };
        }

        //public string Home()
        //{
        //    return rss.Home().Content;
        //}

        //public string GetPage(string pageId)
        //{
        //    return string.Empty;
        //}
    }
}
