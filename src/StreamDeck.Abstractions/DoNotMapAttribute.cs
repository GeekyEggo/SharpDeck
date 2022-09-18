namespace StreamDeck
{
    using System;

    /// <summary>
    /// When present, the action is not automatically mapped as part of <see cref="T:StreamDeck.Extensions.Hosting.GeneratedHostExtensions.MapActions(Microsoft.Extensions.Hosting.IHost)"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class DoNotMapAttribute : Attribute
    {
    }
}
