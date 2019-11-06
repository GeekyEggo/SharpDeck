[![SharpDeck verion on NuGet.org](https://img.shields.io/nuget/v/SharpDeck.svg)](https://www.nuget.org/packages/SharpDeck/)

![](https://github.com/geekyeggo/sharpdeck/workflows/.github/workflows/release.yml/badge.svg)
![](https://github.com/geekyeggo/sharpdeck/workflows/release/badge.svg)
![](https://github.com/geekyeggo/sharpdeck/workflows/release/release/badge.svg)

![](https://github.com/geekyeggo/sharpdeck/workflows/.github/workflows/main.yml/badge.svg)
![](https://github.com/geekyeggo/sharpdeck/workflows/build/build/badge.svg)
![](https://github.com/geekyeggo/sharpdeck/workflows/build/badge.svg)

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

## Interacting with the Property Inspector

It is possible to interact with the Property Inspector by _exposing_ methods from your action, using the  `PropertyInspectorMethodAttribute` attribute, similiar to MVC .NET decorators. The attribute supports tasks, and also allows for results.

### Example
```csharp
// Action.cs
[PropertyInspectorMethod("load")]
protected void OnPropertyInspectorLoad()
  => // ... execute code
```
```js
// Property Inspector js file
websocket.send(JSON.stringify({
    "action": actionUUID,
    "event": "sendToPlugin",
    "context": uuid,
    "payload": {
        "event": "load"
    }
}));
```

### Example 2 (parameters and results)
```csharp
// Action.cs
[PropertyInspectorMethod("load")]
protected Task<ActionResponse> OnPropertyInspectorLoad(ActionPayload args)
{
    // ... execute code, with access to args
    return this.GetSessionKeyAsync(args.UserId);
}

// ActionPayload.cs
public class ActionPayload : SharpDeck.PropertyInspectors.PropertyInspectorPayload
{
    public string UserId { get; set; }
}

// ActionResponse.cs
public class ActionResponse : SharpDeck.PropertyInspectors.PropertyInspectorPayload
{
    public string SessionKey { get; set; }
}
```
```js
// Property Inspector js file
websocket.send(JSON.stringify({
    "action": actionUUID,
    "event": "sendToPlugin",
    "context": uuid,
    "payload": {
        "event": "load",
        "userId": "1337"
    }
}));

/*
upon Action.OnPropertyInspectorLoad completing, websocket will receive a message with the payload data:
{
    "action": actionUUID,
    "event": "sendToPropertyInspector",
    "context": context,
    "payload": {
        "event": "load",
        "sessionKey": <<sessionKey>>
    }
}
*/
```

## Contributing

Having a problem, or got an idea? Let me know!

[![Twitter Logo](https://github.com/GeekyEggo/SharpDeck/raw/master/docs/icons/Twitter.png)@GeekyEggo](https://twitter.com/GeekyEggo)

https://github.com/GeekyEggo/SharpDeck/issues

## License

[The MIT License (MIT)](LICENSE.md)

Stream Deck is a trademark or registered trademark of Elgato Systems.