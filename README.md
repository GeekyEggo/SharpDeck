# SharpDeck
A lightweight .NET wrapper for the official Elgato Stream Deck [SDK](https://developer.elgato.com/documentation/stream-deck/sdk/overview), designed to assist with creating plugins that can be distributed on the Stream Deck store.
## Example
```csharp
static void Main(string[] args)
{
    using (var client = new StreamDeckClient(args))
    {
        client.KeyDown += (_, e) => client.SetTitleAsync(e.Context, "Hello world");
        client.Start(); // continuously listens until the connection closes
    }
}
```

## Registering Actions (Optional)
Optionally, it is possible to register `StreamDeckAction` and have the client handle the context automatically.
```csharp
client.RegisterAction<CounterAction>("com.sharpdeck.testplugin.counter");
```
```csharp
class CounterAction : StreamDeckAction
{
    protected override async void OnKeyDown(ActionEventArgs<KeyPayload> args)
        => this.SetTitleAsync("Hello world");
}
```

## Contributing

Having a problem, or got an idea? Let me know!

![Twitter Logo](docs/icons/Twitter.png) / [@GeekyEggo](https://twitter.com/GeekyEggo)

https://github.com/geekyeggo/Sharpdeck/issues

## License

[The MIT License (MIT)](LICENSE.md)

Stream Deck is a trademark or registered trademark of Elgato Systems.