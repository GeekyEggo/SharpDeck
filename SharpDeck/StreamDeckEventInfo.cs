namespace SharpDeck
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Provides information about an event that represents an Elgato Stream Deck event, and methods for invoking it.
    /// </summary>
    public class StreamDeckEventInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckEventInfo"/> class.
        /// </summary>
        /// <param name="eventInfo">The event information.</param>
        /// <param name="fieldInfo">The field information.</param>
        public StreamDeckEventInfo(EventInfo eventInfo, FieldInfo fieldInfo)
        {
            this.Name = eventInfo.GetCustomAttribute<StreamDeckEventAttribute>()?.Name;
            this.ArgsType = eventInfo.EventHandlerType.GetMethod(nameof(EventHandler.Invoke)).GetParameters()[1].ParameterType;
            this.FieldInfo = fieldInfo;
        }

        /// <summary>
        /// Gets or sets the name of the event; this is the Elgato Stream Deck event name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the arguments associated with the event info.
        /// </summary>
        public Type ArgsType { get; }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        public FieldInfo FieldInfo { get; }

        /// <summary>
        /// Invokes the any handlers associated with the event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        public void Invoke(object sender, object args)
        {
            var @delegate = (Delegate)this.FieldInfo.GetValue(sender);
            if (@delegate == null)
            {
                return;
            }

            foreach (var handler in @delegate.GetInvocationList())
            {
                handler.Method.Invoke(handler.Target, new[] { sender, args });
            }
        }
    }
}
