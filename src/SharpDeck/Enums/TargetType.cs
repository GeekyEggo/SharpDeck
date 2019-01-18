namespace SharpDeck.Enums
{
    /// <summary>
    /// Provides an enumeration of targets.
    /// </summary>
    public enum TargetType
    {
        /// <summary>
        /// Both <see cref="TargetType.Hardware"/> and <see cref="TargetType.Software"/>.
        /// </summary>
        Both = 0,

        /// <summary>
        /// Only hardware.
        /// </summary>
        Hardware = 1,

        /// <summary>
        /// Only software.
        /// </summary>
        Software = 2
    }
}
