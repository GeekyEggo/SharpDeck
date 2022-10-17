namespace StreamDeck.Generators.IO
{
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Provides a <see cref="IndentedTextWriter"/>, that utilizes a <see cref="StringWriter"/>.
    /// </summary>
    internal sealed class IndentedStringWriter : IndentedTextWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndentedStringWriter"/> class.
        /// </summary>
        /// <param name="indent">The initial <see cref="IndentedTextWriter.Indent"/>.</param>
        public IndentedStringWriter(int indent = 0)
            : base(new StringWriter(new StringBuilder()))
            => this.Indent = indent;

        /// <inheritdoc/>
        public override string ToString()
            => this.InnerWriter.ToString();

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.InnerWriter.Dispose();
        }
    }
}
