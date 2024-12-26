﻿using Common.Dto;
using Common.Enum;
using Markdig.Syntax;
using Microsoft.Extensions.Configuration;

namespace Parser
{
    public class Markdown : IParser
    {
        private IConfiguration config;

        private string path;

        List<string> fileToParse = new List<string>();

        private Pages index;
        public string output;

        public Markdown()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
            config = builder.Build();

            path = config["Path"];

            Parse(path);
        }

        public void Parse(string folder)
        {
            var fileList = Directory.GetFiles(folder, "*.md");

            var homePath = Path.Combine(folder, "index.md");
            index = ParseFile(homePath);
            index.Title = "Index";

            foreach (var file in fileToParse)
            {

            }
            //{
            //    if (file == homePath)
            //    {
            //        continue;
            //    }

            //    ParseFile(file);
            //}
        }

        public string Title()
        {
            return index.Title;
        }

        public Pages Home()
        {
            return index;
        }

        private Pages ParseFile(string path)
        {
            var markdown = File.ReadAllText(path);
            var document = Markdig.Markdown.Parse(markdown);

            var heading = document.Descendants<HeadingBlock>()
                .ToArray()
                .Select(hb => hb.Inline.FirstChild.ToString())
                .First();

            var output = heading + "\r\n\r\n";

            var allLinks = document.Descendants<ListBlock>().ToArray();

            foreach (var list in allLinks)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var block = (ListItemBlock)list[i];

                    string text = string.Empty;
                    if (block[0] is ParagraphBlock)
                    {
                        text = ((ParagraphBlock)block[0]).Inline.FirstChild.ToString();
                    } else
                    {
                        text = ((Markdig.Syntax.Inlines.LinkInline)((Markdig.Syntax.Inlines.ContainerInline)((ParagraphBlock)block[0]).Inline.FirstChild)).Url;
                        
                    }

                    var bulletNumber = i + (i < 9 ? 48 : 55);

                    output += "<revon><white> " + (char)(bulletNumber + 1) + " <revoff><lightgrey>";
                        
                    output += " " + text + "\r\n";

                    fileToParse.Add(text.Replace(' ', '_'));
                }
            }

            Pages page = new Pages()
            {
                Source = Sources.Markdown,
                Content = output,
            };

            return page;
        }
    }
}
