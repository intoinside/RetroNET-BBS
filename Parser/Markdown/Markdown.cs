using Common.Dto;
using Common.Enum;
using HtmlAgilityPack;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.Extensions.Configuration;
using Parser.Markdown.Dto;
using System;

namespace Parser.Markdown
{
    public static class Markdown// : IParser
    {
        //private IConfiguration config;

        //private string path;

        //List<string> fileToParse = new List<string>();

        ////private Pages index;

        //private MarkdownDto md;
        //public string output;

        //public Markdown()
        //{
        //    var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
        //    config = builder.Build();

        //    path = config["Path"];

        //    Parse(path);
        //}

        //public async void Parse(string folder)
        //{
        //    var fileList = Directory.GetFiles(folder, "*.md");

        //    var homePath = Path.Combine(folder, "index.md");
        //    md = await ParseFile(homePath);
        //    //index.Title = "Index";

        //    foreach (var file in fileToParse)
        //    {

        //    }
        //    //{
        //    //    if (file == homePath)
        //    //    {
        //    //        continue;
        //    //    }

        //    //    ParseFile(file);
        //    //}
        //}

        //public MarkdownDto GetMd()
        //{
        //    return md;
        //}

        //public string Title()
        //{
        //    return index.Title;
        //}

        //public Pages Home()
        //{
        //    return index;
        //}

        //private async Task<MarkdownDto> ParseFile(string path)
        public static async Task<MarkdownDto> ParseFile(string path)
        {
            var markdown = File.ReadAllText(path);
            var document = Markdig.Markdown.Parse(markdown);

            MarkdownDto entries = new MarkdownDto();

            var heading = document.Descendants<HeadingBlock>()
                .ToArray()
                .Select(hb => hb.Inline.FirstChild.ToString())
                .First();

            entries.Title = heading;
            entries.Articles = new List<MarkdownItemDto>();

            var allLinks = document.Descendants<ListBlock>().ToArray();

            foreach (var list in allLinks)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    MarkdownItemDto item = new MarkdownItemDto();

                    var block = (ListItemBlock)list[i];

                    string text = string.Empty;
                    if (block[0] is ParagraphBlock)
                    {
                        text = ((ParagraphBlock)block[0]).Inline.FirstChild.ToString();
                        var inline = ((ParagraphBlock)block[0]).Inline.FirstChild;
                        if (inline is LinkInline)
                        {
                            var ininline = (LinkInline)inline;
                            var label = ininline.ToMarkdownString();
                            var url = ininline.Url;
                            var title = ininline.Title;

                            //text = "<markdown title=\"" + (string.IsNullOrWhiteSpace(title) ? label : title) + "\" url=\"" + url + "\">";

                            item.Title = string.IsNullOrWhiteSpace(title) ? label : title;
                            item.Link = url;
                            item.Type = Sources.Markdown;
                        }

                    }
                    else if (block[0] is HtmlBlock)
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(((HtmlBlock)block[0]).Lines.ToString());

                        foreach (var attribute in doc.DocumentNode.FirstChild.Attributes)
                        {
                            if (attribute.Name == "title")
                            {
                                item.Title = attribute.Value;
                            }
                            else if (attribute.Name == "url")
                            {
                                item.Link = attribute.Value;
                            }
                        }

                        item.Type = Sources.Rss;
                    }

                    var bulletNumber = i + (i < 9 ? 48 : 55);

                    //output += "<revon><white> " + (char)(bulletNumber + 1) + " <revoff><lightgrey>";

                    //output += " " + text + "\r\n";

                    //fileToParse.Add(text.Replace(' ', '-'));

                    entries.Articles.Add(item);
                }
            }

            //Pages page = new Pages()
            //{
            //    Source = Sources.Markdown,
            //    Content = output,
            //};

            return entries;
        }
    }
}
