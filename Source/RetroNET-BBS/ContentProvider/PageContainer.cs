using Common.Dto;
using Common.Enum;
using Common.Utils;
using Encoder;
using System.Text;

namespace RetroNET_BBS.ContentProvider
{
    /// <summary>
    /// Container for the parsed pages
    /// </summary>
    public static class PageContainer
    {
        /// <summary>
        /// List of the parsed pages
        /// </summary>
        public static List<Page> Pages = new List<Page>();

        /// <summary>
        /// List of the parsed import files
        /// </summary>
        public static Dictionary<string, string> Imports = new Dictionary<string, string>();

        /// <summary>
        /// Search for the page from the given link
        /// </summary>
        /// <param name="link">Link to search</param>
        /// <returns>Page parsed (if present)</returns>
        public static Page? FindPageFromLink(string link)
        {
            return Pages.FirstOrDefault(p => p.Link == link);
        }

        /// <summary>
        /// Get the next content based on the current content
        /// </summary>
        /// <param name="content">Current content</param>
        /// <param name="encoder">Encoder to use</param>
        /// <returns>Page for the next content</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Page? GetNextContent(ContentsType content, IEncoder encoder)
        {
            switch (content.Source)
            {
                case Sources.Markdown:
                    return FindPageFromLink(content.Link);
                case Sources.Rss:
                    return RssDataSource.Instance.GetHome(content.Link, encoder);
                case Sources.Raw:
                case Sources.Dynamic:
                    throw new NotImplementedException();
                default:
                    return null;
            }
        }

        /// <summary>
        ///  Get document with tag stripping based on line length
        /// </summary>
        /// <param name="document">Document</param>
        /// <param name="encoder">Encoder which contains number of columns</param>
        /// <param name="pageNumber">Page to show</param>
        /// <returns>Splitted document</returns>
        public static string GetPage(string document, IEncoder encoder, ref int pageNumber)
        {
            var builder = new StringBuilder();
            using var stringWriter = new StringWriter(builder)
            {
                NewLine = "\r\n"
            };
            builder.Append("<lightgray>");

            var linesSplitted = StringUtils.SplitToLines(document, encoder.NumberOfColumns() - 1);

            int maxPageNumber = (linesSplitted.Count() / encoder.NumberOfRows()) + 1;

            if (pageNumber > maxPageNumber)
            {
                pageNumber = maxPageNumber;
            }
            else if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            var linesToShow = linesSplitted
                .Skip((pageNumber - 1) * encoder.NumberOfRows())
                .Take(encoder.NumberOfRows() - 1);

            foreach (var line in linesToShow)
            {
                stringWriter.WriteLine(line);
            }

            return builder.ToString();
        }
    }
}
