# Counter Plugin

This example contains a single action, "Counter". The counter action increments on each press, and then displays the number of presses. The presses are also saved to the action's settings, allowing for the count to be re-loaded if the Stream Deck closes.

## Files

```
├── Actions
|   ├── CounterAction.cs
|   └── CounterSettings.cs
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
| `Actions/CounterAction.cs` | The Counter action; when pressed the count increments. |
| `Actions/CounterSettings.cs` | The settings for the Counter action. |
| `Images/**` | Images relating to the plugin and its actions; these are set in the `manifest.json`. |
| `manifest.json` | Contains information about your plugin, and tells Stream Deck how to run it. ([read more](https://developer.elgato.com/documentation/stream-deck/sdk/manifest/)). |
| `Program.cs` | Main entry point, this is the first file that runs when your plugin starts. |
