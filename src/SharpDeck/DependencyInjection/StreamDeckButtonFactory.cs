namespace SharpDeck.DependencyInjection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Provides a factory for creating instances of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="StreamDeckButton"/>.</typeparam>
    internal class StreamDeckButtonFactory<T> : IFactory<T>
        where T : StreamDeckButton
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckButtonFactory{T}" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public StreamDeckButtonFactory(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the service provider responsible for creating new instances of <typeparamref name="T"/>.
        /// </summary>
        private IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Attempts to create a new instance of <typeparamref name="T"/> from the specified type.
        /// </summary>
        /// <param name="type">The underlying type to create an instance of.</param>
        /// <returns>The new instance of the specified <paramref name="type"/>.</returns>
        public T Create(Type type)
        {
            var instance = (T)ActivatorUtilities.CreateInstance(this.ServiceProvider, type);
            instance.Logger = this.ServiceProvider.GetService<ILogger<T>>();

            return instance;
        }
    }
}
