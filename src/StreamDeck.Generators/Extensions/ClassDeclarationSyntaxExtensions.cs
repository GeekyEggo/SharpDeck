namespace StreamDeck.Generators.Extensions
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Provides extension methods for <see cref="ClassDeclarationSyntax"/>.
    /// </summary>
    internal static class ClassDeclarationSyntaxExtensions
    {
        /// <summary>
        /// Gets all attributes associated with the <see cref="ClassDeclarationSyntax"/>, represented by their <see cref="AttributeSyntax"/> and <see cref="AttributeData"/>.
        /// </summary>
        /// <param name="class">This <see cref="ClassDeclarationSyntax"/> whose attributes should be discovered.</param>
        /// <param name="symbol">The <see cref="ISymbol"/> representation of the <see cref="ClassDeclarationSyntax"/>.</param>
        /// <returns>The attributes associated with the <see cref="ClassDeclarationSyntax"/>.</returns>
        internal static ImmutableArray<AttributeContext> GetAttributes(this ClassDeclarationSyntax @class, ISymbol symbol)
        {
            var nodes = @class.AttributeLists.SelectMany(attrList => attrList.Attributes).ToArray();
            var datas = symbol.GetAttributes();

            var array = ImmutableArray.CreateBuilder<AttributeContext>();
            foreach (var node in nodes)
            {
                var nodeRef = node.GetReference();
                var data = datas.FirstOrDefault(d => d.ApplicationSyntaxReference?.SyntaxTree == nodeRef?.SyntaxTree && d.ApplicationSyntaxReference?.Span == nodeRef?.Span)!;

                if (data != null)
                {
                    array.Add(new AttributeContext(node, data));
                }
            }

            return array.ToImmutable();
        }
    }
}
