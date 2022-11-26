namespace SharpDeck.Interactivity
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Connectivity;
    using SharpDeck.Enums;

    /// <summary>
    /// Provides methods that enable rendering and feedback to be displayed on a Stream Deck button.
    /// </summary>
    public class StreamDeckButton : IButton
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckButton"/> class.
        /// </summary>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="context">The context used to identify the action.</param>
        internal StreamDeckButton(IStreamDeckConnection connection, string context)
            : this()
        {
            this.Context = context;
            this.StreamDeck = connection;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckButton"/> class.
        /// </summary>
        private protected StreamDeckButton()
        {
        }

        /// <summary>
        /// Gets an opaque value identifying the instance of the action. You will need to pass this opaque value to several APIs like the `setTitle` API.
        /// </summary>
        public string Context { get; internal set; }

        /// <summary>
        /// Gets the connection with the Stream Deck; this is responsible for sending and receiving events and messages.
        /// </summary>
        public IStreamDeckConnection StreamDeck { get; private protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is disposed.
        /// </summary>
        private bool IsDisposed { get; set; } = false;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dynamically change the feedback provided by the touchscreen; this corresponds with the layout associated with the action.
        /// </summary>
        /// <param name="feedback">The feedback object that contains information about the feedback.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the feedback.</returns>
        public Task SetFeedbackAsync(object feedback, CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposed();
            return this.StreamDeck.SetFeedbackAsync(this.Context, feedback, cancellationToken);
        }

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action.
        /// </summary>
        /// <param name="image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). If no image is passed, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the image is set to all states.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the image.</returns>
        public Task SetImageAsync(string image = "", TargetType target = TargetType.Both, int? state = null, CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposed();
            return this.StreamDeck.SetImageAsync(this.Context, image, target, state, cancellationToken);
        }

        /// <summary>
        ///	Change the state of the actions instance supporting multiple states.
        /// </summary>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the state.</returns>
        public Task SetStateAsync(int state = 0, CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposed();
            return this.StreamDeck.SetStateAsync(this.Context, state, cancellationToken);
        }

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// </summary>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the title.</returns>
        public Task SetTitleAsync(string title = "", TargetType target = TargetType.Both, int? state = null, CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposed();
            return this.StreamDeck.SetTitleAsync(this.Context, title, target, state, cancellationToken);
        }
        /// <summary>
        /// Temporarily show an alert icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of showing the alert.</returns>
        public Task ShowAlertAsync(CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposed();
            return this.StreamDeck.ShowAlertAsync(this.Context, cancellationToken);
        }

        /// <summary>
        /// Temporarily show an OK checkmark icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of showing the OK.</returns>
        public Task ShowOkAsync(CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposed();
            return this.StreamDeck.ShowOkAsync(this.Context, cancellationToken);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.StreamDeck = null;
            this.IsDisposed = true;
        }

        /// <summary>
        /// Throws the <see cref="ObjectDisposedException"/> if this instance has been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException($"Action:{this.Context}");
            }
        }
    }
}
