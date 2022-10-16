namespace StreamDeck.PropertyInspectors
{
    using System;

    /// <summary>
    /// Provides information about the generation of a property inspector HTML file.
    /// </summary>
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class PropertyInspectorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInspectorAttribute"/> class.
        /// </summary>
        /// <param name="type">The type that contains information about components within the property inspector.</param>
        public PropertyInspectorAttribute(Type type)
            => this.Type = type;

        /// <summary>
        /// Gets or sets the type that contains information about components within the property inspector.
        /// </summary>
        public Type Type { get; set; }
    }
}
