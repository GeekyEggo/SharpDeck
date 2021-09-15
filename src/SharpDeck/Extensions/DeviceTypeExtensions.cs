namespace SharpDeck.Extensions
{
    using System;
    using SharpDeck.Enums;

    /// <summary>
    /// Provides extension methods for <see cref="DeviceType"/>.
    /// </summary>
    public static class DeviceTypeExtensions
    {
        /// <summary>
        /// Gets the full name of the profile based on the device type. When the <paramref name="profile"/> is "MyProfile":
        /// <list type="bullet">
        ///     <item><see cref="DeviceType.StreamDeckMini"/> = "MyProfileMini"</item>
        ///     <item><see cref="DeviceType.StreamDeck"/> = "MyProfile"</item>
        ///     <item><see cref="DeviceType.StreamDeckXL"/> = "MyProfileXL"</item>
        ///     <item><see cref="DeviceType.StreamDeckMobile"/> = "MyProfileMobile"</item>
        ///     <item><see cref="DeviceType.CorsairGKeys"/> = "MyProfileGKeys"</item>
        /// </list>
        /// </summary>
        /// <param name="deviceType">The device type; this instance.</param>
        /// <param name="profile">The [partial] profile name.</param>
        /// <returns>The full name of the profile based on the <paramref name="deviceType"/>.</returns>
        public static string GetProfileName(this DeviceType deviceType, string profile)
        {
            switch (deviceType)
            {
                case DeviceType.StreamDeckMini: return $"{profile}Mini";
                case DeviceType.StreamDeck: return profile;
                case DeviceType.StreamDeckXL: return $"{profile}XL";
                case DeviceType.StreamDeckMobile: return $"{profile}Mobile";
                case DeviceType.CorsairGKeys: return $"{profile}GKeys";
                default: throw new NotSupportedException($"Unable to transform profile name; {deviceType} is not a supported device.");
            }
        }
    }
}
