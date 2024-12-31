﻿using Common.Enum;
using HtmlAgilityPack;
using Markdig.Syntax.Inlines;
using Markdig.Syntax;
using Parser.Markdown.Dto;
using Common.Dto;
using System.Text;
using System.Security.Cryptography;
using Encoder;
using Common.Utils;

namespace Parser.Markdown
{
    public static class MarkdownStatic
    {
        public static List<Page> ParseAllFiles(string path)
        {
            List<Page> pageParsed = new List<Page>();
            
            Directory.GetFiles(path, "*.md", SearchOption.TopDirectoryOnly)
                .ToList()
                .ForEach(file =>
                {
                    pageParsed.Add(ParseFile(file));
                });

            return pageParsed;
        }


        public static Page ParseFile(string path)
        {
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
                    linked.Path = Path.Combine(folder, linked.Path + ".md");
                }
                acceptedDetailIndex += linked.BulletItem;
            }

            Page page = new Page()
            {
                Hash = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(path))),
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

                    output.AppendLine(heading.Inline.FirstChild.ToString());
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
                else if (item is ParagraphBlock)
                {
                    var paragraph = item as ParagraphBlock;
                    if (paragraph.Parent is MarkdownDocument)
                    {
                        output.AppendLine(paragraph.Inline.FirstChild.ToString());
                    }
                }
            }

            return output.ToString();
        }

        private static List<ContentsType> ParseLinkedContents(MarkdownDocument document)
        {
            List<ContentsType> linkedContentsType = new List<ContentsType>();

            var allLinks = document.Descendants<ListBlock>().ToArray();

            foreach (var list in allLinks)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var block = (ListItemBlock)list[i];

                    MarkdownItemDto item = FillItem(block[0]);

                    var bulletNumber = i + 1 + (i < 9 ? 48 : 55);

                    linkedContentsType.Add(new ContentsType() { Path = item.Link, BulletItem = (char)bulletNumber, Source = item.Type });
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

                    //text = "<markdown title=\"" + (string.IsNullOrWhiteSpace(title) ? label : title) + "\" url=\"" + url + "\">";

                    output.Title = string.IsNullOrWhiteSpace(title) ? label : title;
                    output.Link = ininline.Url;
                    output.Type = Sources.Markdown;
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

        public static string GetHome(string document, IEncoder encoder)
        {
            var content = new StringBuilder();

            // Upper offset
            content.AppendLine("<lightgray><crsrdown><crsrdown><crsrdown><crsrdown>");

            foreach (var line in StringUtils.SplitToLines(document, encoder.NumberOfColumns() - 1))
            {
                content.AppendLine(line);
            }

            return content.ToString();
        }
    }
}
