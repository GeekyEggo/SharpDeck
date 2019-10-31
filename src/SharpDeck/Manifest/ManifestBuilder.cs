namespace SharpDeck.Manifest
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using SharpDeck.Manifest.Converters;
    using SharpDeck.Serialization;

    /// <summary>
    /// Dynamically reads an assembly, constructing a manifest file that can be used when registering an Elgato Stream Deck plugin.
    /// </summary>
    internal class ManifestBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestBuilder"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public ManifestBuilder(Assembly assembly)
            : this(assembly, new StreamDeckPluginAttribute())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestBuilder"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="defaultValues">The default values.</param>
        public ManifestBuilder(Assembly assembly, StreamDeckPluginAttribute defaultValues)
        {
            this.Assembly = assembly;
            this.Attribute = this.GetManifestInfo();
            this.Serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = new PredicateInclusiveContractResolver(m => !typeof(Attribute).IsAssignableFrom(m.DeclaringType) || m.Name != nameof(Attribute.TypeId)),
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore
            });

            this.Manifest = JObject.FromObject(defaultValues, this.Serializer);
            this.Manifest.Merge(JObject.FromObject(this.Attribute, this.Serializer));

            this.BuildManifest();
        }

        /// <summary>
        /// Gets the assembly.
        /// </summary>
        private Assembly Assembly { get; }

        /// <summary>
        /// Gets the main attribute containing information about the plugin.
        /// </summary>
        private StreamDeckPluginAttribute Attribute { get; }

        /// <summary>
        /// Gets the JSON serializer.
        /// </summary>
        private JsonSerializer Serializer { get; }

        /// <summary>
        /// Gets the manifest object containing all of the information about the plugin.
        /// </summary>
        private JObject Manifest { get; }

        /// <summary>
        /// Converts to manifest information to JSON string.
        /// </summary>
        /// <returns>The JSON string.</returns>
        public string ToJson()
            => this.Manifest.ToString();

        /// <summary>
        /// Attempts to get the main manifest information from the assembly.
        /// </summary>
        /// <returns>The manifest information.</returns>
        private StreamDeckPluginAttribute GetManifestInfo()
        {
            var pluginAttr = this.Assembly.GetCustomAttribute<StreamDeckPluginAttribute>();
            if (pluginAttr == null)
            {
                throw new InvalidOperationException($"Missing attribute; please ensure the assembly defines the attribute {nameof(StreamDeckPluginAttribute)}.");
            }

            return pluginAttr;
        }

        /// <summary>
        /// Builds the manifest from a collection of converters, appending each as a property / value on the main manifest object.
        /// </summary>
        private void BuildManifest()
        {
            new List<IConverter>
            {
                new ActionsConverter(this.Serializer),
                new ApplicationsToMonitorConverter(),
                new PlatformsConverter(),
                new ProfilesConverter(),
                new SoftwareConverter()
            }.ForEach(converter =>
            {
                this.AppendChild(
                    converter.PropertyName,
                    converter.GetValue(this.Assembly, this.Attribute));
            });
        }

        /// <summary>
        /// Appends the value as a child property of the main manifest object.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value to add to the manifest. When an <see cref="object"/> is supplied, it is converted to a <see cref="JToken"/>; when <c>null</c> the value is ignored.</param>
        private void AppendChild(string propertyName, object value)
        {
            if (value == null)
            {
                return;
            }

            var token = value is JToken ? (JToken)value : JToken.FromObject(value, this.Serializer);
            if (this.Manifest.ContainsKey(propertyName))
            {
                this.Manifest[propertyName].Replace(token);
            }
            else
            {
                this.Manifest.Add(propertyName, token);
            }
        }
    }
}
