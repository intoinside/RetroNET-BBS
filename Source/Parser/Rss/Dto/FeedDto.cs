namespace Parser.Rss.Dto
{
    /// <summary>
    /// Dto for the rss feed
    /// </summary>
    public class FeedDto
    {
        /// <summary>
        /// Title of the feed
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of the feed
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Link of the feed
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Last update of the feed
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// List of the articles
        /// </summary>
        public List<FeedItemDto> Articles { get; set; } = new List<FeedItemDto>();
    }
}
