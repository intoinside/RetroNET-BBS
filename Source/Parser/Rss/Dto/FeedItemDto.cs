namespace Parser.Rss.Dto
{
    public class FeedItemDto
    {
        public string Link { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }

        public FeedItemDto()
        {
            Link = string.Empty;
            Title = string.Empty;
            Content = string.Empty;
            PublishDate = DateTime.Today;
        }
    }
}
