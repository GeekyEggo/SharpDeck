namespace StreamDeck.Extensions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for <see cref="Delegate"/>.
    /// </summary>
    internal static class DelegateExtensions
    {
        /// <summary>
        /// The expression that represents the <see cref="Task.CompletedTask"/> as a constant.
        /// </summary>
        private static readonly ConstantExpression CompletedTaskExpression = Expression.Constant(Task.CompletedTask);

        /// <summary>
        /// The method info of <see cref="IServiceProvider.GetService(Type)"/>.
        /// </summary>
        private static readonly MethodInfo GetServiceMethodInfo = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService), BindingFlags.Instance | BindingFlags.Public);
        /// <summary>
        /// The expression that represents that <see cref="IServiceProvider"/> parameter.
        /// </summary>
        private static readonly ParameterExpression ServiceProviderExpression = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

        /// <summary>
        /// The expression that represents that <see cref="IStreamDeckConnection"/> parameter.
        /// </summary>
        private static readonly ParameterExpression StreamDeckConnectionExpression = Expression.Parameter(typeof(IStreamDeckConnection), "connection");

        /// <summary>
        /// Compiles the specified <paramref name="delegate"/> to an asynchronous method to be invoked by a <see cref="IStreamDeckConnection"/> event handler.
        /// </summary>
        /// <typeparam name="TArgs">The type of the event arguments.</typeparam>
        /// <param name="delegate">The delegate to compile.</param>
        /// <returns>The compiled delegate.</returns>
        public static Func<IServiceProvider, IStreamDeckConnection, TArgs, Task> Compile<TArgs>(this Delegate @delegate)
        {
            // Build the expressions that represent the parameters, reserving the connection and arguments.
            var tArgsExpression = Expression.Parameter(typeof(TArgs), "tArgs");
            var arguments = @delegate
                .Method
                .GetParameters()
                .Select<ParameterInfo, Expression>(p =>
                {
                    return p.ParameterType switch
                    {
                        Type t when t == typeof(TArgs) => tArgsExpression,
                        Type t when t == typeof(IStreamDeckConnection) => StreamDeckConnectionExpression,
                        _ => Expression.Convert(Expression.Call(ServiceProviderExpression, GetServiceMethodInfo, Expression.Constant(p.ParameterType)), p.ParameterType)
                    };
                })
                .ToArray();

            // When the delegate is not a task, wrap it in a block that returns a completed task.
            Expression body = Expression.Call(Expression.Constant(@delegate.Target), @delegate.Method, arguments);
            if (!typeof(Task).IsAssignableFrom(@delegate.Method.ReturnType))
            {
                body = Expression.Block(body, CompletedTaskExpression);
            }

            // Convert the expression and compile it.
            return Expression.Lambda<Func<IServiceProvider, IStreamDeckConnection, TArgs, Task>>(body, ServiceProviderExpression, StreamDeckConnectionExpression, tArgsExpression)
                .Compile();
        }
    }
}
