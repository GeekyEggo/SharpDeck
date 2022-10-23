namespace StreamDeck.PropertyInspectors
{
    using System;

    /// <summary>
    /// Provides information about a <see href="https://sdpi-components.dev/docs/components/range">sdpi-range</see> to be rendered within a property inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class RangeAttribute : InputAttribute
    {
        /// <summary>
        /// Gets or sets the maximum possible value.
        /// </summary>
        public int Max { get; set; } = default;

        /// <summary>
        /// Gets or sets the minimum possible value.
        /// </summary>
        public int Min { get; set; } = default;

        /// <summary>
        /// Gets or sets a value indicating whether the minimum and maximum labels are shown.
        /// </summary>
        public bool ShowLabels { get; set; } = false;

        /// <summary>
        /// Gets or sets the the granularity that the value must adhere to.
        /// </summary>
        public int Step { get; set; } = default;
    }
}
