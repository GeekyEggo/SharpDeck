namespace StreamDeck.Generators.IO
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides a basic <see cref="StringWriter"/> capable of writing HTML.
    /// NB. As this is used as part of source generation, no HTML encoding occurs, and the ownice is put on the consumer.
    /// </summary>
    internal class HtmlStringWriter : IndentedStringWriter
    {
        /// <summary>
        /// Gets the open tags.
        /// </summary>
        private Stack<HtmlTag> OpenTags { get; } = new Stack<HtmlTag>();

        /// <summary>
        /// Renders the begin tag.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        public void RenderBeginTag(string tagName)
        {
            if (this.TryPeek(out var tag))
            {
                this.WriteLine(">");
                this.Indent++;

                tag!.IsBeginTagOpen = false;
            }

            this.OpenTags.Push(new HtmlTag(tagName));
            this.Write($"<{tagName}");
        }

        /// <summary>
        /// Adds the attribute to the current tag.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public void AddAttribute(string name, object? value)
        {
            if (this.TryPeek(out var _))
            {
                if (value is bool and true)
                {
                    this.Write($" {name}");
                }
                else if (value is not bool and not null
                    && value.ToString() is string strValue and not "")
                {
                    this.Write($" {name}=\"{strValue}\"");
                }
            }
        }

        /// <summary>
        /// Renders the end tag of the open element..
        /// </summary>
        public void RenderEndTag()
        {
            if (this.TryPeek(out var tag))
            {
                if (tag!.IsBeginTagOpen)
                {
                    this.Write($">");
                    tag.IsBeginTagOpen = false;
                }
                else
                {
                    this.Indent--;
                }

                this.WriteLine($"</{tag.Name}>");
                this.OpenTags.Pop();
            }
        }

        /// <summary>
        /// Peeks <see cref="OpenTags"/>, returning <c>true</c> when a tag is open.
        /// </summary>
        /// <param name="tag">The open tag.</param>
        /// <returns><c>true</c> when there is an open tag; otherwise <c>false</c>.</returns>
        private bool TryPeek(out HtmlTag? tag)
        {
            if (this.OpenTags.Count > 0)
            {
                tag = this.OpenTags.Peek();
                return true;
            }
            else
            {
                tag = default;
                return false;
            }
        }

        /// <summary>
        /// Provides basic information about an HTML tag.
        /// </summary>
        private class HtmlTag
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="HtmlTag"/> class.
            /// </summary>
            /// <param name="name">The tag name.</param>
            public HtmlTag(string name)
                => this.Name = name;

            /// <summary>
            /// Gets the name.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance's begin tag is open.
            /// </summary>
            public bool IsBeginTagOpen { get; set; } = true;
        }
    }
}
