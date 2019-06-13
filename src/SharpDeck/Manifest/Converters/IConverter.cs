namespace SharpDeck.Manifest.Converters
{
    using System.Reflection;

    /// <summary>
    /// Provides a converter (constructor) for properties of the manifest.
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="attribute">The attribute containing information about the plugin.</param>
        /// <returns>The value of the property.</returns>
        object GetValue(Assembly assembly, StreamDeckPluginAttribute attribute);
    }
}
