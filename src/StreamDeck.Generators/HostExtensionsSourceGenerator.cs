namespace StreamDeck.Generators
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Analyzers;
    using StreamDeck.Generators.Extensions;

    /// <summary>
    /// Generates extension methods on 'IHost'.
    /// </summary>
    internal class HostExtensionsSourceGenerator : BaseSourceGenerator
    {
        /// <summary>
        /// The fully qualified name of Microsoft.Extensions.Hosting.
        /// </summary>
        public const string HOSTING_FULLY_QUALIFIED_NAME = "global::Microsoft.Extensions.Hosting";

        /// <summary>
        /// The fully qualified name of the Microsoft.Extensions.Hosting.IHost interface.
        /// </summary>
        private const string IHOST_FULLY_QUALIFIED_NAME = $"{HOSTING_FULLY_QUALIFIED_NAME}.IHost";

        /// <inheritdoc/>
        internal override void Execute(GeneratorExecutionContext context, StreamDeckSyntaxReceiver syntaxReceiver, ManifestAnalyzer manifestAnalyzer)
        {
            var sourceText = $$"""
                // <auto-generated />

                namespace StreamDeck.Extensions.Routing
                {
                    using {{HOSTING_FULLY_QUALIFIED_NAME}};

                    public static class GeneratedHostExtensions
                    {
                        /// <summary>
                        /// Maps all actions based on their UUID to the plugin.
                        /// </summary>
                        /// <param name="host">The <see cref="IHost"/> to configure.</param>
                        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
                        public static {{IHOST_FULLY_QUALIFIED_NAME}} MapActions(this {{IHOST_FULLY_QUALIFIED_NAME}} host)
                        {
                            // Auto generated actions from their ActionAttribute.
                            {{GenerateMapActions(manifestAnalyzer)}}
                            return host;
                        }

                        /// <summary>
                        /// Maps all actions based on their UUID, and runs the plugin
                        /// </summary>
                        /// <param name="host">The <see cref="IHost"/> to configure and run.</param>
                        public static void RunPlugin(this {{IHOST_FULLY_QUALIFIED_NAME}} host)
                            => host.MapActions().Run();
                    }
                }
                """;

            context.AddSource("HostExtensions.g", sourceText);
        }

        /// <summary>
        /// Generates the code responsible for mapping actions to the 'IHost'.
        /// </summary>
        /// <param name="manifestAnalyzer">The <see cref="ManifestAnalyzer"/> that contains the actions.</param>
        /// <returns>The generated code.</returns>
        private static string GenerateMapActions(ManifestAnalyzer manifestAnalyzer)
        {
            var actions = new Dictionary<string, ActionAnalyzer>();

            using var writer = new IndentedTextWriter(new StringWriter());
            writer.Indent = 3;

            foreach (var actionAnalyzer in manifestAnalyzer.ActionAnalyzers.Where(CanAutoGenerate))
            {
                if (actions.TryGetValue(actionAnalyzer.Action.UUID, out var existingNode))
                {
                    // TODO: report an error, but only when MapActions is called.
                }
                else
                {
                    actions.Add(actionAnalyzer.Action.UUID, actionAnalyzer);
                    writer.WriteLine($"host.MapAction<{actionAnalyzer.Context.Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>(\"{actionAnalyzer.Action.UUID}\");");
                }
            }

            return writer.InnerWriter.ToString();
        }

        /// <summary>
        /// Determines whether the specified <paramref name="node"/> is declared by a class that can be mapped to the IHost.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns><c>true</c> when the node's symbol can be automatically mapped to an IHost; otherwise <c>false</c>.</returns>
        private static bool CanAutoGenerate(ActionAnalyzer actionAnalyzer)
        {
            if (actionAnalyzer.Context.Symbol.TryGetAttribute<DoNotMapAttribute>(out var _))
            {
                return false;
            }

            if (!actionAnalyzer.HasValidUUID)
            {
                return false;
                // TODO: Warn?
            }

            if (actionAnalyzer.Context.Symbol.IsGenericType)
            {
                // TODO: report an error, but only when MapActions is called.
                return false;
            }

            if (actionAnalyzer.Context.Symbol.IsAbstract)
            {
                // TODO: report an error, but only when MapActions is called.
                return false;
            }

            var baseType = actionAnalyzer.Context.Symbol.BaseType;
            while (baseType != null)
            {
                if (baseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::StreamDeck.StreamDeckAction")
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}
