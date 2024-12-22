namespace RetroNET_BBS.Encoders
{
    /// <summary>
    /// Defines a contract for encoding operations.
    /// </summary>
    public interface Encoder
    {
        /// <summary>
        /// Converts an ASCII string to a byte array.
        /// </summary>
        /// <returns>A byte array representing the encoded ASCII string.</returns>
        byte[] FromAscii();
    }
}
