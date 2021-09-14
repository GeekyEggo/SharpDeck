namespace SharpDeck.DependencyInjection
{
    using System;

    /// <summary>
    /// Provides methods for instantiating instances from their <see cref="Type"/>.
    /// </summary>
    internal interface IActivator
    {
        /// <summary>
        /// Instantiates a new instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The instantiated instance.</returns>
        object CreateInstance(Type type);
    }
}
