namespace SharpDeck.Layouts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides an enumeration of horizontal alignments.
    /// </summary>
    public enum HorizontalAlignment
    {
        /// <summary>
        /// Left alignment.
        /// </summary>
        [EnumMember(Value = "left")]
        Left = 0,

        /// <summary>
        /// Center alignment.
        /// </summary>
        [EnumMember(Value = "center")]
        Center = 1,

        /// <summary>
        /// Right alignment.
        /// </summary>
        [EnumMember(Value = "right")]
        Right = 2
    }
}
