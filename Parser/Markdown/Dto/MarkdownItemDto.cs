using Common.Enum;

namespace Parser.Markdown.Dto
{
    public class MarkdownItemDto
    {
        public Sources Type { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public MarkdownItemDto()
        {
            Link = string.Empty;
            Title = string.Empty;
            Content = string.Empty;
        }
    }
}
