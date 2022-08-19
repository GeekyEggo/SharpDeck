namespace StreamDeck.Events
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides an enumeration of devices.
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// Defines a Stream Deck: kESDSDKDeviceType_StreamDeck.
        /// </summary>
        [EnumMember(Value = "kESDSDKDeviceType_StreamDeck")]
        StreamDeck = 0,

        /// <summary>
        /// Defines a Stream Deck Mini: kESDSDKDeviceType_StreamDeckMini.
        /// </summary>
        [EnumMember(Value = "kESDSDKDeviceType_StreamDeckMini")]
        StreamDeckMini = 1,

        /// <summary>
        /// Defines a Stream Deck XL: kESDSDKDeviceType_StreamDeckXL.
        /// </summary>
        [EnumMember(Value = "kESDSDKDeviceType_StreamDeckXL")]
        StreamDeckXL = 2,

        /// <summary>
        /// Defines a Stream Deck Mobile: kESDSDKDeviceType_StreamDeckMobile.
        /// </summary>
        [EnumMember(Value = "kESDSDKDeviceType_StreamDeckMobile")]
        StreamDeckMobile = 3,

        /// <summary>
        /// Defines a Corsair G-Key compatible keyboard: kESDSDKDeviceType_CorsairGKeys.
        /// </summary>
        [EnumMember(Value = "kESDSDKDeviceType_CorsairGKeys")]
        CorsairGKeys = 4
    }
}
