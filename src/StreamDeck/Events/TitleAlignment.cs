namespace StreamDeck.Events
{
    using StreamDeck.Serialization;

    /// <summary>
    /// Defines the possible title alignments supported by the Elgato Stream Deck.
    /// </summary>
    public class TitleAlignment : EnumString<TitleAlignment>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleAlignment"/> class.
        /// </summary>
        /// <param name="alignment">The alignment.</param>
        internal TitleAlignment(string? alignment)
            : base(alignment ?? string.Empty)
        {
        }

        /// <summary>
        /// Top alignment.
        /// </summary>
        public const string Top = "top";

        /// <summary>
        /// Middle alignment.
        /// </summary>
        public const string Middle = "middle";

        /// <summary>
        /// Bottom alignment.
        /// </summary>
        public const string Bottom = "bottom";
    }
}
