namespace SimplePlugin.Actions
{
    using System.Threading.Tasks;
    using SharpDeck;
    using SharpDeck.Events.Received;
    using SharpDeck.Manifest;

    [StreamDeckAction("Counter", "com.geekyeggo.simpleplugin.counter")]
    public class CounterActions : StreamDeckAction<CounterActions.Settings>
    {
        protected override Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            // increment the count
            var settings = args.Payload.GetSettings<CounterActions.Settings>();
            settings.Count++;

            // save the settings, and set the title
            this.SetSettingsAsync(settings);
            return this.SetTitleAsync(settings.Count.ToString());
        }

        protected override Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            // get the current, and set the title
            var settings = args.Payload.GetSettings<CounterActions.Settings>();
            return this.SetTitleAsync(settings.Count.ToString());
        }

        public class Settings
        {
            public int Count { get; set; } = 0;
        }
    }
}
