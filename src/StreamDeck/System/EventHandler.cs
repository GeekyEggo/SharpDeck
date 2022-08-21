namespace System
{
    /// <summary>
    /// Represents the method that will handle an event when the event provides data.
    /// </summary>
    /// <typeparam name="TSender">The type of the sender.</typeparam>
    /// <typeparam name="TArgs">The type of the arguments.</typeparam>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An object that contains the event data.</param>
    public delegate void EventHandler<in TSender, in TArgs>(TSender sender, TArgs args);
}
