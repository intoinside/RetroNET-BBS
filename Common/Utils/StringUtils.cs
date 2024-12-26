namespace Common.Utils
{
    public static class StringUtils
    {
        /// <summary>
        /// Split a string into lines of a maximum length without breaking words.
        /// </summary>
        /// <param name="stringToSplit">String to split</param>
        /// <param name="maximumLineLength">Maximum length of line</param>
        /// <returns>Splitted string</returns>
        public static IEnumerable<string> SplitToLines(string stringToSplit, int maximumLineLength)
        {
            var words = stringToSplit.Split(' ');
            var line = words.First();
            foreach (var word in words.Skip(1))
            {
                var test = $"{line} {word}";
                if (test.Length > maximumLineLength)
                {
                    yield return line;
                    line = word;
                }
                else
                {
                    line = test;
                }
            }

            yield return line;
        }

        /// <summary>
        /// Generate a standardized bullet number.
        /// </summary>
        /// <param name="bulletNumber">Bullet to add</param>
        /// <returns>Bullet number with decoration</returns>
        public static string CreateBulletNumber(int bulletNumber)
        {
            return "<revon><white> " + (char)bulletNumber + " <revoff><lightgrey>";
        }
    }
}
