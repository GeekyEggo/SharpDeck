namespace DialCounter.Actions;

/// <summary>
/// The <see cref="CounterAction"/> settings.
/// </summary>
public class CounterSettings
{
    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    public int Count { get; set; } = 0;

    /// <summary>
    /// Gets or sets the tick multiplier
    /// </summary>
    public int TickMultiplier { get; set; } = 1;
}
