namespace StreamDeck.Routing
{
    using System;
    using System.Collections.Concurrent;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Provides an implementation of <see cref="IActionFactory"/> that utilises the <see cref="ActivatorUtilities"/> class.
    /// </summary>
    internal class ActivatorUtilitiesActionFactory : IActionFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivatorUtilitiesActionFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ActivatorUtilitiesActionFactory(IServiceProvider serviceProvider)
            => this.ServiceProvider = serviceProvider;

        /// <summary>
        /// Gets the <see cref="ObjectFactory"/> cache.
        /// </summary>
        private ConcurrentDictionary<Type, ObjectFactory> ObjectFactoryCache { get; } = new ConcurrentDictionary<Type, ObjectFactory>();

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        private IServiceProvider ServiceProvider { get; }

        /// <inheritdoc/>
        public StreamDeckAction CreateInstance(Type actionType, ActionInitializationContext context)
        {
            var objFactory = this.ObjectFactoryCache.GetOrAdd(actionType, t => ActivatorUtilities.CreateFactory(t, new[] { typeof(ActionInitializationContext) }));
            return (StreamDeckAction)objFactory.Invoke(this.ServiceProvider, new object[] { context });
        }
    }
}
