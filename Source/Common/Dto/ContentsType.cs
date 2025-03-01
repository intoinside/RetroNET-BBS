using Common.Enum;

namespace Common.Dto
{
    /// <summary>
    /// Detail of the linked resources
    /// </summary>
    public class ContentsType
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
        /// Bullet for the linked resources
        /// </summary>
        public char BulletItem { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ContentsType()
        {
            Link = string.Empty;
            BulletItem = '\0';
        }
    }
}
