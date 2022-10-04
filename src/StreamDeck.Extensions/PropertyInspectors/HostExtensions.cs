namespace StreamDeck.Extensions.PropertyInspectors
{
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.Json.Nodes;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Provides extension methods for <see cref="IHost"/> relating to the property inspector.
    /// </summary>
    public static class HostExtensions
    {
        /// <summary>
        /// Maps the delegate as a data source provider to the property inspector. When invoked, parameters are resolved from the host's <see cref="IServiceProvider" />,
        /// except <see cref="IStreamDeckConnection" /> and <see cref="PartialActionEventArgs{JsonObject}" /> which are propagated directly from the <see cref="IStreamDeckConnection.SendToPlugin" />
        /// event that trigger the request for the data source.
        /// </summary>
        /// <param name="host">The <see cref="IHost" /> to configure.</param>
        /// <param name="event">The name of the data source the <paramref name="action"/> responds to.</param>
        /// <param name="action">The delegate used to get the data source; the result is then returned to the property inspector.</param>
        /// <returns>
        /// The same instance of the <see cref="IHost" /> for chaining.
        /// </returns>
        /// <remarks>
        /// Read more about data sources <see href="https://sdpi-components.dev/docs/helpers/data-source" />.
        /// </remarks>
        public static IHost MapPropertyInspectorDataSource(this IHost host, string @event, Delegate action)
        {
            // todo...
            return host;
        }

        /// <summary>
        /// Compiles the specified <paramref name="delegate" /> to an asynchronous method to be invoked when a <see cref="DataSourcePayload" /> was requests from the property inspector.
        /// </summary>
        /// <param name="delegate">The delegate to compile.</param>
        /// <returns>The compiled delegate.</returns>
        private static Func<IServiceProvider, IStreamDeckConnection, PartialActionEventArgs<JsonObject>, Task<IEnumerable<DataSourceItem>>> Compile(this Delegate @delegate)
        {
            // Validate the return type is a collection of DataSourceItem; this can either be either synchronous or asynchronous.
            var returnType = typeof(Task).IsAssignableFrom(@delegate.Method.ReturnType) && @delegate.Method.ReturnType.GetGenericArguments().FirstOrDefault() is Type taskReturnType
                ? taskReturnType
                : @delegate.Method.ReturnType;

            if (!typeof(IEnumerable<DataSourceItem>).IsAssignableFrom(returnType))
            {
                throw new NotSupportedException($"When mapping a data source to the property inspector, the result type must be a collection of '{typeof(DataSourceItem).FullName}'.");
            }

            // Build the expressions that represent the parameters.
            var argsParameterExpression = Expression.Parameter(typeof(PartialActionEventArgs<JsonObject>), "args");
            var (parameters, serviceProviderParameter, connectionParameter) = @delegate.GetParameterExpressions(argsParameterExpression);

            // Ensure the delegate is a task that returns an IEnumerable<DataSourceItem>.
            Expression body = Expression.Call(Expression.Constant(@delegate.Target), @delegate.Method, parameters);
            if (typeof(Task).IsAssignableFrom(@delegate.Method.ReturnType))
            {
                // The delegate is asynchronous, but might not be IEnumerable, so we cast the result of the task.
                var taskParam = Expression.Parameter(@delegate.Method.ReturnType, "task");
                var taskResult = Expression.Property(taskParam, @delegate.Method.ReturnType.GetProperty(nameof(Task<object>.Result), BindingFlags.Instance | BindingFlags.Public));
                var convertResultToEnumerable = Expression.Convert(taskResult, typeof(IEnumerable<DataSourceItem>));

                // Here, the body becomes a call to the original delegate, followed by a call to .ContinueWith<IEnumerable<DataSourceItem>>(task => task.Result) for the cast.
                body = Expression.Call(body, nameof(Task.ContinueWith), new[] { typeof(IEnumerable<DataSourceItem>) }, Expression.Lambda(convertResultToEnumerable, taskParam));
            }
            else
            {
                // The delegate is not asynchronous, so wrap the result using Task.FromResult<TResult>(TResult).
                body = Expression.Call(typeof(Task), nameof(Task.FromResult), new[] { typeof(IEnumerable<DataSourceItem>) }, body);
            }

            return Expression.Lambda<Func<IServiceProvider, IStreamDeckConnection, PartialActionEventArgs<JsonObject>, Task<IEnumerable<DataSourceItem>>>>(body, serviceProviderParameter, connectionParameter, argsParameterExpression)
                .Compile();
        }
    }
}
