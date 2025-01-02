using Markdig.Renderers.Normalize;
using Markdig.Syntax.Inlines;

namespace Parser.Markdown
{
    public class LinkInlineRendererEx : NormalizeObjectRenderer<LinkInline>
    {
        protected override void Write(NormalizeRenderer renderer, LinkInline link)
        {
            if (link.IsImage)
            {
                renderer.Write('!');
            }
            renderer.Write('[');
            renderer.WriteChildren(link);
            renderer.Write(']');

            if (!string.IsNullOrEmpty(link.Url))
            {
                renderer.Write('(').Write(link.Url);

                if (!string.IsNullOrEmpty(link.Title))
                {
                    renderer.Write(" \"");
                    renderer.Write(link.Title.Replace(@"""", @"\"""));
                    renderer.Write("\"");
                }

                renderer.Write(')');
            }
        }
    }
}
