using Markdig;
using Markdig.Syntax;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Parser
{
    public class Markdown
    {
        private IConfiguration config;

        private string path;

        List<string> fileToParse = new List<string>();

        public string home;
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

            var homePath = Path.Combine(folder, "home.md");
            home = ParseFile(homePath);

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

        public string Home()
        {
            return home;
        }

        private string ParseFile(string path)
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
                    var text = ((ParagraphBlock)((ListItemBlock)list[i])[0]).Inline.FirstChild.ToString();

                    output += text + "\r\n";

                    fileToParse.Add(text.Replace(' ', '_'));
                }
            }

            return output;
        }
    }
}
