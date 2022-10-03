namespace StreamDeck.Extensions.Tests.Helpers
{
    /// <summary>
    /// Provides test helper methods for <see cref="DataSourceItem"/>.
    /// </summary>
    internal static class DataSourceItemsHelper
    {
        /// <summary>
        /// Verifies the specified <see cref="DataSourceItem"/>.
        /// </summary>
        /// <param name="item">The item to verify.</param>
        /// <param name="value">The expected <see cref="DataSourceItem.Value"/>.</param>
        /// <param name="label">The expected <see cref="DataSourceItem.Label"/>.</param>
        /// <param name="disabled">The expected <see cref="DataSourceItem.Disabled"/>.</param>
        /// <param name="childCount">The expected number of <see cref="DataSourceItem.Children"/>; when <c>null</c>, asserts the collection is <c>null</c>.</param>
        public static void Verify(DataSourceItem item, string? value, string? label, bool disabled, int? childCount)
        {
            Assert.Multiple(() =>
            {
                if (childCount == null)
                {
                    Assert.That(item.Children, Is.Null);
                }
                else
                {
                    Assert.That(item.Children!.Count(), Is.EqualTo(childCount));
                }

                Assert.That(item.Disabled, Is.EqualTo(disabled));
                Assert.That(item.Label, Is.EqualTo(label));
                Assert.That(item.Value, Is.EqualTo(value));
            });
        }
    }
}
