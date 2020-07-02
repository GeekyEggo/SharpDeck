[![SharpDeck verion on NuGet.org](https://img.shields.io/nuget/v/SharpDeck.svg)](https://www.nuget.org/packages/SharpDeck/) [![Build status](https://github.com/GeekyEggo/SharpDeck/workflows/build/badge.svg)](https://github.com/GeekyEggo/SharpDeck/actions?query=workflow%3Abuild)

# SharpDeck

A lightweight .NET wrapper for creating Stream Deck plugins, using the official Elgato Stream Deck [SDK](https://developer.elgato.com/documentation/stream-deck/sdk/overview).

## ⚡ What does it do?

SharpDeck takes the hassle out of communicating with the Stream Deck SDK, and handles caching and calling your actions! At a glance, SharpDeck handles...

* Connecting to the Stream Deck.
* Registering your custom actions.
* Calling your custom actions when something happens (i.e. button press).

## :package: Examples

* [Counter](/examples/Counter) - Single action counter plugin.
* [Shared Counter](/examples/SharedCounter) - Multiple action plugin with a shared counter and reset.

## :page_facing_up: Getting started

We recommend taking a look at the official [Stream Deck SDK documentation](https://developer.elgato.com/documentation/stream-deck/sdk/overview/). Each plugin has a `manifest.json` file ([example](/examples/SharedCounter/manifest.json)) which tells Stream Deck everything about your plugin, and it's actions.

## :pencil2: Writing your plugin

1. Create a .NET console app (Framework or Core supported).
1. Add your `manifest.json` file.
   * Set the `Copy to Output Directory` property to `Copy Always`.
1. Add SharpDeck `dotnet add package SharpDeck`.
1. Update `Program.cs` to initiate your plugin.
   ```csharp
   public static void Main(string[] args)
   {
       // optional, but recommended
       #if DEBUG
           System.Diagnostics.Debugger.Launch(); 
       #endif
       
       // register actions and connect to the Stream Deck
       SharpDeck.StreamDeckClient.Run();
   }
   ```
1. Create your action as a class. Each action must,
   * Have the attribute `[StreamDeckAction(UUID)]`, with a unique UUID.
   * Inherit from either `StreamDeckAction`, or `StreamDeckAction{TSettings}`.

## :construction: Testing your plugin

1. Navigate to `%APPDATA%\Elgato\StreamDeck\Plugins`, this is where all Stream Deck plugins live.
1. Create a folder for your plugin, typically `com.{author}.{plugin}.sdPlugin`.
1. Copy your files from your `bin\Debug` folder to your plugin folder.
1. Restart Stream Deck, and your plugin should be there! :thumbsup:

## :hammer_and_wrench: Make testing quicker and easier (optional)

Copying your plugin each time can be a bit tedious, but don't worry, there's a few steps to make it easier!

1. Ensure your `Program.cs` calls `System.Diagnostics.Debugger.Launch();`.
1. Add a pre-build task to terminate `StreamDeck.exe` process:
   * `taskkill -f -t -im StreamDeck.exe -fi "status eq running"`
1. Build to your plugin folder directly:
   * e.g. `<OutputPath>$(APPDATA)\Elgato\StreamDeck\Plugins\com.geekyeggo.counter.sdPlugin\</OutputPath>`
1. Create a debug profile in `Properties/launchSettings.json` ([example](/examples/Counter/Properties/launchSettings.json)).
   ```json
   {
     "profiles": {
       "DebugWin": {
         "commandName": "Executable",
         "executablePath": "c:\\windows\\system32\\cmd.exe",
         "commandLineArgs": "/S /C \"start \"title\" /B \"%ProgramW6432%\\Elgato\\StreamDeck\\StreamDeck exe\" \""
       }
      }
   }
   ```
1. Voila, `F5` should now terminate Stream Deck, rebuild your plugin, and then re-launch Stream Deck!


## :rotating_light: Need help?

1. If you can't see your plugin at all:
   * Check your plugin folder contains your plugin's executable. 
   * Check the `manifest.json` file exists, and the `codePath` points to your executable.
1. If you can see your plugin but it isn't working:
   * Check `%APPDATA%\Elgato\StreamDeck\logs` if there are any logs for your plugin.
   * Update your `Program.cs` to call `Debugger.Launch()` ([example](/examples/SharedCounter/Program.cs)) which will allow you to debug your plugin.

*Still can't get it working? No problems, raise an [issue](https://github.com/GeekyEggo/SharpDeck/issues) and we'll try to help!*

## :page_with_curl: License

[The MIT License (MIT)](LICENSE.md)
Stream Deck is a trademark or registered trademark of Elgato Systems.
