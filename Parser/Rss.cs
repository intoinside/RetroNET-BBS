using Common.Dto;
using Common.Enum;
using System.Text;
using System.Xml.Linq;

namespace Parser
{
    public class Rss : IParser
    {
        private string url;

        private Pages index;

        public Rss(string rssUrl)
        {
            url = rssUrl;

            index = ParseFeed().Result;
        }

        public void Parse(string folder)
        {

        }

        private async Task<Pages> ParseFeed()
        {
            IEnumerable<NewsFeedItem> entries;

            string title = string.Empty;
            try
            {
                XDocument doc = XDocument.Load(url);

                title = doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "title").First().Value.ToString();

                entries = from item in doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item")
                          select new NewsFeedItem
                          {
                              Content = item.Elements().First(i => i.Name.LocalName == "description").Value,
                              Link = item.Elements().First(i => i.Name.LocalName == "link").Value,
                              PublishDate = ParseDate(item.Elements().First(i => i.Name.LocalName == "pubDate").Value),
                              Title = item.Elements().First(i => i.Name.LocalName == "title").Value
                          };
            }
            catch (Exception ex)
            {
                return new Pages();
            }

            var content = new StringBuilder();

            int i = 0;
            foreach (var item in entries)
            {
                var bulletNumber = i + (i < 9 ? 48 : 55);

                content.Append("<revon><white> " + (char)(bulletNumber + 1) + " <revoff><lightgrey>");
                content.Append(" ");
                var itemTitle = item.Title.Trim();
                content.AppendLine(itemTitle.Substring(0, itemTitle.Length > 32 ? 30 : itemTitle.Length) + "...");
                content.AppendLine("    " + item.PublishDate.ToString("dd/MM/yyyy HH:mm"));

                if (i == 8)
                {
                    break;
                }

                i++;
            }

            Pages page = new Pages()
            {
                Source = Sources.Rss,
                Title = title,
                Content = content.ToString(),
            };

            return page;
        }

        public string Title()
        {
            return index.Title;
        }

        public Pages Home()
        {
            return index;
        }

        private DateTime ParseDate(string date)
        {
            DateTime result;
            if (DateTime.TryParse(date, out result))
                return result;
            else
                return DateTime.MinValue;
        }

        public class NewsFeedItem
        {
            public string Link { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public DateTime PublishDate { get; set; }

            public NewsFeedItem()
            {
                Link = string.Empty;
                Title = string.Empty;
                Content = string.Empty;
                PublishDate = DateTime.Today;
            }
        }
    }
}
