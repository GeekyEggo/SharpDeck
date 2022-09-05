#if NET6_0_OR_GREATER
namespace StreamDeck
{
    using System.Runtime.Versioning;

    /// <summary>
    /// Provides a <see cref="UUID"/> used to identify the Stream Deck action.
    /// </summary>
    [RequiresPreviewFeatures]
    public interface IStreamDeckActionIdentifier
    {
        /// <summary>
        /// Gets the the unique identifier of the action; see <see href="https://developer.elgato.com/documentation/stream-deck/sdk/manifest/#actions"/>.
        /// </summary>
        static abstract string UUID { get; }
    }
}
#endif
