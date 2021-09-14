namespace SharpDeck.DependencyInjection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Provides an implementation of <see cref="IActivator"/> that utilizes a <see cref="IServiceProvider"/>.
    /// </summary>
    internal class SelfContainedServiceProviderActivator : IActivator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfContainedServiceProviderActivator"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public SelfContainedServiceProviderActivator(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the serivce provider.
        /// </summary>
        private IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Instantiates a new instance of <paramref name="type" />.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The instantiated instance.</returns>
        public object CreateInstance(Type type)
            => ActivatorUtilities.CreateInstance(this.ServiceProvider, type);
    }
}
