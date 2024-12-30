using System.Text;

namespace RetroNET_BBS.ContentProvider
{
    public class MarkdownDataSource
    {
        Parser.Markdown.Markdown markdown;
        
        public static MarkdownDataSource Instance => _instance.Value;

        private static readonly Lazy<MarkdownDataSource> _instance =
            new Lazy<MarkdownDataSource>(() => new MarkdownDataSource());

        MarkdownDataSource()
        {
            markdown = new Parser.Markdown.Markdown();
        }

        public string Home()
        {
            var content = new StringBuilder();

            // Upper offset
            content.AppendLine("<lightgray><crsrdown><crsrdown><crsrdown><crsrdown>");

            content.AppendLine(markdown.Home().Content);
            return content.ToString();
        }

        public string GetPage(string pageId)
        {
            return string.Empty;
        }
    }
}
