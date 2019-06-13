namespace SharpDeck.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides an enumeration of platforms.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PlatformType
    {
        /// <summary>
        /// Defines the Mac platform: kESDSDKApplicationInfoPlatformMac.
        /// </summary>
        [EnumMember(Value = "mac")]
        Mac = 0,

        /// <summary>
        /// Defines the Windows platform: kESDSDKApplicationInfoPlatformWindows.
        /// </summary>
        [EnumMember(Value = "windows")]
        Windows = 1
    }
}
