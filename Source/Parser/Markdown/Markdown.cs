using Common.Enum;
using HtmlAgilityPack;
using Markdig.Syntax.Inlines;
using Markdig.Syntax;
using Parser.Markdown.Dto;
using Common.Dto;
using System.Text;

namespace Parser.Markdown
{
    /// <summary>
    /// Markdown parser
    /// </summary>
    public static class Markdown
    {
        /// <summary>
        /// Parse all markdown files in the given path
        /// </summary>
        /// <param name="path">Path to parse</param>
        /// <returns>List of <seealso cref="Page"/></returns>
        public static List<Page> ParseAllFiles(string path)
        {
            List<Page> pageParsed = new List<Page>();

            foreach (var file in Directory.GetFiles(path, "*.md"))
            {
                pageParsed.Add(ParseFile(file));
            }

            return pageParsed;
        }

        /// <summary>
        /// Parse a single markdown file
        /// </summary>
        /// <param name="path">Path of file to parse</param>
        /// <returns>File parsed in <seealso cref="Page"/></returns>
        public static Page ParseFile(string path)
        {
            if (!File.Exists(path))
            {
                path += ".md";
            }

            var markdown = File.ReadAllText(path);
            var document = Markdig.Markdown.Parse(markdown);

            // Get the heading of the document
            var heading = document.Descendants<HeadingBlock>()
                .ToArray()
                .Select(hb => hb.Inline.FirstChild.ToString())
                .First();

            // Parse file content
            var content = ParseContent(document);

            // Parse linked contents
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
                            else if (inline is LiteralInline)
                            {
                                var ininline = (LiteralInline)inline;

                                var label = ininline.ToMarkdownString();
                                output.Append("* ");
                                output.AppendLine(label);
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

                if (doc.DocumentNode.FirstChild.Name == "dynamic")
                {
                    output.Type = Sources.Dynamic;
                }
                else
                {
                    output.Type = Sources.Rss;
                }

                foreach (var attribute in doc.DocumentNode.FirstChild.Attributes)
                {
                    if (attribute.Name == "title")
                    {
                        output.Title = attribute.Value;
                    }
                    else if (attribute.Name == "url" || attribute.Name == "pluginname")
                    {
                        output.Link = attribute.Value;
                    }
                }

            }

            return output;
        }
    }
}
