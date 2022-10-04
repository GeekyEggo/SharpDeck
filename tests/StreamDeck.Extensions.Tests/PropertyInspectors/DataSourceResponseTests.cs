namespace StreamDeck.Extensions.Tests.PropertyInspectors
{
    /// <summary>
    /// Provides assertions for <see cref="DataSourceResponse"/>.
    /// </summary>
    [TestFixture]
    public class DataSourceResponseTests
    {
        /// <summary>
        /// Asserts <see cref="DataSourceResponse(string, DataSourceItem[])"/>
        /// </summary>
        [Test]
        public void Constructor_Params()
        {
            // Arrange, act.
            var payload = new DataSourceResponse("event_name", new DataSourceItem("val_1", "Value One"), new DataSourceItem("val_2", "Value Two"));

            // Assert.
            Assert.Multiple(() =>
            {
                Assert.That(payload.Event, Is.EqualTo("event_name"));
                DataSourceItemsHelper.Verify(payload.Items[0], "val_1", "Value One", false, childCount: null);
                DataSourceItemsHelper.Verify(payload.Items[1], "val_2", "Value Two", false, childCount: null);
            });
        }

        /// <summary>
        /// Asserts <see cref="DataSourceResponse(string, IEnumerable{DataSourceItem})"/>.
        /// </summary>
        [Test]
        public void Constructor_Enumerable()
        {
            // Arrange, act.
            var items = new List<DataSourceItem>
            {
                new DataSourceItem("val_1", "Value One"),
                new DataSourceItem("val_2", "Value Two")
            };

            var payload = new DataSourceResponse("event_name", items);

            // Assert.
            Assert.Multiple(() =>
            {
                Assert.That(payload.Event, Is.EqualTo("event_name"));
                DataSourceItemsHelper.Verify(payload.Items[0], "val_1", "Value One", false, childCount: null);
                DataSourceItemsHelper.Verify(payload.Items[1], "val_2", "Value Two", false, childCount: null);
            });
        }
    }
}
