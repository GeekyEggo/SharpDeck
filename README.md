[![SharpDeck verion on NuGet.org](https://img.shields.io/nuget/v/SharpDeck.svg)](https://www.nuget.org/packages/SharpDeck/) ![Build status on Appveyor](https://ci.appveyor.com/api/projects/status/fev4i9a61a7ylhyq/branch/master?svg=true)

# SharpDeck

A lightweight .NET wrapper for creating Stream Deck plugins, using the official Elgato Stream Deck [SDK](https://developer.elgato.com/documentation/stream-deck/sdk/overview).

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

[![Twitter Logo](https://github.com/GeekyEggo/SharpDeck/raw/master/docs/icons/Twitter.png)@GeekyEggo](https://twitter.com/GeekyEggo)

https://github.com/GeekyEggo/SharpDeck/issues

## License

[The MIT License (MIT)](LICENSE.md)

Stream Deck is a trademark or registered trademark of Elgato Systems.