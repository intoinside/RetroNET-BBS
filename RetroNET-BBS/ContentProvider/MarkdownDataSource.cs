//using Common.Dto;
//using Common.Enum;
//using Common.Utils;
//using Parser.Markdown;
//using Parser.Markdown.Dto;
//using RetroNET_BBS.Encoders;
//using System.Security.Cryptography;
//using System.Text;

//namespace RetroNET_BBS.ContentProvider
//{
//    public class MarkdownDataSource
//    {
//        Markdown markdown;

//        public static MarkdownDataSource Instance => _instance.Value;

//        private static readonly Lazy<MarkdownDataSource> _instance =
//            new Lazy<MarkdownDataSource>(() => new MarkdownDataSource());

//        private Dictionary<string, MarkdownDto> mardownDtos = new Dictionary<string, MarkdownDto>();

//        MarkdownDataSource()
//        {
//            markdown = new Markdown();

//            var dto = markdown.GetMd();

//            mardownDtos.Add("index", dto);
//        }

//        public Page GetHome(IEncoder encoder)
//        {
//            var markdownToShow = mardownDtos["index"];
//            var acceptedDetailIndex = string.Empty;
//            var linkedContents = new List<ContentsType>();

//            int i = 0;
//            var content = new StringBuilder();

//            // Upper offset
//            content.AppendLine("<lightgray><crsrdown><crsrdown><crsrdown><crsrdown>");

//            foreach (var item in markdownToShow.Articles)
//            {
//                var bulletNumber = i + (i < 9 ? 48 : 55);

//                var bulletNumberChar = (char)(bulletNumber + 1);

//                acceptedDetailIndex += bulletNumberChar;

//                content.Append(StringUtils.CreateBulletNumber(bulletNumber + 1));
//                content.Append(" ");

//                var itemTitle = encoder.Cleaner(item.Title);
//                content.AppendLine(StringUtils.SplitToLines(itemTitle, encoder.NumberOfColumns() - 8).First() + "...");

//                linkedContents.Add(new ContentsType() { BulletItem = bulletNumberChar, Source = item.Type, });

//                i++;
//            }

//            return new Page()
//            {
//                Hash = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(content.ToString()))),
//                Source = Sources.Markdown,
//                Title = "TITLE TO FIX",
//                Content = content.ToString(),
//                AcceptedDetailIndex = acceptedDetailIndex,
//                LinkedContentsType = linkedContents,
//            };
//        }

//        public Page GetPage(string pageId)
//        {
//            return new Page();
//        }
//    }
//}
