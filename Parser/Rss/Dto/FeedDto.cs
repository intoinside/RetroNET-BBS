namespace Parser.Rss.Dto
{
    public class FeedDto
    {
        // Informazioni sul feed
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public DateTime LastUpdated { get; set; }

        // Elenco degli articoli
        public List<FeedItemDto> Articles { get; set; } = new List<FeedItemDto>();
    }
}
