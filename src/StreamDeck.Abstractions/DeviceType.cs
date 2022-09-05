namespace StreamDeck
{
    /// <summary>
    /// Provides an enumeration of devices.
    /// </summary>
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    enum DeviceType
    {
        /// <summary>
        /// Stream Deck (kESDSDKDeviceType_StreamDeck).
        /// </summary>
        StreamDeck = 0,

        /// <summary>
        /// Stream Deck Mini (kESDSDKDeviceType_StreamDeckMini).
        /// </summary>
        StreamDeckMini = 1,

        /// <summary>
        /// Stream Deck XL (kESDSDKDeviceType_StreamDeckXL).
        /// </summary>
        StreamDeckXL = 2,

        /// <summary>
        /// Stream Deck Mobile (kESDSDKDeviceType_StreamDeckMobile).
        /// </summary>
        StreamDeckMobile = 3,

        /// <summary>
        /// Corsair G-Key compatible keyboard (kESDSDKDeviceType_CorsairGKeys).
        /// </summary>
        CorsairGKeys = 4,

        /// <summary>
        /// Stream Deck Pedal (kESDSDKDeviceType_StreamDeckPedal).
        /// </summary>
        StreamDeckPedal = 5,

        /// <summary>
        /// Corsair Voyager laptop (kESDSDKDeviceType_CorsairVoyager).
        /// </summary>
        CorsairVoyager = 6
    }
}
