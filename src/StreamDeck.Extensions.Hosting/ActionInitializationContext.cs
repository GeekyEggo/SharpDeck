namespace StreamDeck
{
    using Microsoft.Extensions.Logging;
    using StreamDeck.Events;

    /// <summary>
    /// Provides initialization information for a <see cref="StreamDeckAction"/>.
    /// </summary>
    public struct ActionInitializationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInitializationContext"/> struct.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="actionInfo">The <see cref="ActionEventArgs{ActionPayload}"/> instance containing the event data.</param>
        /// <param name="logger">The logger to be used by the action.</param>
        internal ActionInitializationContext(IStreamDeckConnection connection, ActionEventArgs<ActionPayload> actionInfo, ILogger? logger = null)
        {
            this.ActionInfo = actionInfo;
            this.Connection = connection;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the action information.
        /// </summary>
        public ActionEventArgs<ActionPayload> ActionInfo { get; }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        public IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger? Logger { get; }
    }
}
