using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using StreamDeck.Extensions.Hosting;

#if DEBUG
System.Diagnostics.Debugger.Launch();
#endif

StreamDeckPlugin.Create()
    .ConfigurePlugin(conn =>
    {
        var counts = new ConcurrentDictionary<string, int>();

        // Show the count for the action on willAppear, and increment the count on keyDown.
        conn.WillAppear += (_, e) => conn.SetTitleAsync(e.Context, counts.GetOrAdd(e.Context, 0).ToString());
        conn.KeyDown += (_, e) =>
        {
            var count = counts.AddOrUpdate(e.Context, 1, (_, value) => ++value);
            conn.SetTitleAsync(e.Context, count.ToString());
        };
    })
    .Build()
    .Run();
