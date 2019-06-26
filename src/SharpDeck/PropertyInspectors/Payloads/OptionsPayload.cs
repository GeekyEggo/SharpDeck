namespace SharpDeck.PropertyInspectors.Payloads
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides a payload containing options that can serve as a data source for an HTML select input.
    /// </summary>
    public class OptionsPayload : PropertyInspectorPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsPayload"/> class.
        /// </summary>
        public OptionsPayload()
            : this(new List<Option>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsPayload"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public OptionsPayload(IEnumerable<Option> options)
        {
            this.Options = options.ToList();
        }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public List<Option> Options { get; set; }
    }
}
