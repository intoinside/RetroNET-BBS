using Common.Enum;

namespace Common.Dto
{
    public class ContentsType
    {
        public Sources Source { get; set; }

        public string Path { get; set; }

        public char BulletItem { get; set; }

        public Page LinkedPage { get; set; }
    }

    public class Page
    {
        public string Hash { get; set; }

        public Sources Source { get; set; }

        public string Link { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string AcceptedDetailIndex { get; set; }

        public List<ContentsType> LinkedContentsType { get; set; }

        public Page()
        {
            LinkedContentsType = new List<ContentsType>();
        }

        public char HandleInput(string receivedMessage)
        {
            return (char)0;
        }
    }
}
