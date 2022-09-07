namespace StreamDeck
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides an <see cref="Attribute"/> whose <see cref="Attribute.TypeId"/> is not serialized.
    /// </summary>
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class SerializableSafeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableSafeAttribute"/> class.
        /// </summary>
        internal SerializableSafeAttribute()
        {
        }

        /// <inheritdoc/>
        [IgnoreDataMember]
        public override object TypeId => base.TypeId;
    }
}
