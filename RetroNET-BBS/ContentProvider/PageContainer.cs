using Common.Dto;

namespace RetroNET_BBS.ContentProvider
{
    public static class PageContainer
    {
        public static List<Page> Pages = new List<Page>();

        public static Page FindPage(string hash)
        {
            return Pages.FirstOrDefault(p => p.Hash == hash);
        }
    }
}
