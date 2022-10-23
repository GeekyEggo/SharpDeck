namespace StreamDeck.PropertyInspectors
{
    /// <summary>
    /// Provides an enumeration of possible <see cref="CalendarAttribute.Type"/>.
    /// </summary>
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    enum CalendarType
    {
        /// <summary>
        /// The <see href="https://sdpi-components.dev/docs/components/calendar/date">date</see> calendar type.
        /// </summary>
        Date,

        /// <summary>
        /// The <see href="https://sdpi-components.dev/docs/components/calendar/datetime-local">datetime-local</see> calendar type.
        /// </summary>
        DateTimeLocal,

        /// <summary>
        /// The <see href="https://sdpi-components.dev/docs/components/calendar/month">month</see> calendar type.
        /// </summary>
        Month,

        /// <summary>
        /// The <see href="https://sdpi-components.dev/docs/components/calendar/week">week</see> calendar type.
        /// </summary>
        Week,

        /// <summary>
        /// The <see href="https://sdpi-components.dev/docs/components/calendar/time">time</see> calendar type.
        /// </summary>
        Time
    }
}
