# Examples

* [Counter](/Counter)
* [Shared Counter](/SharedCounter)

## How it works

1. Each action class must
   1. have a `[StreamDeckAction($name$, $UUID$)]` attribute.
   1. inherit from `StreamDeckAction`, or `StreamDeckAction{TSettings}`.
1. `Program.cs` calls `StreamDeckPlugin.Run()`, this will
   1. discover and register your actions.
   1. connects to the Stream Deck.
   1. keeps your plug-in active.
1. Events from Stream Deck are routed to your action, e.g.
   * [`keyDown`](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#keydown) is handled by via overriding `Task OnKeyDown(ActionEventArgs<KeyPayload> args)`
1. Your plug-in can then communicate with the Stream Deck, e.g.
    * `this.SetTitleAsync("Hello World)`
