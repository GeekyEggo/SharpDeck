namespace SharedCounter
{
    using System;

    /// <summary>
    /// The count.
    /// </summary>
    public class Count
    {
        /// <summary>
        /// The synchronization root.
        /// </summary>
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// Private member field for <see cref="Instance"/>.
        /// </summary>
        private static readonly Lazy<Count> _instance = new Lazy<Count>(() => new Count(), true);

        /// <summary>
        /// Private member field for <see cref="Value"/>
        /// </summary>
        private int _value = 0;

        /// <summary>
        /// Prevents a default instance of the <see cref="Count"/> class from being created.
        /// </summary>
        private Count()
        {
            this.Value = 0;
        }

        /// <summary>
        /// Occurs when <see cref="Count.Value"/> changes.
        /// </summary>
        public event EventHandler CountChanged;

        /// <summary>
        /// Gets the singleton instance of <see cref="Count"/>.
        /// </summary>
        public static Count Instance => _instance.Value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public int Value
        {
            get => this._value;
            private set
            {
                lock (_syncRoot)
                {
                    if (this._value != value)
                    {
                        this._value = value;
                        this.CountChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Increments <see cref="Count.Value"/> by one.
        /// </summary>
        public void Increment()
            => this.Value++;

        /// <summary>
        /// Resets <see cref="Count.Value"/> to zero.
        /// </summary>
        public void Reset()
            => this.Value = 0;

        /// <summary>
        /// Sets the count.
        /// </summary>
        /// <param name="count">The count.</param>
        public void Set(int count)
            => this.Value = count;
    }
}
