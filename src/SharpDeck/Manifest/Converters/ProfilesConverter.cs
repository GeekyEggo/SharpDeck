namespace SharpDeck.Manifest.Converters
{
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides conversion for profiles defined within the assembly supplied to the <see cref="IConverter.GetValue(Assembly, StreamDeckPluginAttribute)"/>.
    /// </summary>
    public class ProfilesConverter : IConverter
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string PropertyName { get; } = "Profiles";

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="attribute">The attribute containing information about the plugin.</param>
        /// <returns>The value of the property.</returns>
        public object GetValue(Assembly assembly, StreamDeckPluginAttribute attribute)
        {
            var profiles = assembly.GetCustomAttributes<StreamDeckProfileAttribute>();
            return profiles.Any() ? profiles : null;
        }
    }
}
