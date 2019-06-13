namespace SharpDeck.Manifest.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using SharpDeck.Extensions;
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides conversion for actions defined within the assembly supplied to the <see cref="IConverter.GetValue(Assembly, StreamDeckPluginAttribute)"/>.
    /// </summary>
    public class ActionsConverter : IConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionsConverter"/> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public ActionsConverter(JsonSerializer serializer)
        {
            this.Serializer = serializer;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string PropertyName { get; } = "Actions";

        /// <summary>
        /// Gets the serializer.
        /// </summary>
        public JsonSerializer Serializer { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="attribute">The attribute containing information about the action.</param>
        /// <returns>The value of the property.</returns>
        public object GetValue(Assembly assembly, StreamDeckPluginAttribute attribute)
            => assembly.GetTypesWithCustomAttribute<StreamDeckActionAttribute>().Aggregate(new JArray(), this.AccumulateActions);

        /// <summary>
        /// Accumulates the actions.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <param name="info">The plugin information.</param>
        /// <returns>The array of actions.</returns>
        private JArray AccumulateActions(JArray seed, (Type type, StreamDeckActionAttribute attribute) info)
        {
            var action = JObject.FromObject(info.attribute, this.Serializer);
            action.Add(new JProperty("States", info.type.GetCustomAttributes<StreamDeckActionStateAttribute>().Aggregate(new JArray(), this.AccumulateStates)));

            seed.Add(action);
            return seed;
        }

        /// <summary>
        /// Accumulates the states.
        /// </summary>
        /// <param name="seed">The seed; this is the array stored within the actions object.</param>
        /// <param name="attribute">The attribute containing information about the actions state.</param>
        /// <returns>The array of states.</returns>
        private JArray AccumulateStates(JArray seed, StreamDeckActionStateAttribute attribute)
        {
            seed.Add(JObject.FromObject(attribute, this.Serializer));
            return seed;
        }
    }
}
