using Common.Enum;
using HtmlAgilityPack;
using Markdig.Syntax.Inlines;
using Markdig.Syntax;
using Parser.Markdown.Dto;
using Common.Dto;
using System.Text;

namespace Parser.Markdown
{
    public static class Markdown
    {
        static List<Page> pageToParse = new List<Page>();

        public static List<Page> ParseAllFiles(string path)
        {
            List<Page> pageParsed = new List<Page>();

            foreach (var file in Directory.GetFiles(path, "*.md"))
            {
                pageParsed.Add(ParseFile(file));
            }

            while (pageToParse.Count > 0)
            {
                var page = pageToParse.First();

                if (!pageParsed.Any(x => x.Link == page.Link))
                {
                    switch (page.Source)
                    {
                        case Sources.Markdown:
                            pageParsed.Add(ParseFile(Path.Combine(path, page.Link)));
                            break;
                        case Sources.Raw:
                            pageParsed.Add(Raw.Raw.ParseFile(Path.Combine(path, page.Link)));
                            break;
                    };
                }

                pageToParse.RemoveAt(0);
            }

            return pageParsed;
        }

        public static Page ParseFile(string path)
        {
            if (!File.Exists(path))
            {
                path += ".md";
            }

            var markdown = File.ReadAllText(path);
            var document = Markdig.Markdown.Parse(markdown);

            var heading = document.Descendants<HeadingBlock>()
                .ToArray()
                .Select(hb => hb.Inline.FirstChild.ToString())
                .First();

            var content = ParseContent(document);

            var linkedContent = ParseLinkedContents(document);
            var folder = Path.GetDirectoryName(path);

            string acceptedDetailIndex = string.Empty;
            foreach (var linked in linkedContent)
            {
                if (linked.Source == Sources.Markdown)
                {
                    linked.Link = Path.Combine(folder, linked.Link + ".md");
                }
                else if (linked.Source == Sources.Raw)
                {
                    linked.Link = Path.Combine(folder, linked.Link);
                }
                acceptedDetailIndex += linked.BulletItem;
            }

            Page page = new Page()
            {
                Source = Sources.Markdown,
                Link = path,
                Title = heading,
                Content = content,
                LinkedContentsType = linkedContent,
                AcceptedDetailIndex = acceptedDetailIndex,
            };

            return page;
        }

        /// <summary>
        /// Parse content in order to convert it to meta-file
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private static string ParseContent(MarkdownDocument document)
        {
            var output = new StringBuilder();

            int bulletIndex = 0;

            var allElements = document.Descendants().ToArray();

            for (var index = 0; index < allElements.Count(); index++)
            {
                var item = allElements[index];
                if (item is HeadingBlock)
                {
                    var heading = item as HeadingBlock;

                    if (heading.Parent is Block && !(heading.Parent is MarkdownDocument))
                    {
                        continue;
                    }

                    output.Append("\r\n");

                    var enumerator = heading.Inline.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current is HtmlInline)
                        {
                            output.Append(((HtmlInline)(enumerator.Current)).Tag);
                        }
                        else if (enumerator.Current is LiteralInline)
                        {
                            output.Append(((LiteralInline)enumerator.Current).Content.ToString());
                        }
                    }
                    output.AppendLine();
                }
                else if (item is ListBlock)
                {
                    var listBlock = item as ListBlock;
                    for (var i = 0; i < listBlock.Count(); i++)
                    {
                        var block = (ListItemBlock)listBlock[i];

                        string text = string.Empty;
                        if (block[0] is ParagraphBlock)
                        {
                            text = ((ParagraphBlock)block[0]).Inline.FirstChild.ToString();
                            var inline = ((ParagraphBlock)block[0]).Inline.FirstChild;
                            if (inline is LinkInline)
                            {
                                var ininline = (LinkInline)inline;

                                if (ininline.Url.StartsWith("http://") || ininline.Url.StartsWith("https://"))
                                {
                                    continue;
                                }

                                var label = ininline.ToMarkdownString();
                                var title = ininline.Title;

                                var bulletNumber = bulletIndex + (bulletIndex < 9 ? 48 : 55);

                                text = (char)(bulletNumber + 1) + ". " + (string.IsNullOrWhiteSpace(title) ? label : title);

                                output.AppendLine(text);

                                bulletIndex++;
                            }
                        }
                        else if (block[0] is HtmlBlock)
                        {
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(((HtmlBlock)block[0]).Lines.ToString());

                            var bulletNumber = bulletIndex + (bulletIndex < 9 ? 48 : 55);
                            output.Append((char)(bulletNumber + 1) + ". ");

                            foreach (var attribute in doc.DocumentNode.FirstChild.Attributes)
                            {
                                if (attribute.Name == "title")
                                {
                                    output.Append(attribute.Value);
                                }
                            }

                            output.AppendLine();

                            bulletIndex++;
                        }
                    }
                }
                else if (item is HtmlBlock)
                {
                    var html = item as HtmlBlock;

                    if (html.Parent is Block && !(html.Parent is MarkdownDocument))
                    {
                        continue;
                    }

                    for (int i = 0; i < html.Lines.Lines.Count(); i++)
                    {
                        output.Append(html.Lines.Lines[i].ToString());
                    }
                    output.AppendLine();
                }
                else if (item is ParagraphBlock)
                {
                    var paragraph = item as ParagraphBlock;
                    if (paragraph.Parent is Block && !(paragraph.Parent is MarkdownDocument))
                    {
                        continue;
                    }

                    var enumerator = paragraph.Inline.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current is HtmlInline)
                        {
                            output.Append(((HtmlInline)(enumerator.Current)).Tag);
                        }
                        else if (enumerator.Current is LiteralInline)
                        {
                            output.Append(((LiteralInline)enumerator.Current).Content.ToString());
                        }
                    }
                    output.AppendLine();
                }
            }

            return output.ToString();
        }

        private static List<ContentsType> ParseLinkedContents(MarkdownDocument document)
        {
            List<ContentsType> linkedContentsType = new List<ContentsType>();

            var allLinks = document.Descendants<ListBlock>().ToArray();

            var bulletIndex = 1;
            foreach (var list in allLinks)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var block = (ListItemBlock)list[i];

                    MarkdownItemDto item = FillItem(block[0]);

                    if (item.Link.StartsWith("http://") || item.Link.StartsWith("https://") || string.IsNullOrWhiteSpace(item.Link))
                    {
                        continue;
                    }

                    var bulletNumber = bulletIndex + (bulletIndex < 10 ? 48 : 55);

                    linkedContentsType.Add(new ContentsType() { Link = item.Link, BulletItem = (char)bulletNumber, Source = item.Type });

                    bulletIndex++;
                }
            }

            return linkedContentsType;
        }

        public static MarkdownItemDto FillItem(Block item)
        {
            var output = new MarkdownItemDto();

            string text = string.Empty;
            if (item is ParagraphBlock)
            {
                text = ((ParagraphBlock)item).Inline.FirstChild.ToString();
                var inline = ((ParagraphBlock)item).Inline.FirstChild;
                if (inline is LinkInline)
                {
                    var ininline = (LinkInline)inline;
                    var label = ininline.ToMarkdownString();
                    var title = ininline.Title;

                    output.Title = string.IsNullOrWhiteSpace(title) ? label : title;
                    output.Link = ininline.Url;

                    switch (Path.GetExtension(output.Link))
                    {
                        case ".md":
                            output.Type = Sources.Markdown;
                            break;
                        case ".raw":
                            output.Type = Sources.Raw;
                            break;
                    }
                }

            }
            else if (item is HtmlBlock)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(((HtmlBlock)item).Lines.ToString());

                foreach (var attribute in doc.DocumentNode.FirstChild.Attributes)
                {
                    if (attribute.Name == "title")
                    {
                        output.Title = attribute.Value;
                    }
                    else if (attribute.Name == "url")
                    {
                        output.Link = attribute.Value;
                    }
                }

                output.Type = Sources.Rss;
            }

            return output;
        }
    }
}
