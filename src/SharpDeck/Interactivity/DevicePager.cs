namespace SharpDeck.Interactivity
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides pagination for a collection; the page size is dependent on the Stream Deck device.
    /// </summary>
    /// <typeparam name="T">The type of item within the collection.</typeparam>
    public class DevicePager<T>
    {
        /// <summary>
        /// The synchronization root.
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="DevicePager{T}" /> class.
        /// </summary>
        /// <param name="buttons">The available buttons for the device.</param>
        /// <param name="dataSource">The underlying data source to be paged.</param>
        public DevicePager(MonitoredButtonCollection buttons, IEnumerable<T> dataSource)
        {
            this.Buttons = buttons;
            this.DataSource = dataSource;

            this.SetItems(0);
        }

        /// <summary>
        /// Gets the page's current items.
        /// </summary>
        public T[] Items { get; private set; } = new T[0];

        /// <summary>
        /// Gets the number of navigation buttons currently available for the page.
        /// </summary>
        public int NavigationButtonCount
        {
            get
            {
                return this.IsFirstPage && this.IsLastPage
                    ? 0
                    : this.IsFirstPage || this.IsLastPage
                        ? 1
                        : 2;
            }
        }

        /// <summary>
        /// Gets the the next button when there is one; otherwise <c>null</c>.
        /// </summary>
        public IButton NextButton => !this.IsLastPage ? this.Buttons[this.Buttons.Length - 1] : null;

        /// <summary>
        /// Gets the the previous button when there is one; otherwise <c>null</c>.
        /// </summary>
        public IButton PreviousButton => !this.IsFirstPage ? this.Buttons[this.Buttons.Length - this.NavigationButtonCount] : null;

        /// <summary>
        /// Gets the available buttons for the device.
        /// </summary>
        private MonitoredButtonCollection Buttons { get; }

        /// <summary>
        /// Gets or sets the current page index.
        /// </summary>
        private int PageIndex { get; set; } = 0;

        /// <summary>
        /// Gets the data source.
        /// </summary>
        private IEnumerable<T> DataSource { get; }

        /// <summary>
        /// Gets a value indicating whether <see cref="Items"/> represent the first page of the data source.
        /// </summary>
        private bool IsFirstPage => this.PageIndex == 0;

        /// <summary>
        /// Gets a value indicating whether <see cref="Items"/> represent the last page of the data source.
        /// </summary>
        private bool IsLastPage { get; set; } = false;

        /// <summary>
        /// Changes the current page to the previous page.
        /// </summary>
        public void MovePrevious()
        {
            lock (this._syncRoot)
            {
                if (!this.IsFirstPage)
                {
                    this.SetItems(--this.PageIndex);
                }
            }
        }

        /// <summary>
        /// Changes the current page to the next page.
        /// </summary>
        public void MoveNext()
        {
            lock (this._syncRoot)
            {
                if (!this.IsLastPage)
                {
                    this.SetItems(++this.PageIndex);
                }
            }
        }

        /// <summary>
        /// Sets the current <see cref="Items"/> based on the specified <paramref name="pageIndex"/>.
        /// </summary>
        /// <param name="pageIndex">The desired page index.</param>
        private void SetItems(int pageIndex)
        {
            this.PageIndex = pageIndex;

            // Reduce the capacity; the first page will always have at least 1 button (close). All other pages will have at least 2 (close and next/previous).
            var capacity = this.IsFirstPage ? this.Buttons.Length - 1 : this.Buttons.Length - 2;

            // The first page has a greater capacity as it doesn't have a previous button.
            var offset = this.IsFirstPage ? 0 : capacity + ((this.PageIndex - 1) * (capacity - 1));

            // Take an extra item to determine if there is a next page.
            var items = this.DataSource.Skip(offset).Take(capacity + 1);
            this.IsLastPage = items.Count() <= capacity;

            // When there is a next page, we must allow for a "Next" button.
            this.Items = items.Take(this.IsLastPage ? capacity : capacity - 1).ToArray();
        }
    }
}
