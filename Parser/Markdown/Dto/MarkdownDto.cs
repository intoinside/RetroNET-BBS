namespace Parser.Markdown.Dto
{
    public class MarkdownDto
    {
        public string Title { get; set; }

        public List<MarkdownItemDto> Articles { get; set; } = new List<MarkdownItemDto>();
    }
}
