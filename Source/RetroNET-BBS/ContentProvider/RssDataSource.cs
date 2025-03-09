using Common.Dto;
using Common.Enum;
using Common.Utils;
using Encoder;
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
            content.AppendLine("<lightgray><crsrdown><crsrdown><crsrdown><crsrdown>");

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

        public Page GetPage(string url, char selection, IEncoder encoder)
        {
            var requestedFeed = feeds[url];
            var rowsToShow = encoder.NumberOfRows() - 8;

            int index = 0;
            if (selection <= 57 && selection >= 48)
            {
                index = selection - 48;
            }
            else
            {
                index = selection - 65;
            }

            var content = new StringBuilder();

            // Upper offset
            content.AppendLine("<lightgray><crsrdown><crsrdown><crsrdown><crsrdown>");
            content.AppendJoin('\r', StringUtils.SplitToLines(encoder.Cleaner(requestedFeed.Articles[index - 1].Content), encoder.NumberOfColumns() - 1).Take(rowsToShow));

            return new Page()
            {
                Source = Sources.Rss,
                Title = string.Empty,
                Content = content.ToString(),
                AcceptedDetailIndex = string.Empty,
            };
        }
    }
}
