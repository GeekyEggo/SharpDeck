using System.Collections.Generic;

namespace SharpDeck.PropertyInspectors.Payloads
{
    /// <summary>
    /// Provides information about an HTML option.
    /// </summary>
    public class Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Option"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="value">The value.</param>
        public Option(string label, string value)
        {
            this.Label = label;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Option"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="children">The children.</param>
        public Option(string label, List<Option> children)
        {
            this.Label = label;
            this.Children = children;
        }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        public List<Option> Children { get; set; } = null;

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }
    }
}
