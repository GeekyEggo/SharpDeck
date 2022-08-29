namespace Counter
{
    using StreamDeck.Manifest;

    /// <summary>
    /// Defines the action that increments the count.
    /// </summary>
    [PluginAction(
        name: "Counter",
        uuid: "com.geekyeggo.counter-example.increment",
        icon: "Images/Action",
        SupportedInMultiActions = false,
        Tooltip = "Increment the count by one.")]
    [PluginActionState(
        image: "Images/Key",
        TitleAlignment = TitleAlignment.Middle,
        FontFamily = FontFamily.Default,
        FontSize = "18")]
    public class IncrementAction
    {
    }
}
