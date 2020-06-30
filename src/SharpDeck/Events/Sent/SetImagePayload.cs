namespace SharpDeck.Events.Sent
{
    using Enums;

    /// <summary>
    /// Provides payload information used to set the image.
    /// </summary>
    public class SetImagePayload : TargetPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetImagePayload"/> class.
        /// </summary>
        /// <param name="base64Image">The base64 image.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the image is set to all states.</param>
        public SetImagePayload(string base64Image, TargetType target = TargetType.Both, int? state = null)
            : base(target, state)
        {
            this.Image = base64Image;
        }

        /// <summary>
        /// Gets or sets the image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). If no image is passed, the image is reset to the default image from the manifest.
        /// </summary>
        public string Image { get; set; }
    }
}
