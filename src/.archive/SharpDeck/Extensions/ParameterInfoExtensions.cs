namespace SharpDeck.Extensions
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for <see cref="ParameterInfo"/>.
    /// </summary>
    internal static class ParameterInfoExtensions
    {
        /// <summary>
        /// Gets the default value for the <see cref="ParameterInfo.ParameterType"/>.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The default value.</returns>
        internal static object GetDefaultValue(this ParameterInfo parameter)
        {
            return parameter.ParameterType.IsValueType
                ? Activator.CreateInstance(parameter.ParameterType)
                : null;
        }
    }
}
