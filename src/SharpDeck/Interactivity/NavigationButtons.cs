namespace SharpDeck.Interactivity
{
    using System;

    /// <summary>
    /// Provides an enumeration of possible navigation buttons.
    /// </summary>
    [Flags]
    public enum NavigationButtons
    {
        /// <summary>
        /// No buttons.
        /// </summary>
        None = 0,

        /// <summary>
        /// The previous button.
        /// </summary>
        Previous = 1,

        /// <summary>
        /// The next button.
        /// </summary>
        Next = 2
    }
}
