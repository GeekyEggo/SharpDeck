namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StreamDeck.Generators.Extensions;

    /// <summary>
    /// Provides information about an attribute.
    /// </summary>
    internal struct AttributeContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeContext"/> struct.
        /// </summary>
        /// <param name="node">The <see cref="AttributeSyntax"/> node of the attribute.</param>
        /// <param name="data">The <see cref="AttributeData"/> of the attribute.</param>
        public AttributeContext(AttributeSyntax node, AttributeData data)
        {
            this.Data = data;
            this.Node = node;
        }

        /// <summary>
        /// Gets the <see cref="AttributeData"/>.
        /// </summary>
        public AttributeData Data { get; }

        /// <summary>
        /// Gets the <see cref="AttributeSyntax"/>.
        /// </summary>
        public AttributeSyntax Node { get; }

        /// <summary>
        /// Creates a new instance of <typeparamref name="T"/>, and attempts to populate it from <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">The type of attribute to construct.</typeparam>
        /// <returns>The constructed attribute.</returns>
        public T As<T>()
            => this.Data.CreateInstance<T>();
    }
}
