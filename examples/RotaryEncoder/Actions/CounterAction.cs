namespace DialCounter.Actions;

using Newtonsoft.Json.Linq;
using SharpDeck;
using SharpDeck.Events.Received;
using SharpDeck.Layouts;
using System.Threading.Tasks;

/// <summary>
/// The dial counter action; displays the count in A1 layout which
/// - increases/decreases on rotating the dial
/// - reset on pressing the dial
/// - increases on pressing the LED screen
/// - reset on holding the LED screen
/// </summary>
[StreamDeckAction("com.geekyeggo.dialcounter.counter")]
public class CounterAction : StreamDeckAction<CounterSettings>
{
    /// <summary>
    /// Occurs when <see cref="IStreamDeckConnection.WillAppear" /> is received for this instance.
    /// </summary>
    /// <param name="args">The <see cref="ActionEventArgs{T}" /> instance containing the event data.</param>
    /// <returns>The task of handling the event.</returns>
    protected override Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
    {
        // get the current, and set the layout
        var settings = args.Payload.GetSettings<CounterSettings>();
        return this.UpdateCountAsync(settings.Count);
    }

    protected override async Task OnSendToPlugin(ActionEventArgs<JObject> args)
    {
        switch (args.Payload["sdpi_collection"]?["key"]?.ToString())
        {
            case "tickmultiplier":
                if (int.TryParse(args.Payload["sdpi_collection"]?["value"]?.ToString(), out var value))
                {
                    // modify the multiplier
                    var settings = await this.GetSettingsAsync<CounterSettings>();
                    settings.TickMultiplier = value;

                    // save the settings
                    await this.SetSettingsAsync(settings);
                }
                break;
        }
        
    }

    /// <summary>
    /// Occurs when <see cref="IStreamDeckConnection.DialRotate" /> is received for this instance.
    /// </summary>
    /// <param name="args">The <see cref="ActionEventArgs{T}" /> instance containing the event data.</param>
    /// <returns>The task of handling the event.</returns>
    protected override async Task OnDialRotate(ActionEventArgs<DialRotatePayload> args)
    {
        // modify the count
        var settings = args.Payload.GetSettings<CounterSettings>();
        settings.Count += args.Payload.Ticks * settings.TickMultiplier;

        // save the settings, and set the layout
        await this.SetSettingsAsync(settings);
        await this.UpdateCountAsync(settings.Count);
    }

    /// <summary>
    /// Occurs when <see cref="IStreamDeckConnection.DialPress" /> is received for this instance.
    /// </summary>
    /// <param name="args">The <see cref="ActionEventArgs{T}" /> instance containing the event data.</param>
    /// <returns>The task of handling the event.</returns>
    protected override async Task OnDialPress(ActionEventArgs<DialPayload> args)
    {
        // reset the count
        var settings = args.Payload.GetSettings<CounterSettings>();
        settings.Count = 0;

        // save the settings, and set the layout
        await this.SetSettingsAsync(settings);
        await this.UpdateCountAsync(settings.Count);
    }

    /// <summary>
    /// Occurs when <see cref="IStreamDeckConnection.TouchTap" /> is received for this instance.
    /// </summary>
    /// <param name="args">The <see cref="ActionEventArgs{T}" /> instance containing the event data.</param>
    /// <returns>The task of handling the event.</returns>
    protected override async Task OnTouchTap(ActionEventArgs<TouchTapPayload> args)
    {
        var settings = args.Payload.GetSettings<CounterSettings>();
        if (args.Payload.Hold)
        {
            // reset the count
            settings.Count = 0;
        }
        else
        {
            // increase the count
            settings.Count += 1;
        }

        // save the settings, and set the layout
        await this.SetSettingsAsync(settings);
        await this.UpdateCountAsync(settings.Count);
    }

    private Task UpdateCountAsync(int count)
    {
        return this.SetFeedbackAsync(new LayoutA1()
        {
            Value = count.ToString()
        });
    }
}
