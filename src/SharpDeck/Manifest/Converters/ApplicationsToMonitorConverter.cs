namespace SharpDeck.Manifest.Converters
{
    using SharpDeck.Enums;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Provides conversion for applications to monitor defined within the attribute supplied to the <see cref="IConverter.GetValue(Assembly, StreamDeckPluginAttribute)"/>.
    /// </summary>
    public class ApplicationsToMonitorConverter : IConverter
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string PropertyName { get; } = "ApplicationsToMonitor";

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="attribute">The attribute containing information about the plugin.</param>
        /// <returns>The value of the property.</returns>
        public object GetValue(Assembly assembly, StreamDeckPluginAttribute attribute)
        {
            var applicationsToMonitor = new Dictionary<PlatformType, string[]>();
            void add(PlatformType os, string[] apps)
            {
                if (apps?.Length > 0)
                {
                    applicationsToMonitor.Add(os, apps);
                }
            }

            add(PlatformType.Mac, attribute.MacApplicationsToMonitor);
            add(PlatformType.Windows, attribute.WindowsApplicationsToMonitor);

            return applicationsToMonitor.Count > 0 ? applicationsToMonitor : null;
        }
    }
}
