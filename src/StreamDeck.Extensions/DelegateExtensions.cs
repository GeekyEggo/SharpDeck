namespace StreamDeck.Extensions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using StreamDeck;

    /// <summary>
    /// Provides extension methods for <see cref="Delegate"/>.
    /// </summary>
    internal static class DelegateExtensions
    {
        /// <summary>
        /// The method info of <see cref="IServiceProvider.GetService(Type)"/>.
        /// </summary>
        private static readonly MethodInfo GetServiceMethodInfo = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService), BindingFlags.Instance | BindingFlags.Public)!;

        /// <summary>
        /// The expression that represents that <see cref="IServiceProvider"/> parameter.
        /// </summary>
        private static readonly ParameterExpression ServiceProviderParameter = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

        /// <summary>
        /// The expression that represents that <see cref="IStreamDeckConnection"/> parameter.
        /// </summary>
        private static readonly ParameterExpression StreamDeckConnectionParameter = Expression.Parameter(typeof(IStreamDeckConnection), "connection");

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/> from this <see cref="Delegate"/>.
        /// </summary>
        /// <param name="delegate">This instance whose <see cref="ParameterInfo"/> should be converted to <see cref="ParameterExpression"/>.</param>
        /// <param name="argsParameterExpression">The <see cref="ParameterExpression"/> that represents the arguments.</param>
        /// <returns>The collection of <see cref="ParameterExpression"/> associated with this <see cref="Delegate"/>, and the parameters supplied to them.</returns>
        public static (Expression[], ParameterExpression ServiceProvider, ParameterExpression Connection) GetParameterExpressions(this Delegate @delegate, Expression argsParameterExpression)
        {
            return (
                @delegate.Method
                    .GetParameters()
                    .Select(p =>
                        p.ParameterType switch
                        {
                            Type t when t == argsParameterExpression.Type => argsParameterExpression,
                            Type t when t == typeof(IStreamDeckConnection) => StreamDeckConnectionParameter,
                            _ => Expression.Convert(Expression.Call(ServiceProviderParameter, GetServiceMethodInfo, Expression.Constant(p.ParameterType)), p.ParameterType)
                        })
                    .ToArray(),
                ServiceProviderParameter,
                StreamDeckConnectionParameter
            );
        }
    }
}
