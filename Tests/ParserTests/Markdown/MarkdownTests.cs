using FluentAssertions;
using Common.Enum;

namespace ParserTests.Markdown
{
    [TestClass]
    public class MarkdownTests
    {
        private string _testDirectory;
        private string _testFilePath;

        [TestInitialize]
        public void Setup()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "MarkdownTests_" + Guid.NewGuid());
            Directory.CreateDirectory(_testDirectory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [TestMethod]
        public void ParseAllFiles_WithValidMarkdownFiles_ShouldReturnListOfPages()
        {
            // Arrange
            var markdown1 = "# Heading 1\nSome content";
            var markdown2 = "# Heading 2\nOther content";
            
            File.WriteAllText(Path.Combine(_testDirectory, "file1.md"), markdown1);
            File.WriteAllText(Path.Combine(_testDirectory, "file2.md"), markdown2);

            // Act
            var result = Parser.Markdown.Markdown.ParseAllFiles(_testDirectory);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(page => page.Should().NotBeNull());
        }

        [TestMethod]
        public void ParseAllFiles_WithEmptyDirectory_ShouldReturnEmptyList()
        {
            // Arrange
            // _testDirectory is empty

            // Act
            var result = Parser.Markdown.Markdown.ParseAllFiles(_testDirectory);

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void ParseAllFiles_WithMultipleMarkdownFiles_ShouldParseAllFiles()
        {
            // Arrange
            var filenames = new[] { "index.md", "about.md", "contact.md" };
            foreach (var filename in filenames)
            {
                File.WriteAllText(Path.Combine(_testDirectory, filename), $"# {filename}\nContent");
            }

            // Act
            var result = Parser.Markdown.Markdown.ParseAllFiles(_testDirectory);

            // Assert
            result.Should().HaveCount(3);
            result.Should().AllSatisfy(page => page.Title.Should().NotBeNullOrEmpty());
        }

        [TestMethod]
        public void ParseAllFiles_WithNonMarkdownFiles_ShouldIgnoreThem()
        {
            // Arrange
            File.WriteAllText(Path.Combine(_testDirectory, "file.md"), "# Title\nContent");
            File.WriteAllText(Path.Combine(_testDirectory, "file.txt"), "Not markdown");
            File.WriteAllText(Path.Combine(_testDirectory, "file.json"), "{}");

            // Act
            var result = Parser.Markdown.Markdown.ParseAllFiles(_testDirectory);

            // Assert
            result.Should().HaveCount(1);
            result[0].Source.Should().Be(Sources.Markdown);
        }

        [TestMethod]
        public void ParseFile_WithValidFile_ShouldReturnPage()
        {
            // Arrange
            var markdown = "# My Title\nThis is content";
            _testFilePath = Path.Combine(_testDirectory, "test.md");
            File.WriteAllText(_testFilePath, markdown);

            // Act
            var result = Parser.Markdown.Markdown.ParseFile(_testFilePath);

            // Assert
            result.Should().NotBeNull();
            result.Source.Should().Be(Sources.Markdown);
            result.Title.Should().Be("My Title");
        }

        [TestMethod]
        public void ParseFile_WithFileWithoutExtension_ShouldAppendMdExtension()
        {
            // Arrange
            var markdown = "# Test\nContent here";
            _testFilePath = Path.Combine(_testDirectory, "test");
            File.WriteAllText(_testFilePath + ".md", markdown);

            // Act
            var result = Parser.Markdown.Markdown.ParseFile(_testFilePath);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Test");
        }

        [TestMethod]
        public void ParseFile_WithPathSeparators_ShouldNormalizePathToUnderscores()
        {
            // Arrange
            var markdown = "# Title\nContent";
            _testFilePath = Path.Combine(_testDirectory, "test.md");
            File.WriteAllText(_testFilePath, markdown);

            // Act
            var result = Parser.Markdown.Markdown.ParseFile(_testFilePath);

            // Assert
            result.Link.Should().NotContain("\\");
            result.Link.Should().NotContain("/");
            result.Link.Should().Contain("_");
        }

        [TestMethod]
        public void ParseFile_WithSimpleContent_ShouldParseContent()
        {
            // Arrange
            var markdown = "# Heading\nThis is a paragraph.";
            _testFilePath = Path.Combine(_testDirectory, "simple.md");
            File.WriteAllText(_testFilePath, markdown);

            // Act
            var result = Parser.Markdown.Markdown.ParseFile(_testFilePath);

            // Assert
            result.Content.Should().NotBeNullOrEmpty();
            result.Content.Should().Contain("Heading");
        }

        [TestMethod]
        public void ParseFile_WithHeadingAndLinks_ShouldExtractTitleAndParseLinks()
        {
            // Arrange
            var markdown = @"# Main Title
Content here
- [Link1](page1.md)
- [Link2](page2.md)";
            _testFilePath = Path.Combine(_testDirectory, "withlinks.md");
            File.WriteAllText(_testFilePath, markdown);

            // Act
            var result = Parser.Markdown.Markdown.ParseFile(_testFilePath);

            // Assert
            result.Title.Should().Be("Main Title");
            result.LinkedContentsType.Should().NotBeEmpty();
        }

        [TestMethod]
        public void ParseFile_WithAcceptedDetailIndex_ShouldBuildBulletIndexString()
        {
            // Arrange
            var markdown = @"# Title
- [Link1](link1.md)
- [Link2](link2.md)
- [Link3](link3.md)";
            _testFilePath = Path.Combine(_testDirectory, "bullets.md");
            File.WriteAllText(_testFilePath, markdown);

            // Act
            var result = Parser.Markdown.Markdown.ParseFile(_testFilePath);

            // Assert
            result.AcceptedDetailIndex.Should().NotBeNullOrEmpty();
            result.AcceptedDetailIndex.Should().HaveLength(3);
        }

        [TestMethod]
        public void ParseFile_WithMarkdownLinkedContent_ShouldSetSourceToMarkdown()
        {
            // Arrange
            var markdown = @"# Title
- [Link](other.md)";
            _testFilePath = Path.Combine(_testDirectory, "mdlink.md");
            File.WriteAllText(_testFilePath, markdown);

            // Act
            var result = Parser.Markdown.Markdown.ParseFile(_testFilePath);

            // Assert
            result.LinkedContentsType.Should().NotBeEmpty();
            result.LinkedContentsType[0].Source.Should().Be(Sources.Markdown);
        }

        [TestMethod]
        public void ParseFile_WithRawLinkedContent_ShouldSetSourceToRaw()
        {
            // Arrange
            var markdown = @"# Title
- [Link](file.raw)";
            _testFilePath = Path.Combine(_testDirectory, "rawlink.md");
            File.WriteAllText(_testFilePath, markdown);

            // Act
            var result = Parser.Markdown.Markdown.ParseFile(_testFilePath);

            // Assert
            result.LinkedContentsType.Should().NotBeEmpty();
            result.LinkedContentsType[0].Source.Should().Be(Sources.Raw);
        }
    }
}