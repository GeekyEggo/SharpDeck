namespace StreamDeck.Generators.PropertyInspectors
{
    using StreamDeck.Generators.Generators.PropertyInspectors;
    using StreamDeck.PropertyInspectors;

    /// <summary>
    /// Provides an HTML writer capable of writing a
    /// <see href="https://sdpi-components.dev/docs/components/calendar/date">date</see>,
    /// <see href="https://sdpi-components.dev/docs/components/calendar/datetime-local">datetime-local</see>,
    /// <see href="https://sdpi-components.dev/docs/components/calendar/month">month</see>,
    /// <see href="https://sdpi-components.dev/docs/components/calendar/week">week</see>,
    /// <see href="https://sdpi-components.dev/docs/components/calendar/time">time</see>.
    /// </summary>
    internal class CalendarWriter : FieldItemWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarWriter"/> class.
        /// </summary>
        public CalendarWriter()
            : base("sdpi-calendar")
        {
        }

        /// <inheritdoc/>
        protected override (string PropertyName, object? Value) GetAttribute(string propertyName, object? value)
            => propertyName switch
            {
                nameof(CalendarAttribute.Type) when value is int and (int)CalendarType.DateTimeLocal => (propertyName.ToLowerInvariant(), "datetime-local"),
                nameof(CalendarAttribute.Type) when value is int and (int)CalendarType.Month => (propertyName.ToLowerInvariant(), "month"),
                nameof(CalendarAttribute.Type) when value is int and (int)CalendarType.Time => (propertyName.ToLowerInvariant(), "time"),
                nameof(CalendarAttribute.Type) when value is int and (int)CalendarType.Week => (propertyName.ToLowerInvariant(), "week"),
                nameof(CalendarAttribute.Type) => (propertyName.ToLowerInvariant(), "date"),
                _ => base.GetAttribute(propertyName, value)
            };
    }
}
