namespace StreamDeck.Serialization
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Extends <see cref="IgnoreDataMemberAttribute"/> to only ignore the data member when the value equals <see cref="IgnoreDataMemberWhenAttribute.Value"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal class IgnoreDataMemberWhenAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoreDataMemberWhenAttribute"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public IgnoreDataMemberWhenAttribute(object value)
            => this.Value = value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value { get; }
    }
}
