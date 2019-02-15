namespace SharpDeck.Tests.Mocks
{
    using SharpDeck.Events.PropertyInspectors;

    /// <summary>
    /// Provides a mock implementation of <see cref="PropertyInspectorPayload"/>.
    /// </summary>
    public class FooPropertyInspectorPayload : PropertyInspectorPayload
    {
        /// <summary>
        /// Gets or sets the source of the method that set this value.
        /// </summary>
        public string Source { get; set; }
    }
}
