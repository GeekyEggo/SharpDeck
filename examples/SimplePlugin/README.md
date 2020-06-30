# Simple Plugin

This example contains a single action, "Counter". The counter action increments on each press, and then displays the number of presses. The presses are also saved to the action's settings, allowing for the count to be re-loaded if the Stream Deck closes.

## Files

```
├── Actions
|   ├── CounterAction.cs
├── Images
│   ├── Counter
|   |   ├── Icon[@2x].png
|   |   └── Image[@2x].png
|   └── Plugin
|       ├── CategoryIcon[@2x].png
|       └── Icon[@2x].png
├── manifest.json
└── Program.cs
```

| File | Description |
| --- | --- |
| `manifest.json` | Contains information about your plug-in, and tells Stream Deck how to run it. ([read more](https://developer.elgato.com/documentation/stream-deck/sdk/manifest/))
| `Program.cs` | Main entry point, this is the first file that runs when your plug-in starts |
| `CounterAction.cs` | The Counter action; when the button is pressed, methods in here are called. |
| `Images/Counter/**` | Images relating to the action; these are set in the `manifest.json` |
| `Images/Plugin/**` | Images relating to the plugin; these are set in the `manifest.json` |

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
