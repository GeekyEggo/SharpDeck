# Change Log 

## 5.0.2

#### ⭐ Added

* New `IStreamDeckConnection.GetGlobalSettingsAsync<T>()` now returns the global settings!
* New `StreamDeckPlugin.OnRegistered(Func<IStreamDeckConnection>)` delegate; called after the plugin is registered.
* New `StreamDeckPlugin.Run()` method; used when using `StreamDeckPlugin.Create`.
* `StreamDeckActionAttribute` now has a simplified constructor that accepts `UUID`.
* Support for SDK 4.8
  * Added `state` parameter to `setImage`.
  * Added `state` parameter to `setTitle`.
* Support for SDK 4.7
  * Added `kESDSDKDeviceType_CorsairGKeys` device type.
* All new sample plugins.

## 5.0.1

#### 🐞 Fixed 

* Fixed an issue with `StreamDeckClient.Run()` not blocking the main thread.

## 5.0.0

#### 🚨 Breaking

* Removed `StreamDeckClient`; replaced with `IStreamDeckConnection`.
* Updated `StreamDeckAction.StreamDeck` to be `IStreamDeckConnection`.

#### ⭐ Added

* Greatly simplfied starting a plugin.
  * `StreamDeckPlugin.Run()`
  * `StreamDeckPlugin.RunAsync()`
* Added basic unit tests.
* Added GitHub workflows.
* Added `StreamDeckAction.OnInit` virtual.

#### ♻ Changed

* Actions with `StreamDeckAttribute` are automatically registered.
* Complete re-write of action caching.
* Complete re-write of event routing.

#### 🐞 Fixed

* Fixed an issue with incorrect actions being invoked.

## 4.0.1/2

#### ♻ Changed

* Updated deployment to NuGet.

## 4.0.0

#### 🚨 Breaking

* Removed `StreamDeckAction.Initialized` event, please use `StreamDeckAction.WillAppear`
* Removed `StreamDeckAction<TSettings>.Settings` to prevent misuse.

#### ⭐ Added

* Added automatic manifest generation!
* Added support for initializing actions with their `AppearancePayload`.
* Added action context when raising `StreamDeckClient.Error` *(where possible)*.
* Added support for `FontFamilyType` and `FontStyleType`.
* Improved deadlock prevention for all WebSocket requests.

#### 🐞 Fixed

* Fixed `DeviceType`, `PlatformType`, and `TitleAlignmentType`.

## 3.0.0

#### 🚨 Breaking

* Relocated to `SharpDeck.Events.StreamDeckAction` to top level `SharpDeck.StreamDeckAction`.
* Re-aligned namespaces of events (received) / messages (sent) to match Elgato SDK terminology.

#### ⭐ Added

* Support for SDK 4.1
  * [didReceiveGlobalSettings](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#didreceiveglobalsettings)
  * [didReceiveSettings](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#didreceivesettings)
  * [getGlobalSettings](https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setglobalsettings)
  * [getSettings](https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#getsettings)
  * [logMessage](https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#logmessage)
  * [propertyInspectorDidAppear](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#propertyinspectordidappear)
  * [propertyInspectorDidDisappear](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#propertyinspectordiddisappear)
  * [setGlobalSettings](https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setglobalsettings)
 * Support for SDK 4.3
   * [systemDidWakeUp](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#systemdidwakeup)
   * Added device name to registration info.
* `StreamDeckAction{TSettings}` base class.
* `RequestId` to property inspector methods, allowing for responses to be identified more easily (e.g. with promises).
* `StreamDeckXL` and `StreamDeckMobile` to `DeviceType`.

#### 🐞 Fixed

* Fixed JSON serialization casing inconsistencies when using JObject (specifically action settings).
* Fixed `openUrl` event.

## 2.0.0

#### ⭐ Added

* `PropertyInspectorMethodAttribute` decorator for interacting with Property Inspector.
* Added unit testing, including AppVeyor integration.

#### ♻ Changed

* Updated `StreamDeckAction` overrides to require a task.
* Streamlined publishing to NuGet with npm package scripts.

#### 🐞 Fixed

* Fixed potential async/await issue within `StreamDeckClient`.

## 1.0.3

#### ♻ Changed

* Removed unnecessary parameters when calling `SendToPropertyInspectorAsync`.

#### 🐞 Fixed

* Fixed an issue with `sendToPlugin` event not triggering.

## 1.0.2

#### ⭐ Added

* Added missing event: `sendToPlugin`.

#### ♻ Changded

* `StreamDeckClient.RegisterAction` now supports a value factory.
* Centralised all event related models to the `SharpDeck.Events` namespace.
* Improved dependency injection capabilities.

## 1.0.1

#### ⭐ Added

* `StreamDeckClient`, allowing for connections to an Elgato Stream Deck.
* Event listeners ([ref: "Events Received"](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/)).
* Messaging ([ref: "Events Sent"](https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/)).
* Support for registering `StreamDeckAction`.
