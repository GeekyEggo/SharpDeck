namespace StreamDeck.PropertyInspectors
{
    using System;

    /// <summary>
    /// Provides information for rendering &lt;option&gt; and &lt;optgroup&gt; elements.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class OptionAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether the option is disabled.
        /// </summary>
        public bool IsDisabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the group the option is associated with; this will also be the &lt;optgroup&gt; label.
        /// </summary>
        public string? Group { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string? Value { get; set; }
    }
}
