namespace StreamDeck.Generators.IO
{
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Provides a basic structure for creating and writing HTML elements.
    /// </summary>
    internal class HtmlStringWriter
    {
        /// <summary>
        /// All element tags that represent a void element.
        /// <see href="https://www.w3.org/TR/2011/WD-html-markup-20110113/syntax.html#void-element"/>.
        /// </summary>
        private static readonly string[] VOID_ELEMENTS = new[] { "area", "base", "br", "col", "command", "embed", "hr", "img", "input", "keygen", "link", "meta", "param", "source", "track", "wbr" };

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlStringWriter"/> class.
        /// </summary>
        public HtmlStringWriter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlStringWriter"/> class.
        /// </summary>
        /// <param name="tagName">The name of the HTML tag.</param>
        private HtmlStringWriter(string tagName)
            => this.TagName = tagName;

        /// <summary>
        /// Gets the attributes associated with the element.
        /// </summary>
        private IDictionary<string, object?> Attributes { get; } = new Dictionary<string, object?>();

        /// <summary>
        /// Gets the children.
        /// </summary>
        private List<HtmlStringWriter> Children { get; } = new List<HtmlStringWriter>();

        /// <summary>
        /// Gets the name of the HTML tag.
        /// </summary>
        private string? TagName { get; }

        /// <summary>
        /// Adds the specified tag name to this element as a child.
        /// </summary>
        /// <param name="tagName">Name of the child tag.</param>
        /// <param name="build">The action used to build the child element.</param>
        /// <returns>This <see cref="HtmlStringWriter"/> for chaining.</returns>
        public HtmlStringWriter Add(string tagName, Action<HtmlStringWriter> build)
        {
            if (VOID_ELEMENTS.Contains(this.TagName))
            {
                throw new InvalidOperationException("Void elements cannot have children.");
            }

            var elem = new HtmlStringWriter(tagName);
            build(elem);

            this.Children.Add(elem);
            return this;
        }

        /// <summary>
        /// Adds an attribute to this <see cref="HtmlStringWriter"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>This <see cref="HtmlStringWriter"/> for chaining.</returns>
        public HtmlStringWriter AddAttribute(string name, object? value)
        {
            this.Attributes.Add(name, value);
            return this;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            using var writer = new IndentedStringWriter();

            writer.WriteLine("<!doctype html>");
            this.Children.ForEach(c => c.Write(writer));

            return writer.ToString();
        }

        /// <summary>
        /// Writes this HTMl element to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        private void Write(IndentedStringWriter writer)
        {
            writer.Write($"<{this.TagName}");
            foreach (var attr in this.Attributes)
            {
                if (attr.Value is bool and true)
                {
                    writer.Write($" {attr.Key}");
                }
                else if (attr.Value is not bool and not null
                    && attr.Value.ToString() is string strValue and not "")
                {
                    writer.Write($" {attr.Key}=\"{HttpUtility.HtmlEncode(strValue)}\"");
                }
            }

            if (this.Children.Count > 0)
            {
                writer.WriteLine(">");
                writer.Indent++;
                this.Children.ForEach(c => c.Write(writer));
                writer.Indent--;
                writer.WriteLine($"</{this.TagName}>");
            }
            else if (VOID_ELEMENTS.Contains(this.TagName))
            {
                writer.WriteLine(" />");
            }
            else
            {
                writer.WriteLine($"></{this.TagName}>");
            }
        }
    }
}
