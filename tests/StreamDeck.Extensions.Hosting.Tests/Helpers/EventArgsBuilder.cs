namespace StreamDeck.Tests.Helpers
{
    using System.Text.Json.Nodes;
    using StreamDeck.Events;

    /// <summary>
    /// Provides helper methods for creating event args.
    /// </summary>
    internal static class EventArgsBuilder
    {
        /// <summary>
        /// The mock <see cref="IActionContext.Action"/>.
        /// </summary>
        public const string ACTION_UUID = "com.tests.plugin.action";

        /// <summary>
        /// The mock <see cref="IActionContext.Context"/>.
        /// </summary>
        public const string CONTEXT = "ABC123";

        /// <summary>
        /// Creates a new instance of a mock <see cref="ActionEventArgs{ActionPayload}"/>.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="context">The context.</param>
        /// <returns>The event args.</returns>
        internal static ActionEventArgs<ActionPayload> CreateActionEventArgs(string action = ACTION_UUID, string context = CONTEXT)
        {
            var payload = new ActionPayload(
                new Coordinates(column: 1, row: 1),
                isInMultiAction: false,
                settings: new JsonObject());

            return new ActionEventArgs<ActionPayload>(
                @event: "event",
                payload,
                action: action,
                context: context,
                device: "XYZ789");
        }

        /// <summary>
        /// Creates a new instance of a mock <see cref="ActionEventArgs{KeyPayload}"/>.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="context">The context.</param>
        /// <returns>The event args.</returns>
        internal static ActionEventArgs<KeyPayload> CreateKeyEventArgs(string action = ACTION_UUID, string context = CONTEXT)
        {
            var payload = new KeyPayload(
                new Coordinates(column: 1, row: 1),
                isInMultiAction: false,
                settings: new JsonObject());

            return new ActionEventArgs<KeyPayload>(
                @event: "event",
                payload,
                action: action,
                context: context,
                device: "XYZ789");
        }

        /// <summary>
        /// Creates a new instance of a mock <see cref="ActionEventArgs{TitlePayload}"/>.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="context">The context.</param>
        /// <returns>The event args.</returns>
        internal static ActionEventArgs<TitlePayload> CreateTitleEventArgs(string action = ACTION_UUID, string context = CONTEXT)
        {
            var payload = new TitlePayload(
                title: "Hello world",
                titleParameters: new TitleParameters("FontFamily", 1, "Bold", true, true, "middle", "#000000"),
                coordinates: new Coordinates(column: 1, row: 1),
                isInMultiAction: false,
                settings: new JsonObject());

            return new ActionEventArgs<TitlePayload>(
                @event: "event",
                payload,
                action: action,
                context: context,
                device: "XYZ789");
        }
    }
}
