using Common.Dto;

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
        /// Search for the page from the given link
        /// </summary>
        /// <param name="link">Link to search</param>
        /// <returns>Page parsed (if present)</returns>
        public static Page? FindPageFromLink(string link)
        {
            return Pages.FirstOrDefault(p => p.Link == link);
        }
    }
}
