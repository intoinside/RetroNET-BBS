﻿using Common.Dto;
using Common.Utils;
using Encoder;
using System.Text;

namespace RetroNET_BBS.ContentProvider
{
    /// <summary>
    /// Container for the parsed pages
    /// </summary>
    public static class PageContainer
    {
        /// <summary>
        /// List of the parsed pages
        /// </summary>
        public static List<Page> Pages = new List<Page>();

        /// <summary>
        /// Search for the page from the given link
        /// </summary>
        /// <param name="link">Link to search</param>
        /// <returns>Page parsed (if present)</returns>
        public static Page? FindPageFromLink(string link)
        {
            return Pages.FirstOrDefault(p => p.Link == link);
        }

        /// <summary>
        ///  Get document with tag stripping based on line length
        /// </summary>
        /// <param name="document">Document</param>
        /// <param name="encoder">Encoder which contains number of columns</param>
        /// <returns>Splitted document</returns>
        public static string GetPage(string document, IEncoder encoder)
        {
            var builder = new StringBuilder();
            using var stringWriter = new StringWriter(builder)
            {
                NewLine = "\r\n"
            };

            foreach (var line in StringUtils.SplitToLines(document, encoder.NumberOfColumns() - 1))
            {
                stringWriter.WriteLine(line);
            }

            return builder.ToString();
        }
    }
}
