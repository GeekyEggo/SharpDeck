namespace StreamDeck.Extensions.Tests.PropertyInspectors
{
    /// <summary>
    /// Provides assertions for <see cref="DataSourceItem"/>.
    /// </summary>
    [TestFixture]
    public class DataSourceItemTests
    {
        /// <summary>
        /// Asserts <see cref="DataSourceItem()"/>.
        /// </summary>
        [Test]
        public void Constructor()
            => DataSourceItemsHelper.Verify(new DataSourceItem(), string.Empty, null, false, childCount: null);

        /// <summary>
        /// Asserts <see cref="DataSourceItem(string, string, bool)"/>.
        /// </summary>
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void Constructor_Item_WithValueAndLabel(bool disabled)
        {
            // Arrange, act.
            const string value = "foo";
            const string label = "bar";

            var item = new DataSourceItem(value, label, disabled);

            // Assert.
            DataSourceItemsHelper.Verify(item, value, label, disabled, childCount: null);
        }

        /// <summary>
        /// Asserts <see cref="DataSourceItem(DataSourceItem[])"/>.
        /// </summary>
        [Test]
        public void Constructor_ItemGroup()
        {
            // Arrange, act.
            var item = new DataSourceItem(
                new[]
                {
                    new DataSourceItem("foo", "bar"),
                    new DataSourceItem("hello", "world", disabled: true)
                });

            // Assert.
            DataSourceItemsHelper.Verify(item, null, null, false, childCount: 2);
            DataSourceItemsHelper.Verify(item.Children!.First(), "foo", "bar", false, childCount: null);
            DataSourceItemsHelper.Verify(item.Children!.Last(), "hello", "world", true, childCount: null);
        }

        /// <summary>
        /// Asserts <see cref="DataSourceItem(string, DataSourceItem[])"/>.
        /// </summary>
        [Test]
        public void Constructor_ItemGroup_WithLabel()
        {
            // Arrange, act.
            var item = new DataSourceItem(
                label: "Group",
                new[]
                {
                    new DataSourceItem("1", "One"),
                    new DataSourceItem("Sub-Group",
                    new[]
                    {
                        new DataSourceItem("1-1", "> One"),
                        new DataSourceItem("1-2", "> Two")
                    })
                });

            // Assert.
            DataSourceItemsHelper.Verify(item, null, "Group", false, childCount: 2);
            DataSourceItemsHelper.Verify(item.Children!.First(), "1", "One", false, childCount: null);
            DataSourceItemsHelper.Verify(item.Children!.Last(), null, "Sub-Group", false, childCount: 2);
            DataSourceItemsHelper.Verify(item.Children!.Last().Children!.First(), "1-1", "> One", false, childCount: null);
            DataSourceItemsHelper.Verify(item.Children!.Last().Children!.Last(), "1-2", "> Two", false, childCount: null);
        }
    }
}
