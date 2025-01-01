using Common.Dto;

namespace RetroNET_BBS.ContentProvider
{
    public static class PageContainer
    {
        public static List<Page> Pages = new List<Page>();

        public static Page FindPageFromPath(string path)
        {
            return Pages.FirstOrDefault(p => p.Link == path);
        }
    }
}
