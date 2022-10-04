namespace StreamDeck.Extensions.Tests.Extensions.Hosting
{
    using System.Text.Json.Nodes;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Provides assertions for <see cref="StreamDeck.Extensions.Routing.HostExtensions"/>.
    /// </summary>
    [TestFixture]
    public class HostExtensionsTests
    {
        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapConnection(IHost, Action{IStreamDeckConnection})"/> correctly calls the delegate enabling the configuring of the <see cref="IStreamDeckConnection"/>.
        /// </summary>
        [Test]
        public void MapConnection()
        {
            // Arrange.
            IStreamDeckConnection? actualConnection = null;
            var host = new MockHost();

            // Act.
            host.Object.MapConnection(c => actualConnection = c);

            // Assert.
            Assert.That(actualConnection, Is.EqualTo(host.Connection.Object));
        }

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapApplicationDidLaunch(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapApplicationDidLaunch()
            => VerifyMap(
                (host, handler) => host.MapApplicationDidLaunch(handler),
                conn => conn.ApplicationDidLaunch += null,
                args: new StreamDeckEventArgs<ApplicationPayload>("foo", new ApplicationPayload("notepad.exe")),
                context: string.Empty);

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapApplicationDidTerminate(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapApplicationDidTerminate()
            => VerifyMap(
                (host, handler) => host.MapApplicationDidTerminate(handler),
                conn => conn.ApplicationDidTerminate += null,
                args: new StreamDeckEventArgs<ApplicationPayload>("foo", new ApplicationPayload("notepad.exe")),
                context: string.Empty);

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapDeviceDidConnect(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapDeviceDidConnect()
            => VerifyMap(
                (host, handler) => host.MapDeviceDidConnect(handler),
                conn => conn.DeviceDidConnect += null,
                args: new DeviceConnectEventArgs("event", "device", new DeviceInfo("name", new Size(1, 1), Device.StreamDeckXL)),
                context: string.Empty);

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapDeviceDidDisconnect(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapDeviceDidDisconnect()
            => VerifyMap(
                (host, handler) => host.MapDeviceDidDisconnect(handler),
                conn => conn.DeviceDidDisconnect += null,
                args: new DeviceEventArgs("event", "device"),
                context: string.Empty);

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapDidReceiveGlobalSettings(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapDidReceiveGlobalSettings()
            => VerifyMap(
                (host, handler) => host.MapDidReceiveGlobalSettings(handler),
                conn => conn.DidReceiveGlobalSettings += null,
                args: new StreamDeckEventArgs<SettingsPayload>("event", new SettingsPayload(new System.Text.Json.Nodes.JsonObject())),
                context: string.Empty);

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapDidReceiveSettings(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapDidReceiveSettings()
            => VerifyMap(
                (host, handler) => host.MapDidReceiveSettings(handler),
                conn => conn.DidReceiveSettings += null,
                args: EventArgsBuilder.CreateActionEventArgs(context: "ABC123"),
                context: "ABC123");

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapKeyDown(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapKeyDown()
            => VerifyMap(
                (host, handler) => host.MapKeyDown(handler),
                conn => conn.KeyDown += null,
                args: EventArgsBuilder.CreateKeyEventArgs(context: "ABC123"),
                context: "ABC123");

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapKeyUp(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapKeyUp()
            => VerifyMap(
                (host, handler) => host.MapKeyUp(handler),
                conn => conn.KeyUp += null,
                args: EventArgsBuilder.CreateKeyEventArgs(context: "ABC123"),
                context: "ABC123");

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapPropertyInspectorDidAppear(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapPropertyInspectorDidAppear()
            => VerifyMap(
                (host, handler) => host.MapPropertyInspectorDidAppear(handler),
                conn => conn.PropertyInspectorDidAppear += null,
                args: new ActionEventArgs("event", "action", "ABC123", "device"),
                context: "ABC123");

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapPropertyInspectorDidDisappear(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapPropertyInspectorDidDisappear()
            => VerifyMap(
                (host, handler) => host.MapPropertyInspectorDidDisappear(handler),
                conn => conn.PropertyInspectorDidDisappear += null,
                args: new ActionEventArgs("event", "action", "ABC123", "device"),
                context: "ABC123");

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapSendToPlugin(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapSendToPlugin()
            => VerifyMap(
                (host, handler) => host.MapSendToPlugin(handler),
                conn => conn.SendToPlugin += null,
                args: new PartialActionEventArgs<JsonObject>("event", new JsonObject(), "action", "ABC123"),
                context: "ABC123");

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapSystemDidWakeUp(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapSystemDidWakeUp()
            => VerifyMap(
                (host, handler) => host.MapSystemDidWakeUp(handler),
                conn => conn.SystemDidWakeUp += null,
                args: new StreamDeckEventArgs("event"),
                context: string.Empty);

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapTitleParametersDidChange(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapTitleParametersDidChange()
            => VerifyMap(
                (host, handler) => host.MapTitleParametersDidChange(handler),
                conn => conn.TitleParametersDidChange += null,
                args: EventArgsBuilder.CreateTitleEventArgs(context: "ABC123"),
                context: "ABC123");

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapWillAppear(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapWillAppear()
            => VerifyMap(
                (host, handler) => host.MapWillAppear(handler),
                conn => conn.WillAppear += null,
                args: EventArgsBuilder.CreateActionEventArgs(context: "ABC123"),
                context: "ABC123");

        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapWillDisappear(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapWillDisappear()
            => VerifyMap(
                (host, handler) => host.MapWillDisappear(handler),
                conn => conn.WillDisappear += null,
                args: EventArgsBuilder.CreateActionEventArgs(context: "ABC123"),
                context: "ABC123");

        /// <summary>
        /// Verifies a mapping on the <see cref="IHost"/>.
        /// </summary>
        /// <typeparam name="TArgs">The type of the arguments.</typeparam>
        /// <param name="map">The action responsible for applying the mapping.</param>
        /// <param name="eventExpression">The event expression used by <see cref="IMock{IStreamDeckConnection}"/> to raise the event.</param>
        /// <param name="args">The arguments supplied to the event when raising it.</param>
        /// <param name="context">The expected context supplied to the <see cref="IDispatcher.Invoke(Func{Task}, string)"/>.</param>
        private static void VerifyMap<TArgs>(Action<IHost, Delegate> map, Action<IStreamDeckConnection> eventExpression, TArgs args, string context)
        {
            // Arrange.
            var wasInvoked = false;
            var host = new MockHost();

            // Act.
            map(host.Object, Handler);
            host.Connection.Raise(eventExpression, host.Connection.Object, args);

            // Assert.
            Assert.That(wasInvoked, Is.True);
            host.Dispatcher.Verify(d => d.Invoke(It.IsAny<Func<Task>>(), context), Times.Once);

            void Handler(IStreamDeckConnection sender, TArgs actualArgs, IService resolvedService)
            {
                wasInvoked = true;
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(host.Connection.Object));
                    Assert.That(actualArgs, Is.EqualTo(args));
                    Assert.That(resolvedService, Is.EqualTo(host.Service));
                });
            }
        }
    }
}
