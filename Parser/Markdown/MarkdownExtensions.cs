using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using Markdig;
using Markdig.Renderers.Html.Inlines;

namespace Parser.Markdown
{
    public static class MarkdownExtensions
    {
        public static string ToMarkdownString(this Block block)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .UsePipeTables()
                .Build();

            using (var writer = new StringWriter())
            {
                var renderer = new NormalizeRenderer(writer);
                renderer.ObjectRenderers.Remove(renderer.ObjectRenderers.FirstOrDefault(i => i is LinkInlineRenderer));
                renderer.ObjectRenderers.Add(new LinkInlineRendererEx());
                pipeline.Setup(renderer);

                renderer.Render(block);
                return writer.ToString();
            }
        }

        public static string ToMarkdownString(this Markdig.Syntax.Inlines.ContainerInline inlines)
        {
            var str = string.Empty;
            foreach (var inline in inlines)
            {
                str += inline.ToMarkdownString();
            }
            return str;
        }

        public static string ToMarkdownString(this Markdig.Syntax.Inlines.Inline inline)
        {
            string add = string.Empty;
            if (inline is Markdig.Syntax.Inlines.LineBreakInline)
            {
                add = "\n";
            }
            else if (inline is Markdig.Syntax.Inlines.LiteralInline)
            {
                var literalInline = inline as Markdig.Syntax.Inlines.LiteralInline;
                add = literalInline.Content.ToString();
            }
            else if (inline is Markdig.Syntax.Inlines.EmphasisInline)
            {
                var emphasisInline = inline as Markdig.Syntax.Inlines.EmphasisInline;
                var delimiterChar = emphasisInline.DelimiterChar.ToString();
                if (emphasisInline.DelimiterCount == 2)
                {
                    delimiterChar += delimiterChar;
                }
                add = delimiterChar + emphasisInline.ToMarkdownString() + delimiterChar;
            }
            else if (inline is Markdig.Syntax.Inlines.LinkInline)
            {
                var linkInline = inline as Markdig.Syntax.Inlines.LinkInline;
                add = string.Empty;
                if (linkInline.IsImage)
                {
                    add = "!";
                }
                var label = linkInline.ToMarkdownString();
                var url = linkInline.Url;
                var title = linkInline.Title;
                if (!string.IsNullOrEmpty(title))
                {
                    add += $"[{label}]({url} \"{title}\")";
                }
                else
                {
                    add += $"[{label}]({url})";
                }
            }
            else if (inline is Markdig.Syntax.Inlines.ContainerInline)
            {
                var containerInline = inline as Markdig.Syntax.Inlines.ContainerInline;
                add = containerInline.ToMarkdownString();
            }
            else if (inline is Markdig.Syntax.Inlines.HtmlInline)
            {
                var htmlInline = inline as Markdig.Syntax.Inlines.HtmlInline;
                add = htmlInline.Tag;
            }
            else
            {
                add = inline.ToString();
            }
            return add;
        }

        public static string ToMarkdownString(this ParagraphBlock paragraphBlock)
        {
            var str = string.Empty;
            str += paragraphBlock.Inline.ToMarkdownString();
            if (paragraphBlock.IsBreakable)
            {
                str += "\n";
            }
            return str;
        }

        public static string ToMarkdownString(this Markdig.Extensions.Tables.Table tableBlock)
        {
            var ret = string.Empty;
            foreach (Markdig.Extensions.Tables.TableRow row in tableBlock)
            {
                var line = "|";
                foreach (Markdig.Extensions.Tables.TableCell cell in row)
                {
                    foreach (ParagraphBlock block in cell)
                    {
                        line += block.ToMarkdownString().Replace("\n", "");
                    }
                    line += "|";
                }
                if (row.IsHeader)
                {
                    line += "\n|";
                    for (int i = 0; i < row.Count; i++)
                    {
                        line += "---|";
                    }
                }
                ret += line + "\n";
            }
            return ret;
        }

        public static Dictionary<string, List<string>> ToTable(this Markdig.Extensions.Tables.Table tableBlock)
        {
            var table = new Dictionary<string, List<string>>();
            var indexes = new Dictionary<int, string>();
            foreach (var blockrow in tableBlock)
            {
                var row = blockrow as Markdig.Extensions.Tables.TableRow;
                int indexCol = 0;
                foreach (var blockcell in row)
                {
                    var cell = blockcell as Markdig.Extensions.Tables.TableCell;
                    foreach (var blockpar in cell)
                    {
                        var par = blockpar as ParagraphBlock;
                        var name = par.ToMarkdownString().Trim();
                        if (row.IsHeader)
                        {
                            indexes[indexCol] = name;
                            table[name] = new List<string>();
                        }
                        else
                        {
                            table[indexes[indexCol]].Add(name);
                        }
                    }
                    indexCol++;
                }
            }
            return table;
        }
    }
}
