namespace SharpDeck.Events.Sent
{
    using Enums;

    /// <summary>
    /// Provides information about a payload that can be targetted.
    /// </summary>
    public class TargetPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetPayload"/> class.
        /// </summary>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        public TargetPayload(TargetType target = TargetType.Both)
        {
            this.Target = target;
        }

        /// <summary>
        /// Gets or sets a value indicating which display should be updated.
        /// </summary>
        public TargetType Target { get; set; }
    }
}
