using FluentAssertions;
using Common.Utils;

namespace CommonTests.Utils
{
    [TestClass]
    public class StringUtilsTests
    {
        [TestMethod]
        public void SplitToLines_ShouldSplitStringIntoLinesWithoutBreakingWords()
        {
            // Arrange
            string input = "This is a test string that should be split into multiple lines without breaking words.";
            int maxLineLength = 20;

            // Act
            IEnumerable<string> result = StringUtils.SplitToLines(input, maxLineLength);

            // Assert
            result.Should().BeEquivalentTo(new List<string>
            {
                "This is a test",
                "string that should",
                "be split into",
                "multiple lines",
                "without breaking",
                "words."
            });
        }

        [TestMethod]
        public void SplitToLines_ShouldHandleEmptyString()
        {
            // Arrange
            string input = "";
            int maxLineLength = 20;

            // Act
            IEnumerable<string> result = StringUtils.SplitToLines(input, maxLineLength);

            // Assert
            result.Should().Contain(string.Empty);
        }

        [TestMethod]
        public void SplitToLines_ShouldHandleSingleWordLongerThanMaxLineLength()
        {
            // Arrange
            string input = "Supercalifragilisticexpialidocious";
            int maxLineLength = 10;

            // Act
            IEnumerable<string> result = StringUtils.SplitToLines(input, maxLineLength);

            // Assert
            result.Should().BeEquivalentTo(new List<string>
            {
                "Supercalifragilisticexpialidocious"
            });
        }

        [TestMethod]
        public void SplitToLines_ShouldHandleMultipleLinesWithTags()
        {
            // Arrange
            string input = "This is a <red>test</red> string that should be split into multiple lines without breaking words.";
            int maxLineLength = 20;

            // Act
            IEnumerable<string> result = StringUtils.SplitToLines(input, maxLineLength);

            // Assert
            result.Should().BeEquivalentTo(new List<string>
            {
                "This is a <red>test</red>",
                "string that should",
                "be split into",
                "multiple lines",
                "without breaking",
                "words."
            });
        }

        [TestMethod]
        public void CreateBulletNumber_ShouldReturnFormattedBulletNumber()
        {
            // Arrange
            int bulletNumber = 65; // ASCII for 'A'

            // Act
            string result = StringUtils.CreateBulletNumber(bulletNumber);

            // Assert
            result.Should().Be("<white><revon> A <revoff><lightgrey>");
        }

        [TestMethod]
        public void CreateBulletNumber_ShouldHandleSingleDigitBulletNumber()
        {
            // Arrange
            int bulletNumber = 49; // ASCII for '1'

            // Act
            string result = StringUtils.CreateBulletNumber(bulletNumber);

            // Assert
            result.Should().Be("<white><revon> 1 <revoff><lightgrey>");
        }

        [TestMethod]
        public void CreateBulletNumber_ShouldHandleSpecialCharacterBulletNumber()
        {
            // Arrange
            int bulletNumber = 42; // ASCII for '*'

            // Act
            string result = StringUtils.CreateBulletNumber(bulletNumber);

            // Assert
            result.Should().Be("<white><revon> * <revoff><lightgrey>");
        }

        [TestMethod]
        public void CreateBulletNumber_ShouldHandleLowerCaseLetterBulletNumber()
        {
            // Arrange
            int bulletNumber = 97; // ASCII for 'a'

            // Act
            string result = StringUtils.CreateBulletNumber(bulletNumber);

            // Assert
            result.Should().Be("<white><revon> a <revoff><lightgrey>");
        }
    }
}
