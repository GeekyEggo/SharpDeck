namespace SharpDeck.Events.Received
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides an enumeration of controllers.
    /// </summary>
    public enum Controller
    {
        /// <summary>
        /// The controller is an encoder, i.e. a dial / touchpad.
        /// </summary>
        [EnumMember(Value = "Encoder")]
        Encoder,

        /// <summary>
        /// The controller is a keypad, i.e. a button.
        /// </summary>
        [EnumMember(Value = "Keypad")]
        Keypad
    }
}
