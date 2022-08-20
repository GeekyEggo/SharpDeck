namespace StreamDeck
{
    /// <summary>
    /// Provides an enumeration of targets used to identify the Stream Deck.
    /// </summary>
    public enum Target
    {
        /// <summary>
        /// Both <see cref="Hardware"/> and <see cref="Software"/>.
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
