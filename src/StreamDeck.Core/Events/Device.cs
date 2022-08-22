namespace StreamDeck.Events
{
    /// <summary>
    /// Provides an enumeration of devices.
    /// </summary>
    public enum Device
    {
        /// <summary>
        /// Defines a Stream Deck: kESDSDKDeviceType_StreamDeck.
        /// </summary>
        StreamDeck = 0,

        /// <summary>
        /// Defines a Stream Deck Mini: kESDSDKDeviceType_StreamDeckMini.
        /// </summary>
        StreamDeckMini = 1,

        /// <summary>
        /// Defines a Stream Deck XL: kESDSDKDeviceType_StreamDeckXL.
        /// </summary>
        StreamDeckXL = 2,

        /// <summary>
        /// Defines a Stream Deck Mobile: kESDSDKDeviceType_StreamDeckMobile.
        /// </summary>
        StreamDeckMobile = 3,

        /// <summary>
        /// Defines a Corsair G-Key compatible keyboard: kESDSDKDeviceType_CorsairGKeys.
        /// </summary>
        CorsairGKeys = 4
    }
}
