using Markdig;
using Parser;

namespace RetroNET_BBS.ContentProvider
{
    public class MarkdownDataSource
    {
        Parser.Markdown markdown;
        
        public static MarkdownDataSource Instance => _instance.Value;

        private static readonly Lazy<MarkdownDataSource> _instance =
            new Lazy<MarkdownDataSource>(() => new MarkdownDataSource());

        MarkdownDataSource()
        {
            markdown = new Parser.Markdown();
        }

        public string Home()
        {
            return markdown.Home().Content;
        }

        public string GetPage(string pageId)
        {
            return string.Empty;
        }
    }
}
