

namespace J2N.IO
{
    /// <summary>
    /// Endianness
    /// </summary>
    public enum Endianness
    {
        /// <summary>
        /// Read-only mapping mode.
        /// </summary>
        LittleEndian,

        /// <summary>
        /// Private mapping mode (equivalent to copy on write).
        /// </summary>
        BigEndian
    }
}
