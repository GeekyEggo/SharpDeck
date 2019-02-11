namespace SharpDeck.Extensions
{
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods 
    /// </summary>
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Invokes the method information asynchronously.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="obj">The object.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The result of invoking the method information, after awaiting the result.</returns>
        public static async Task<object> InvokeAsync(this MethodInfo methodInfo, object obj, object[] parameters)
        {
            var result = methodInfo.Invoke(obj, parameters);
            if (result is Task task)
            {
                await task.ConfigureAwait(false);
                return task;
            }

            return result;
        }
    }
}
