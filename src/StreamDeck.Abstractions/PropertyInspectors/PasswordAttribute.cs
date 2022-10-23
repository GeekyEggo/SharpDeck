namespace StreamDeck.PropertyInspectors
{
    using System;

    /// <summary>
    /// Provides information about a <see href="https://sdpi-components.dev/docs/components/password">sdpi-password</see> to be rendered within a property inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class PasswordAttribute : InputAttribute
    {
        /// <summary>
        /// Gets or sets maximum length of the value.
        /// </summary>
        public int MaxLength { get; set; } = default;
    }
}
