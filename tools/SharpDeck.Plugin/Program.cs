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
                client.Start();

                var settings = new ActionSettings();
                client.WillAppear += (async (_, e) =>
                {
                    settings = e.Payload.GetSettings<ActionSettings>();
                    if (settings == null)
                    {
                        settings = new ActionSettings();
                        await client.SetSettingsAsync(e.Context, settings);
                    }

                    await client.SetTitleAsync(e.Context, settings.Count.ToString());
                });

                client.KeyDown += (async (_, e) =>
                {
                    settings.Count++;
                    await client.SetTitleAsync(e.Context, settings.Count.ToString());
                    await client.SetSettingsAsync(e.Context, settings);
                });

                client.Wait();
            }
        }
    }

    public class ActionSettings
    {
        public int Count { get; set; } = 0;
    }
}
