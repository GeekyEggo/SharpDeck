namespace StreamDeck.PropertyInspectors
{
    using System;

    /// <summary>
    /// Provides information about a <see href="https://sdpi-components.dev/docs/components/file">sdpi-file</see> to be rendered within a property inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class FileAttribute : InputAttribute
    {
        /// <summary>
        /// Gets or sets the types of files that can be selected; directly mapped to the input's accept attribute.
        /// </summary>
        public string? Accept { get; set; }

        /// <summary>
        /// Gets or sets the optional label displayed in the button used to activate the file selector (default ...).
        /// </summary>
        public string? ButtonLabel { get; set; }
    }
}
