namespace SharpDeck.Plugin
{
    using System.Diagnostics;

    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            Debugger.Launch();
#endif
            
            using (var client = new StreamDeckClient(args))
            {
                //client.RegisterAction<CounterAction>("com.sharpdeck.testplugin.counter");

                client.Start();
                client.KeyDown += (s, e) =>
                {
                    client.SetTitleAsync(e.Context, "ON");
                };

                client.Wait();
                //var settings = new ActionSettings();
                //client.WillAppear += (async (_, e) =>
                //{
                //    //client.SetTitleAsync
                //    settings = e.Payload.GetSettings<ActionSettings>();
                //    if (settings == null)
                //    {
                //        settings = new ActionSettings();
                //        await client.SetSettingsAsync(e.Context, settings);
                //    }
                //
                //    await client.SetTitleAsync(e.Context, settings.Count.ToString());
                //});
                //
                //client.KeyDown += (async (_, e) =>
                //{
                //    settings.Count++;
                //    await client.SetTitleAsync(e.Context, settings.Count.ToString());
                //    await client.SetSettingsAsync(e.Context, settings);
                //});

            }
        }
    }

    public class ActionSettings
    {
        public int Count { get; set; } = 0;
    }
}
