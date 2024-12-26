using Parser.Rss.Dto;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Parser.Rss
{
    public class Rss
    {
        private string url;

        private FeedDto feed;

        public Rss(string rssUrl)
        {
            url = rssUrl;
        }

        public async void Parse()
        {
            feed = await ParseFeed();
        }

        public FeedDto GetFeed()
        {
            return feed;
        }

        private async Task<FeedDto> ParseFeed()
        {
            FeedDto entries = new FeedDto();

            string title = string.Empty;
            try
            {
                XDocument doc = XDocument.Load(url);

                var channelNode = doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements();

                entries.Title = channelNode.Where(i => i.Name.LocalName == "title").First().Value.ToString();
                entries.Description = channelNode.Where(i => i.Name.LocalName == "description").First().Value.ToString();
                entries.Link = channelNode.Where(i => i.Name.LocalName == "link").First().Value.ToString();
                entries.LastUpdated = ParseDate(channelNode.Where(i => i.Name.LocalName == "lastBuildDate").First().Value.ToString());

                var itemlist = from item in channelNode.Where(i => i.Name.LocalName == "item")
                               select new FeedItemDto
                               {
                                   Content = item.Elements().Any(i => i.Name.LocalName == "encoded")
                                       ? Regex.Replace(item.Elements().First(i => i.Name.LocalName == "encoded").Value, "<.*?>", string.Empty)
                                       : item.Elements().First(i => i.Name.LocalName == "description").Value,
                                   Link = item.Elements().First(i => i.Name.LocalName == "link").Value,
                                   PublishDate = ParseDate(item.Elements().First(i => i.Name.LocalName == "pubDate").Value),
                                   Title = item.Elements().First(i => i.Name.LocalName == "title").Value
                               };

                entries.Articles = itemlist.ToList();
            }
            catch (Exception ex)
            {
                return new FeedDto();
            }

            return entries;

            //var content = new StringBuilder();

            //int i = 0;
            //foreach (var item in entries)
            //{
            //    var bulletNumber = i + (i < 9 ? 48 : 55);

            //    content.Append("<revon><white> " + (char)(bulletNumber + 1) + " <revoff><lightgrey>");
            //    content.Append(" ");
            //    var itemTitle = item.Title.Trim();

            //    content.AppendLine(StringUtils.SplitToLines(itemTitle, 31).First() + "...");

            //    content.AppendLine("    " + item.PublishDate.ToString("dd/MM/yyyy HH:mm"));

            //    if (i == 8)
            //    {
            //        break;
            //    }

            //    i++;
            //}

            //Pages page = new Pages()
            //{
            //    Source = Sources.Rss,
            //    Title = title,
            //    Content = content.ToString(),
            //};

            //return page;
        }

        //public string Title()
        //{
        //    return index.Title;
        //}

        //public Pages Home()
        //{
        //    return index;
        //}

        private DateTime ParseDate(string date)
        {
            DateTime result;
            if (DateTime.TryParse(date, out result))
                return result;
            else
                return DateTime.MinValue;
        }
    }
}
