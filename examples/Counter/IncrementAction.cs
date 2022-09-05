namespace Counter
{
    /// <summary>
    /// Defines the action that increments the count.
    /// </summary>
    [Action(
        name: "Counter",
        uuid: "com.geekyeggo.counter-example.increment",
        icon: "Images/Action",
        SupportedInMultiActions = false,
        Tooltip = "Increment the count by one.")]
    [State(
        image: "Images/Key",
        TitleAlignment = TitleAlignment.Middle,
        FontFamily = FontFamily.Default,
        FontSize = "18")]
    public class IncrementAction
    {
    }
}
