# Examples

* [Counter](/examples/Counter) - Single action counter plugin.
* [Shared Counter](/examples/SharedCounter) - Multiple action plugin with a shared counter and reset.

## How it works

1. Each action class must
   1. have a `[StreamDeckAction($UUID$)]` attribute.
   1. inherit from `StreamDeckAction`, or `StreamDeckAction{TSettings}`.
1. `Program.cs` calls `StreamDeckPlugin.Run()`, this will
   1. discover and register your actions.
   1. connects to the Stream Deck.
   1. keeps your plugin active.
1. Events from Stream Deck are routed to your action, e.g.
   * [`keyDown`](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#keydown) is handled by via overriding `Task OnKeyDown(ActionEventArgs<KeyPayload> args)`
1. Your plugin can then communicate with the Stream Deck, e.g.
    * `this.SetTitleAsync("Hello World)`
