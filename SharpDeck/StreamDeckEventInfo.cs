namespace SharpDeck
{
    using System;
    using System.Reflection;

    public class StreamDeckEventInfo
    {
        public StreamDeckEventInfo(EventInfo eventInfo, FieldInfo fieldInfo)
        {
            this.Name = eventInfo.GetCustomAttribute<StreamDeckEventAttribute>()?.Name;
            this.ArgsType = eventInfo.EventHandlerType.GetMethod(nameof(EventHandler.Invoke)).GetParameters()[1].ParameterType;
            this.FieldInfo = fieldInfo;
        }

        public string Name { get; set; }
        public Type ArgsType { get; set; }
        public FieldInfo FieldInfo { get; }

        public void Invoke(object sender, object args)
        {
            var @delegate = (Delegate)this.FieldInfo.GetValue(sender);
            foreach (var handler in @delegate.GetInvocationList())
            {
                handler.Method.Invoke(handler.Target, new[] { sender, args });
            }
        }
    }
}
