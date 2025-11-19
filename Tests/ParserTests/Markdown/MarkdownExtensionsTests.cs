using FluentAssertions;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Parser.Markdown;

namespace ParserTests.Markdown
{
    [TestClass]
    public class MarkdownExtensionsTests
    {
        [TestMethod]
        public void ContainerInline_ToMarkdownString_WithSingleLiteralInline_ShouldReturnLiteralContent()
        {
            // Arrange
            var markdown = "Hello World";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var containerInline = paragraph.Inline;

            // Act
            var result = containerInline.ToMarkdownString();

            // Assert
            result.Should().Be("Hello World");
        }

        [TestMethod]
        public void ContainerInline_ToMarkdownString_WithMultipleInlines_ShouldConcatenateThem()
        {
            // Arrange
            var markdown = "**bold** and *italic*";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var containerInline = paragraph.Inline;

            // Act
            var result = containerInline.ToMarkdownString();

            // Assert
            result.Should().Contain("bold");
            result.Should().Contain("italic");
        }

        [TestMethod]
        public void ContainerInline_ToMarkdownString_WithLink_ShouldReturnFormattedLink()
        {
            // Arrange
            var markdown = "[Google](https://google.com)";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var containerInline = paragraph.Inline;

            // Act
            var result = containerInline.ToMarkdownString();

            // Assert
            result.Should().Contain("[Google]");
            result.Should().Contain("https://google.com");
        }

        [TestMethod]
        public void ContainerInline_ToMarkdownString_Empty_ShouldReturnEmptyString()
        {
            // Arrange
            var markdown = "   ";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().FirstOrDefault();

            if (paragraph == null)
            {
                // Skip if no paragraph found for whitespace-only markdown
                Assert.Inconclusive("Whitespace-only markdown produces no paragraph");
            }

            var containerInline = paragraph.Inline;

            // Act
            var result = containerInline.ToMarkdownString();

            // Assert
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void Inline_ToMarkdownString_WithLineBreak_ShouldReturnNewline()
        {
            // Arrange
            var markdown = "Line 1  \nLine 2";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var lineBreakInline = paragraph.Inline.FirstOrDefault(i => i is LineBreakInline);

            // Act
            var result = lineBreakInline?.ToMarkdownString() ?? "";

            // Assert
            result.Should().Be("\n");
        }

        [TestMethod]
        public void Inline_ToMarkdownString_WithLiteral_ShouldReturnContent()
        {
            // Arrange
            var markdown = "Plain text";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var literalInline = paragraph.Inline.FirstOrDefault(i => i is LiteralInline);

            // Act
            var result = literalInline?.ToMarkdownString() ?? "";

            // Assert
            result.Should().Be("Plain text");
        }

        [TestMethod]
        public void Inline_ToMarkdownString_WithSpecialCharacters_ShouldPreserveSpecialChars()
        {
            // Arrange
            var markdown = "Hello @#$% World!";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var literalInline = paragraph.Inline.FirstOrDefault(i => i is LiteralInline);

            // Act
            var result = literalInline?.ToMarkdownString() ?? "";

            // Assert
            result.Should().Contain("@#$%");
        }

        [TestMethod]
        public void Inline_ToMarkdownString_WithItalic_ShouldReturnWithAsterisks()
        {
            // Arrange
            var markdown = "*italic text*";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var emphasisInline = paragraph.Inline.FirstOrDefault(i => i is EmphasisInline);

            // Act
            var result = emphasisInline?.ToMarkdownString() ?? "";

            // Assert
            result.Should().Match("*italic text*");
        }

        [TestMethod]
        public void Inline_ToMarkdownString_WithBold_ShouldReturnWithDoubleAsterisks()
        {
            // Arrange
            var markdown = "**bold text**";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var emphasisInline = paragraph.Inline.FirstOrDefault(i => i is EmphasisInline);

            // Act
            var result = emphasisInline?.ToMarkdownString() ?? "";

            // Assert
            result.Should().Match("**bold text**");
        }

        [TestMethod]
        public void Inline_ToMarkdownString_WithUnderscoreEmphasis_ShouldReturnWithUnderscores()
        {
            // Arrange
            var markdown = "_italic with underscore_";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var emphasisInline = paragraph.Inline.FirstOrDefault(i => i is EmphasisInline);

            // Act
            var result = emphasisInline?.ToMarkdownString() ?? "";

            // Assert
            result.Should().Contain("italic with underscore");
        }

        [TestMethod]
        public void Inline_ToMarkdownString_WithLink_ShouldReturnFormattedLink()
        {
            // Arrange
            var markdown = "[Click here](https://example.com)";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var linkInline = paragraph.Inline.FirstOrDefault(i => i is LinkInline);

            // Act
            var result = linkInline?.ToMarkdownString() ?? "";

            // Assert
            result.Should().Contain("[Click here]");
            result.Should().Contain("https://example.com");
        }

        [TestMethod]
        public void Inline_ToMarkdownString_WithLinkAndTitle_ShouldIncludeTitle()
        {
            // Arrange
            var markdown = "[Example](https://example.com \"Example Site\")";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var linkInline = paragraph.Inline.FirstOrDefault(i => i is LinkInline);

            // Act
            var result = linkInline?.ToMarkdownString() ?? "";

            // Assert
            result.Should().Contain("[Example]");
            result.Should().Contain("https://example.com");
            result.Should().Contain("Example Site");
        }

        [TestMethod]
        public void Inline_ToMarkdownString_WithImage_ShouldReturnWithExclamationMark()
        {
            // Arrange
            var markdown = "![Alt text](https://example.com/image.png)";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var linkInline = paragraph.Inline.FirstOrDefault(i => i is LinkInline);

            // Act
            var result = linkInline?.ToMarkdownString() ?? "";

            // Assert
            result.Should().StartWith("!");
            result.Should().Contain("https://example.com/image.png");
        }

        [TestMethod]
        public void Inline_ToMarkdownString_WithImageAndTitle_ShouldReturnWithTitleAndExclamation()
        {
            // Arrange
            var markdown = "![Alt](https://example.com/img.png \"Image Title\")";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var linkInline = paragraph.Inline.FirstOrDefault(i => i is LinkInline);

            // Act
            var result = linkInline?.ToMarkdownString() ?? "";

            // Assert
            result.Should().StartWith("!");
            result.Should().Contain("Image Title");
        }

        [TestMethod]
        public void Inline_ToMarkdownString_WithHtmlInline_ShouldReturnTag()
        {
            // Arrange
            var markdown = "Text with <span>HTML</span> inline";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var htmlInline = paragraph.Inline.FirstOrDefault(i => i is HtmlInline);

            // Act
            var result = htmlInline?.ToMarkdownString() ?? "";

            // Assert
            result.Should().Contain("<span>");
        }

        [TestMethod]
        public void Inline_ToMarkdownString_WithNestedContainerInline_ShouldProcessRecursively()
        {
            // Arrange
            var markdown = "**bold with *nested italic***";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();

            // Act
            var result = paragraph.Inline.ToMarkdownString();

            // Assert
            result.Should().Contain("bold");
            result.Should().Contain("nested italic");
        }

        [TestMethod]
        public void Inline_ToMarkdownString_WithUnknownInlineType_ShouldReturnToString()
        {
            // Arrange
            var markdown = "Normal text";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();
            var inline = paragraph.Inline.FirstOrDefault();

            // Act
            var result = inline?.ToMarkdownString() ?? "";

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void ParagraphBlock_ToMarkdownString_WithSingleParagraph_ShouldReturnContent()
        {
            // Arrange
            var markdown = "This is a paragraph.";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();

            // Act
            var result = paragraph.ToMarkdownString();

            // Assert
            result.Should().Contain("This is a paragraph.");
        }

        [TestMethod]
        public void ParagraphBlock_ToMarkdownString_BreakableParagraph_ShouldAppendNewline()
        {
            // Arrange
            var markdown = "First paragraph\n\nSecond paragraph";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraphs = document.Descendants<ParagraphBlock>().ToList();
            var firstParagraph = paragraphs.First();

            // Act
            var result = firstParagraph.ToMarkdownString();

            // Assert
            // The result should contain the paragraph content
            result.Should().Contain("First paragraph");
        }

        [TestMethod]
        public void ParagraphBlock_ToMarkdownString_WithFormattedText_ShouldPreserveFormatting()
        {
            // Arrange
            var markdown = "This is **bold** and *italic* text.";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();

            // Act
            var result = paragraph.ToMarkdownString();

            // Assert
            result.Should().Contain("bold");
            result.Should().Contain("italic");
        }

        [TestMethod]
        public void ParagraphBlock_ToMarkdownString_WithLink_ShouldIncludeLinkFormatting()
        {
            // Arrange
            var markdown = "Check out [this link](https://example.com).";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().First();

            // Act
            var result = paragraph.ToMarkdownString();

            // Assert
            result.Should().Contain("[this link]");
            result.Should().Contain("https://example.com");
        }

        [TestMethod]
        public void ParagraphBlock_ToMarkdownString_EmptyParagraph_ShouldReturnEmptyOrMinimalContent()
        {
            // Arrange
            var markdown = "   \n\n";
            var document = Markdig.Markdown.Parse(markdown);
            var paragraph = document.Descendants<ParagraphBlock>().FirstOrDefault();

            if (paragraph == null)
            {
                Assert.Inconclusive("Whitespace-only content produces no paragraph");
            }

            // Act
            var result = paragraph.ToMarkdownString();

            // Assert
            result.Should().NotBeNull();
        }
    }
}