namespace StreamDeck.PropertyInspectors
{
    /// <summary>
    /// Provides an enumeration of possible "value-type" values.
    /// </summary>
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    enum InputValueType
    {
        /// <summary>
        /// The value will be a <see cref="bool"/>.
        /// </summary>
        Boolean,

        /// <summary>
        /// The value will be a numeric value, e.g. <see cref="int"/>, <see cref="float"/>, etc.
        /// </summary>
        Number,

        /// <summary>
        /// The value will be a <see cref="string"/>.
        /// </summary>
        String
    }
}
