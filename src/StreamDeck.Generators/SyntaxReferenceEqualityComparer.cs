namespace StreamDeck.Generators
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides an <see cref="IEqualityComparer{T}"/> for <see cref="SyntaxReference"/>.
    /// </summary>
    internal class SyntaxReferenceEqualityComparer : IEqualityComparer<SyntaxReference?>
    {
        /// <summary>
        /// Gets the default <see cref="IEqualityComparer{SyntaxReference}"/>.
        /// </summary>
        public static IEqualityComparer<SyntaxReference?> Default { get; } = new SyntaxReferenceEqualityComparer();

        /// <inheritdoc/>
        public bool Equals(SyntaxReference? x, SyntaxReference? y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.SyntaxTree == y.SyntaxTree && x.Span == y.Span;

        }

        /// <inheritdoc/>
        public int GetHashCode(SyntaxReference? obj)
            => obj?.GetHashCode() ?? 0;
    }
}
