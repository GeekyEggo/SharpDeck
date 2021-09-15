namespace SharpDeck.Interactivity
{
    using System.Collections.Generic;
    using System.Linq;
    using SharpDeck.Events.Received;

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
        /// <param name="deviceButtonMap">The device button map.</param>
        /// <param name="dataSource">The underlying data source to be paged.</param>
        public DevicePager(DeviceButtonMap deviceButtonMap, IEnumerable<T> dataSource)
        {
            this.DataSource = dataSource;
            this.Buttons = deviceButtonMap;

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
        /// Gets the coordinates of the next button; otherwise <c>null</c>.
        /// </summary>
        public Coordinates NextButtonCoordinates => !this.IsLastPage ? this.Buttons[this.Buttons.Count - 1].Coordinates : null;

        /// <summary>
        /// Gets the coordinates of the previous button; otherwise <c>null</c>.
        /// </summary>
        public Coordinates PreviousButtonCoordinates => !this.IsFirstPage ? this.Buttons[this.Buttons.Count - this.NavigationButtonCount].Coordinates : null;

        /// <summary>
        /// Gets the button map.
        /// </summary>
        private DeviceButtonMap Buttons { get; }

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

            // Determine the maximum number of items that can be shown; we subtract 1 to allow for a "X" button.
            var capacity = this.Buttons.Count - 1;

            if (this.IsFirstPage)
            {
                var items = this.DataSource.Take(capacity + 1);

                this.IsLastPage = items.Count() <= capacity;
                this.Items = items.Take(this.IsLastPage ? capacity : capacity - 1).ToArray();
            }
            else
            {
                // We must show a previous button, so the capacity is reduced.
                capacity--;
                var offset = capacity + ((this.PageIndex - 1) * (capacity - 1));
                var items = this.DataSource.Skip(offset).Take(capacity + 1);

                this.IsLastPage = items.Count() <= capacity;
                this.Items = items.Take(this.IsLastPage ? capacity : capacity - 1).ToArray();
            }
        }
    }
}
