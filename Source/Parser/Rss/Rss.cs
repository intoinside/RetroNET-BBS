using Parser.Rss.Dto;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Parser.Rss
{
    /// <summary>
    /// Rss parser
    /// </summary>
    public class Rss
    {
        private string url;

        public Rss(string rssUrl)
        {
            url = rssUrl;
        }

        /// <summary>
        /// Gets and parses the feed starting for a given url
        /// </summary>
        /// <returns>Feed parsed</returns>
        public FeedDto GetFeed()
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
        }

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
