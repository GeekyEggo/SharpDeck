namespace SharpDeck
{
    /// <summary>
    /// An event handler that is invoked by a <see cref="IStreamDeckClient"/>.
    /// </summary>
    /// <typeparam name="T">The type of event arguments.</typeparam>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void StreamDeckClientEventHandler<T>(IStreamDeckClient sender, T e);
}
