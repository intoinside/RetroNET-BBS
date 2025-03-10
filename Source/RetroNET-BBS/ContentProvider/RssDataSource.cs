using Common.Dto;
using Common.Enum;
using Common.Utils;
using Encoder;
using Parser.Rss;
using Parser.Rss.Dto;
using System.Text;

namespace RetroNET_BBS.ContentProvider
{
    public class RssDataSource
    {
        private Rss? rss;

        public static RssDataSource Instance => _instance.Value;

        private static readonly Lazy<RssDataSource> _instance =
            new Lazy<RssDataSource>(() => new RssDataSource());

        private Dictionary<string, FeedDto> feeds = new Dictionary<string, FeedDto>();

        RssDataSource()
        {
            rss = null;
        }

        /// <summary>
        /// Get the home page of the given url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public Page GetHome(string url, IEncoder encoder)
        {
            FeedDto? mainFeed;
            if (feeds.ContainsKey(url))
            {
                mainFeed = feeds[url];
            }
            else
            {
                mainFeed = RequestFeed(url);
            }

            var articleAvailableToShow = (encoder.NumberOfRows() - 8) / 2;
            var acceptedDetailIndex = string.Empty;

            int i = 0;
            var content = new StringBuilder();

            // Upper offset
            content.AppendLine("<lightgray>");

            foreach (var item in mainFeed.Articles)
            {
                var bulletNumber = i + (i < 9 ? 48 : 55);

                acceptedDetailIndex += (char)(bulletNumber + 1);

                content.Append(StringUtils.CreateBulletNumber(bulletNumber + 1));
                content.Append(" ");

                var itemTitle = encoder.Cleaner(item.Title);
                content.AppendLine(StringUtils.SplitToLines(itemTitle, encoder.NumberOfColumns() - 8).First() + "...");

                content.AppendLine("    " + item.PublishDate.ToString("dd/MM/yyyy HH:mm"));

                if (i == articleAvailableToShow)
                {
                    break;
                }

                i++;
            }

            return new Page()
            {
                Source = Sources.Rss,
                Title = mainFeed.Title,
                Content = content.ToString(),
                AcceptedDetailIndex = acceptedDetailIndex,
            };
        }

        /// <summary>
        /// Load a rss feed from the given url, parse it and put it in feeds array
        /// </summary>
        /// <param name="url">Url of rss feed</param>
        /// <returns>Feed dto</returns>
        private FeedDto RequestFeed(string url)
        {
            rss = new Rss(url);

            var feed = rss.GetFeed();

            feeds.Add(url, feed);

            return feed;
        }
    }
}
