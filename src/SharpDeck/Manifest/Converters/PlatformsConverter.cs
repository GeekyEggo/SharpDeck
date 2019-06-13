namespace SharpDeck.Manifest.Converters
{
    using SharpDeck.Enums;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides conversion for platforms defined within the attribute supplied to the <see cref="IConverter.GetValue(Assembly, StreamDeckPluginAttribute)"/>.
    /// </summary>
    public class PlatformsConverter : IConverter
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string PropertyName { get; } = "OS";

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="attribute">The attribute containing information about the plugin.</param>
        /// <returns>The value of the property.</returns>
        public object GetValue(Assembly assembly, StreamDeckPluginAttribute attribute)
        {
            return new[]
            {
                new PlatformInfo(PlatformType.Mac, attribute.MacMinimumVersion),
                new PlatformInfo(PlatformType.Windows, attribute.WindowsMinimumVersion)
            }.Where(p => !string.IsNullOrWhiteSpace(p.MinimumVersion));
        }
    }
}
