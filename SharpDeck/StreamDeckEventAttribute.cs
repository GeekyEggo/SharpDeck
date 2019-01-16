namespace SharpDeck
{
    using System;

    [AttributeUsage(AttributeTargets.Event)]
    public class StreamDeckEventAttribute : Attribute
    {
        public StreamDeckEventAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
