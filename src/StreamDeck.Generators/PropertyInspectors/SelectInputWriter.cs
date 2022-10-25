namespace StreamDeck.Generators.PropertyInspectors
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Extensions;
    using StreamDeck.Generators.IO;
    using StreamDeck.PropertyInspectors;

    /// <summary>
    /// Provides an HTML writer capable of writing an <see href="https://sdpi-components.dev/docs/components/select">sdpi-select</see>.
    /// </summary>
    internal class SelectInputWriter : OptionBasedInputWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectInputWriter"/> class.
        /// </summary>
        public SelectInputWriter()
            : base("sdpi-select")
        {
        }

        /// <inheritdoc/>
        protected override IEnumerable<(HtmlStringWriter Element, AttributeData Data)> GetOptions(ImmutableArray<AttributeData> propertyAttributes)
        {
            var options = new List<(HtmlStringWriter Element, AttributeData Data)>();

            // Gets the options, and group that together where applicable, by their GroupLabel, into optgroup elements.
            foreach (var (option, attr) in base.GetOptions(propertyAttributes))
            {
                if (attr.TryGetNamedArgument(nameof(OptionAttribute.Group), out string? groupLabel))
                {
                    if (options.FirstOrDefault(opt => opt.Element.TagName == "optgroup" && opt.Element.Attributes.TryGetValue("label", out var label) && label?.ToString() == groupLabel) is (HtmlStringWriter Element, AttributeData Data) optGroup)
                    {
                        // Add the option to the existing group.
                        optGroup.Element.Add(option);
                    }
                    else
                    {
                        // Add the option to a new group.
                        options.Add((new HtmlStringWriter("optgroup").AddAttribute("label", groupLabel!).Add(option), attr));
                    }
                }
                else
                {
                    options.Add((option, attr));
                }
            }

            return options;
        }
    }
}
