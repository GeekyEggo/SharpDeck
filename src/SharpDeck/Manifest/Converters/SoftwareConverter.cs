namespace SharpDeck.Manifest.Converters
{
    using System.Reflection;

    /// <summary>
    /// Provides conversion for supported minimum version of the Elgato software defined within the attribute supplied to the <see cref="IConverter.GetValue(Assembly, StreamDeckPluginAttribute)"/>.
    /// </summary>
    public class SoftwareConverter : IConverter
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string PropertyName { get; } = "Software";

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="attribute">The attribute containing information about the plugin.</param>
        /// <returns>The value of the property.</returns>
        public object GetValue(Assembly assembly, StreamDeckPluginAttribute attribute)
            => new { MinimumVersion = attribute.SoftwareMinimumVersion };
    }
}
