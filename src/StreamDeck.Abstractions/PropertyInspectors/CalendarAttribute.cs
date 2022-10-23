namespace StreamDeck.PropertyInspectors
{
    using System;

    /// <summary>
    /// Provides information about a calendar input to be rendered within a property inspector; these include:
    /// <see href="https://sdpi-components.dev/docs/components/calendar/date">date</see>,
    /// <see href="https://sdpi-components.dev/docs/components/calendar/datetime-local">datetime-local</see>,
    /// <see href="https://sdpi-components.dev/docs/components/calendar/month">month</see>,
    /// <see href="https://sdpi-components.dev/docs/components/calendar/week">week</see>,
    /// <see href="https://sdpi-components.dev/docs/components/calendar/time">time</see>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class CalendarAttribute : InputAttribute
    {
        /// <summary>
        /// Gets or sets the latest acceptable date (<see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/date#max">read more</see>).
        /// </summary>
        public string? Max { get; set; }

        /// <summary>
        /// Gets or sets the earliest acceptable date (<see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/date#min">read more</see>).
        /// </summary>
        public string? Min { get; set; }

        /// <summary>
        /// Gets or sets the granularity that the value must adhere to (<see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/date#step">read more</see>).
        /// </summary>
        public int Step { get; set; } = default;

        /// <summary>
        /// Gets or sets the type of input.
        /// </summary>
        public CalendarType Type { get; set; } = CalendarType.Date;
    }
}
