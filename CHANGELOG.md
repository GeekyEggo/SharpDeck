# Change Log 
## 4.0.0
* Added automatic manifest generation!
* Added support for initializing actions with their `AppearancePayload`.
* Added action context when raising `StreamDeckClient.Error` *(where possible)*.
* Added support for `FontFamilyType` and `FontStyleType`.
* Improved deadlock prevention for all WebSocket requests.
* Fixed `DeviceType`, `PlatformType`, and `TitleAlignmentType`.

#### Breaking Changes
* Removed `StreamDeckAction.Initialized` event, please use `StreamDeckAction.WillAppear`
* Removed `StreamDeckAction<TSettings>.Settings` to prevent misuse.

## 3.0.0
* Added support for SDK 4.1
  * [didReceiveGlobalSettings](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#didreceiveglobalsettings)
  * [didReceiveSettings](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#didreceivesettings)
  * [getGlobalSettings](https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setglobalsettings)
  * [getSettings](https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#getsettings)
  * [logMessage](https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#logmessage)
  * [propertyInspectorDidAppear](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#propertyinspectordidappear)
  * [propertyInspectorDidDisappear](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#propertyinspectordiddisappear)
  * [setGlobalSettings](https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setglobalsettings)
 * Added support for SDK 4.3
   * [systemDidWakeUp](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#systemdidwakeup)
   * Added device name to registration info.
* Added `StreamDeckAction{TSettings}` base class.
* Added `RequestId` to property inspector methods, allowing for responses to be identified more easily (e.g. with promises).
* Added `StreamDeckXL` and `StreamDeckMobile` to `DeviceType`.
* Fixed JSON serialization casing inconsistencies when using JObject (specifically action settings).
* Fixed `openUrl` event.

#### Breaking Changes
* Relocated to `SharpDeck.Events.StreamDeckAction` to top level `SharpDeck.StreamDeckAction`.
* Re-aligned namespaces of events (received) / messages (sent) to match Elgato SDK terminology.

## 2.0.0
* Added `PropertyInspectorMethodAttribute` decorator for interacting with Property Inspector.
* Updated `StreamDeckAction` overrides to require a task.
* Fixed potential async/await issue within `StreamDeckClient`.
* Added unit testing, including AppVeyor integration.
* Streamlined publishing to NuGet with npm package scripts.

## 1.0.3
* Fixed an issue with `sendToPlugin` event not triggering.
* Removed unnecessary parameters when calling `SendToPropertyInspectorAsync`.

## 1.0.2
* Added missing event: `sendToPlugin`.
* Centralised all event related models to the `SharpDeck.Events` namespace.
* `StreamDeckClient.RegisterAction` now supports a value factory.
* Improved dependency injection capabilities.

## 1.0.1
* Added `StreamDeckClient`, allowing for connections to an Elgato Stream Deck.
* Added event listeners ([ref: "Events Received"](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/)).
* Added messaging ([ref: "Events Sent"](https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/)).
* Added support for registering `StreamDeckAction`.