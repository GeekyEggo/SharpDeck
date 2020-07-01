# Shared Counter Plugin

This example contains a two action, "Counter" and "Reset". The counter action increments a shared count on each press, which is displayed on all instances of a "Counter" action. The "Reset" action can then reset the count back to 0.

## Files

```
├── Actions
|   ├── CounterAction.cs
|   └── ResetAction.cs
├── Images
│   ├── Counter
|   |   ├── Icon[@2x].png
|   |   └── Image[@2x].png
|   ├── Plugin
|   |   ├── CategoryIcon[@2x].png
|   |   └── Icon[@2x].png
|   └── Reset
|       ├── CategoryIcon[@2x].png
|       └── Icon[@2x].png
├── Count.cs
├── manifest.json
└── Program.cs
```

| File | Description |
| --- | --- |
| `Count.cs` | The singleton that manages the shared count. |
| `manifest.json` | Contains information about your plug-in, and tells Stream Deck how to run it. ([read more](https://developer.elgato.com/documentation/stream-deck/sdk/manifest/)). |
| `Program.cs` | Main entry point, this is the first file that runs when your plug-in starts. |
| `CounterAction.cs` | The Counter action; increments the shared count. |
| `ResetAction.cs` | The Reset action; resets the shared count to 0. |
| `Images/**` | Images relating to the plug-in and its actions; these are set in the `manifest.json`. |
