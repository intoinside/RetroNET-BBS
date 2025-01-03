using Common.Dto;
using Common.Enum;

namespace Parser.Raw
{
    public static class Raw
    {
        public static Page ParseFile(string path)
        {

            Page page = new Page()
            {
                Source = Sources.Raw,
                Link = path,
                Title = string.Empty,
                Content = "RAW",
                LinkedContentsType = new List<ContentsType>(),
                AcceptedDetailIndex = string.Empty,
            };

            return page;
        }
    }
}
