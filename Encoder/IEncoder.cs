namespace Encoder
{
    /// <summary>
    /// Defines a contract for encoding operations.
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        /// Provide conversion and cleaning of input string in order to
        /// be compliant to the selected encoding.
        /// </summary>
        /// <param name="input">Stream to clean</param>
        /// <returns>Stream cleaned</returns>
        string Cleaner(string input);

        /// <summary>
        /// Number of rows
        /// </summary>
        /// <returns>Number of rows for encoding</returns>
        int NumberOfRows();

        /// <summary>
        /// Number of characters per row
        /// </summary>
        /// <returns>Number of columns for encoding</returns>
        int NumberOfColumns();

        /// <summary>
        /// Converts an ASCII string to a byte array.
        /// </summary>
        /// <param name="stream">Stream to be converted</param>
        /// <param name="clearPage">Whether to clear the page before writing the stream</param>
        /// <returns>A byte array representing the encoded ASCII string.</returns>
        byte[] FromAscii(string stream, bool clearPage = false);
    }
}
