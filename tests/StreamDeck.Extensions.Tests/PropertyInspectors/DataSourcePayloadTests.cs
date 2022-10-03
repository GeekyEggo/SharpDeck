namespace StreamDeck.Extensions.Tests.PropertyInspectors
{
    /// <summary>
    /// Provides assertions for <see cref="DataSourcePayload"/>.
    /// </summary>
    [TestFixture]
    public class DataSourcePayloadTests
    {
        /// <summary>
        /// Asserts <see cref="DataSourcePayload(string, DataSourceItem[])"/>
        /// </summary>
        [Test]
        public void Constructor_Params()
        {
            // Arrange, act.
            var payload = new DataSourcePayload("event_name", new DataSourceItem("val_1", "Value One"), new DataSourceItem("val_2", "Value Two"));

            // Assert.
            Assert.Multiple(() =>
            {
                Assert.That(payload.Event, Is.EqualTo("event_name"));
                DataSourceItemsHelper.Verify(payload.Items[0], "val_1", "Value One", false, childCount: null);
                DataSourceItemsHelper.Verify(payload.Items[1], "val_2", "Value Two", false, childCount: null);
            });
        }

        /// <summary>
        /// Asserts <see cref="DataSourcePayload(string, IEnumerable{DataSourceItem})"/>.
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

            var payload = new DataSourcePayload("event_name", items);

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
