namespace StreamDeck.Generators.Analyzers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StreamDeck.Generators.Extensions;
    using StreamDeck.Generators.Models;

    /// <summary>
    /// Provides an analyzer that constructs and validates a <see cref="Manifest"/>.
    /// </summary>
    internal class ManifestAnalyzer
    {
        /// <summary>
        /// Private member field for <see cref="ActionAnalyzers"/>.
        /// </summary>
        private readonly Collection<ActionAnalyzer> _actionAnalyzers = new Collection<ActionAnalyzer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestAnalyzer"/> class.
        /// </summary>
        /// <param name="context">The <see cref="GeneratorExecutionContext"/>.</param>
        /// <param name="syntaxReceiver">The <see cref="StreamDeckSyntaxReceiver"/> that contains the manifest and action information.</param>
        public ManifestAnalyzer(GeneratorExecutionContext context, StreamDeckSyntaxReceiver syntaxReceiver)
        {
            this.GeneratorContext = context;
            this.DiagnosticReporter = new DiagnosticReporter(context);

            // Analyze the manifest.
            if (syntaxReceiver.ManifestAttribute == null
                || !context.Compilation.Assembly.TryGetAttribute<ManifestAttribute>(out var data))
            {
                this.HasManifest = false;
            }
            else
            {
                this.HasManifest = true;

                this.Context = new AttributeContext(syntaxReceiver.ManifestAttribute, data);
                this.Manifest = this.Context.Data.CreateInstance<Manifest>();
                this.SetDefaultValues();
                this.SetProfiles(syntaxReceiver.ProfileAttributes);
            }

            // Analyze the actions regardless of the manifest; this allows us to generate as much as we can later.
            foreach (var action in syntaxReceiver.Actions.Select(a => new ActionAnalyzer(this.GeneratorContext, a, this.Manifest, this.DiagnosticReporter)).OrderBy(a => a.Action.SortIndex).ThenBy(a => a.Action.Name))
            {
                this._actionAnalyzers.Add(action);

                if (this.Manifest != null
                    && action.HasValidUUID
                    && !this.Manifest.TryAddAction(action.Action, out var existing))
                {
                    // todo: Warn duplicate uuid found on manifest.
                }
            }
        }

        /// <summary>
        /// Gets the action analyzers.
        /// </summary>
        public IReadOnlyCollection<ActionAnalyzer> ActionAnalyzers => this._actionAnalyzers;

        /// <summary>
        /// Gets a value indicating whether this instance has a manifest.
        /// </summary>
        public bool HasManifest { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has an error diagnostic.
        /// </summary>
        public bool HasErrorDiagnostic => this.DiagnosticReporter.HasErrorDiagnostic;

        /// <summary>
        /// Gets the manifest.
        /// </summary>
        public Manifest? Manifest { get; } = null;

        /// <summary>
        /// Gets the manifest context.
        /// </summary>
        public AttributeContext Context { get; }

        /// <summary>
        /// Gets the diagnostic reporter.
        /// </summary>
        private DiagnosticReporter DiagnosticReporter { get; }

        /// <summary>
        /// Gets the generator context.
        /// </summary>
        private GeneratorExecutionContext GeneratorContext { get; }

        /// <summary>
        /// Assigns the default values to the <see cref="Manifest"/>.
        /// </summary>
        private void SetDefaultValues()
        {
            // Author.
            if (string.IsNullOrWhiteSpace(this.Manifest!.Author))
            {
                this.Manifest.Author = this.GetNamedValueOrDefault<AssemblyCompanyAttribute>(nameof(ManifestAttribute.Author), () => this.GeneratorContext.Compilation.Assembly.Identity.Name);
            }

            // CodePath.
            if (string.IsNullOrWhiteSpace(this.Manifest.CodePath))
            {
                this.Manifest.CodePath = this.Context.Data.GetNamedArgumentValueOrDefault(nameof(ManifestAttribute.CodePath), () => $"{this.GeneratorContext.Compilation.Assembly.Identity.Name}.exe");
            }

            // Description.
            if (string.IsNullOrWhiteSpace(this.Manifest.Description))
            {
                this.Manifest.Description = this.GetNamedValueOrDefault<AssemblyDescriptionAttribute>(nameof(ManifestAttribute.Description), () =>
                {
                    this.DiagnosticReporter.ReportValueNotDefined<ManifestAttribute>(this.Context.Node, nameof(ManifestAttribute.Description));
                    return string.Empty;
                });
            }

            // Icon.
            if (string.IsNullOrWhiteSpace(this.Manifest.Icon))
            {
                this.Manifest.Icon = this.Context.Data.GetNamedArgumentValueOrDefault(nameof(ManifestAttribute.Icon), () => string.Empty);
                this.DiagnosticReporter.ReportValueNotDefined<ManifestAttribute>(this.Context.Node, nameof(ManifestAttribute.Icon));
            }

            // Name.
            if (string.IsNullOrWhiteSpace(this.Manifest.Name))
            {
                this.Manifest.Name = this.GetNamedValueOrDefault<AssemblyProductAttribute>(nameof(ManifestAttribute.Name), () => this.GeneratorContext.Compilation.Assembly.Identity.Name);
            }

            // Version.
            if (string.IsNullOrWhiteSpace(this.Manifest.Version))
            {
                this.Manifest.Version = this.Context.Data.GetNamedArgumentValueOrDefault(nameof(ManifestAttribute.Version), () => this.GeneratorContext.Compilation.Assembly.Identity.Version.ToString(3));
            }
        }

        /// <summary>
        /// Sets the <see cref="Manifest.Profiles"/>.
        /// </summary>
        /// <param name="profileNodes">The <see cref="AttributeSyntax"/> that represent the <see cref="<see cref="ProfileAttribute"/>.</param>
        private void SetProfiles(List<AttributeSyntax> profileNodes)
        {
            foreach (var profileAttr in this.GeneratorContext.Compilation.Assembly.GetAttributes<ProfileAttribute>())
            {
                var item = new ProfileAttribute((string)profileAttr.ConstructorArguments[0].Value!, (Device)profileAttr.ConstructorArguments[1].Value!);
                if (item.Name == null)
                {
                    var profileNode = profileNodes.First(n => SyntaxReferenceEqualityComparer.Default.Equals(n.GetReference(), profileAttr.ApplicationSyntaxReference));
                    this.DiagnosticReporter.ReportValueCannotBeNull<ProfileAttribute>(profileNode?.ArgumentList?.Arguments[0].GetLocation() ?? this.Context.Node.GetLocation(), nameof(ProfileAttribute.Name));
                }

                this.Manifest!.Profiles.Add(profileAttr.Populate(item));
            }
        }

        /// <summary>
        /// Gets the named argument value, otherwise constructor argument supplied to the <typeparamref name="TAttribute"/> on the assembly, finally falling back <paramref name="defaultFactory"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute to search for on the assembly as a backup value.</typeparam>
        /// <param name="name">The name of the named argument on the <see cref="Context"/> to search for.</param>
        /// <param name="defaultFactory">The default fallback value factory.</param>
        /// <returns>The value of the named argument, or the <typeparamref name="TAttribute"/> constructor argument, otherwise <paramref name="defaultFactory"/>.</returns>
        private string GetNamedValueOrDefault<TAttribute>(string name, Func<string> defaultFactory)
        {
            return this.Context.Data.GetNamedArgumentValueOrDefault(name, () =>
            {
                if (this.GeneratorContext.Compilation.Assembly.TryGetAttribute<TAttribute>(out var attribute)
                    && attribute.ConstructorArguments is { Length: 1 }
                    && attribute.ConstructorArguments[0].Value?.ToString() is string assemblyValue
                    && !string.IsNullOrWhiteSpace(assemblyValue))
                {
                    return assemblyValue;
                }

                return defaultFactory();
            });
        }
    }
}
