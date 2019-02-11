namespace SharpDeck.Events
{
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides information about method that should be invoked when receiving a specific message from <see cref="StreamDeckActionReceiver.SendToPlugin"/>.
    /// </summary>
    public class PropertyInspectorMethodInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInspectorMethodInfo"/> class.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="attr">The attribute used to decorated the method.</param>
        public PropertyInspectorMethodInfo(MethodInfo methodInfo, PropertyInspectorMethodAttribute attr)
        {
            this.SendToPluginEvent = this.GetValueOrDefault(attr.SendToPluginEvent, methodInfo.Name);
            this.SendToPropertyInspectorEvent = this.GetValueOrDefault(attr.SendToPropertyInspectorEvent, methodInfo.Name);

            this.MethodInfo = methodInfo;
            this.ParameterInfo = methodInfo.GetParameters().FirstOrDefault();
        }

        /// <summary>
        /// Gets the method information.
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        /// Gets the information for the parameter that should be supplied to the method information.
        /// </summary>
        public ParameterInfo ParameterInfo { get; }

        /// <summary>
        /// Gets the `sendToPlugin` event name.
        /// </summary>
        public string SendToPluginEvent { get; }

        /// <summary>
        /// Gets the `sendToPropertyInspector` event name.
        /// </summary>
        public string SendToPropertyInspectorEvent { get; }

        /// <summary>
        /// Gets the string value or default when <see cref="string.IsNullOrWhiteSpace(string)"/> is <c>true</c>.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <param name="default">The default.</param>
        /// <returns>The value; otherwise default.</returns>
        private string GetValueOrDefault(string val, string @default)
            => string.IsNullOrWhiteSpace(val) ? @default : val;
    }
}
