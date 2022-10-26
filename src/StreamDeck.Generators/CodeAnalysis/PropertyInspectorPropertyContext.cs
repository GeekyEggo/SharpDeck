namespace StreamDeck.Generators.CodeAnalysis
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides information about a property being written to a property inspector.
    /// </summary>
    internal struct PropertyInspectorPropertyContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInspectorPropertyContext"/> struct.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="component">The attribute data that represents the component type.</param>
        /// <param name="attributes">The attributes associated with the property.</param>
        public PropertyInspectorPropertyContext(IPropertySymbol property, AttributeData component, ImmutableArray<AttributeData> attributes)
        {
            this.Attributes = attributes;
            this.Component = component;
            this.Property = property;
        }

        /// <summary>
        /// Gets the attributes associated with the <see cref="Property"/>.
        /// </summary>
        public ImmutableArray<AttributeData> Attributes { get; }

        /// <summary>
        /// Gets the attribute data that represents the component type.
        /// </summary>
        public AttributeData Component { get; }

        /// <summary>
        /// Gets the property.
        /// </summary>
        public IPropertySymbol Property { get; }
    }
}
