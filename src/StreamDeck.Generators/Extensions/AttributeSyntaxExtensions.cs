namespace StreamDeck.Generators.Extensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Provides extension methods for <see cref="AttributeSyntax"/>.
    /// </summary>
    internal static class AttributeSyntaxExtensions
    {
        /// <summary>
        /// Gets the <see cref="Location"/> of the first named argument that matches <paramref name="name"/>; otherwise <see cref="Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode.GetLocation"/> of this instance.
        /// </summary>
        /// <param name="node">This <see cref="AttributeSyntax"/>.</param>
        /// <param name="name">The name of the argument to whose location is being retrieved..</param>
        /// <returns>The <see cref="Location"/> of the argument; otherwise the <<see cref="AttributeSyntax"/> location.</returns>
        internal static Location GetNamedArgumentLocationOrDefault(this AttributeSyntax node, string name)
            => node.TryGetNamedArgument(name, out var arg)
                ? arg!.GetLocation()
                : node.GetLocation();

        /// <summary>
        /// Attempts to get the first named argument that matches <paramref name="name"/>.
        /// </summary>
        /// <param name="node">This <see cref="AttributeSyntax"/>.</param>
        /// <param name="name">The name of the argument.</param>
        /// <param name="argument">The argument; otherwise <c>null</c>.</param>
        /// <returns><c>true</c> when a matching argument was found; otherwise <c>false</c>.</returns>
        internal static bool TryGetNamedArgument(this AttributeSyntax node, string name, out AttributeArgumentSyntax? argument)
        {
            if (node.ArgumentList != null
                && node.ArgumentList.Arguments.FirstOrDefault(a => a.NameEquals?.Name?.ToString() == name) is AttributeArgumentSyntax arg)
            {
                argument = arg;
                return true;
            }

            argument = null;
            return false;
        }
    }
}
