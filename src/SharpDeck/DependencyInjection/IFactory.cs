namespace SharpDeck.DependencyInjection
{
    using System;

    /// <summary>
    /// Provides a factory for creating new instances of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of instances this factory creates.</typeparam>
    internal interface IFactory<T>
    {
        /// <summary>
        /// Attempts to create a new instance of <typeparamref name="T"/> from the specified type.
        /// </summary>
        /// <param name="type">The underlying type to create an instance of.</param>
        /// <returns>The new instance of the specified <paramref name="type"/>.</returns>
        T Create(Type type);
    }
}
