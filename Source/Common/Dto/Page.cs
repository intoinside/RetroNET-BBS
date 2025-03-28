﻿using Common.Enum;

namespace Common.Dto
{
    /// <summary>
    /// Page structure, contains the parsed sources
    /// </summary>
    public class Page
    {
        /// <summary>
        /// Determines the type of the source
        /// </summary>
        public Sources Source { get; set; }

        /// <summary>
        /// Link (or path) to the source
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Title of the page if any
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Private content parsed from the source
        /// </summary>
        private string content;

        /// <summary>
        /// Content parsed from the source (with normalization)
        /// </summary>
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value.Replace("\r\n", "\n");
                content = content.Replace("\n", "\r\n");
            }
        }

        /// <summary>
        /// List of bullet items for the linked resources
        /// </summary>
        public string AcceptedDetailIndex { get; set; }

        /// <summary>
        /// List of the linked resources
        /// </summary>
        public List<ContentsType> LinkedContentsType { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Page()
        {
            Link = string.Empty;
            Title = string.Empty;
            Content = string.Empty;
            AcceptedDetailIndex = string.Empty;
            LinkedContentsType = new List<ContentsType>();
        }
    }
}
