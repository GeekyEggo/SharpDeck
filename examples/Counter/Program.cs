using System.Collections.Concurrent;

#if DEBUG
System.Diagnostics.Debugger.Launch();
#endif

var counts = new ConcurrentDictionary<string, int>();

// Construct the connection, and register the event handler.
var connection = new StreamDeckConnection();
connection.WillAppear += (_, e) => connection.SetTitleAsync(e.Context!, counts.GetOrAdd(e.Context!, 0).ToString());
connection.KeyDown += (_, e) =>
{
    var count = counts.AddOrUpdate(e.Context!, 1, (_, value) => ++value);
    _ = connection.SetTitleAsync(e.Context!, count.ToString());
};

await connection.ConnectAndWaitAsync();
