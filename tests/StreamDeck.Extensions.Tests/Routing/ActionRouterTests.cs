namespace StreamDeck.Tests.Routing
{
    using System;
    using System.Text.Json.Nodes;
    using Moq;
    using StreamDeck.Events;
    using StreamDeck.Routing;
    using StreamDeck.Tests.Helpers;

    /// <summary>
    /// Provides assertions for <see cref="ActionRouter"/>.
    /// </summary>
    [TestFixture]
    public class ActionRouterTests
    {
        /// <summary>
        /// Asserts that nothing is invoked if the action has not yet been constructed, i.e. via <see cref="IStreamDeckConnection.WillAppear"/>.
        /// </summary>
        [Test]
        public void IgnoreNotConstructed()
        {
            // Arrange.
            var (actions, actionFactory, dispatcher, connection, router) = CreateTestCase();
            router.MapAction<StreamDeckAction>(EventArgsBuilder.ACTION_UUID);

            // Act.
            var args = EventArgsBuilder.CreateKeyEventArgs();
            connection.Raise(c => c.KeyDown += null, connection.Object, args);

            // Assert.
            actionFactory.Verify(a => a.CreateInstance(typeof(StreamDeckAction), It.IsAny<ActionInitializationContext>()), Times.Never);
            dispatcher.Verify(d => d.Invoke(It.IsAny<Func<ActionEventArgs<KeyPayload>, Task>>(), args), Times.Never);
            Assert.That(actions, Is.Empty);
        }

        /// <summary>
        /// Asserts that unmapped <see cref="IActionContext.Action"/> are ignored.
        /// </summary>
        [Test]
        public void IgnoreUnmapped()
        {
            // Arrange.
            var actionFactory = new Mock<IActionFactory>();
            var eventDispatcher = new Mock<IEventDispatcher>();
            var connection = new Mock<IStreamDeckConnection>();
            var router = new ActionRouter(actionFactory.Object, eventDispatcher.Object, connection.Object, null);

            // Act.
            var args = EventArgsBuilder.CreateActionEventArgs();
            connection.Raise(c => c.WillAppear += null, connection.Object, args);

            // Assert.
            actionFactory.Verify(a => a.CreateInstance(It.IsAny<Type>(), It.IsAny<ActionInitializationContext>()), Times.Never);
        }

        /// <summary>
        /// Asserts <see cref="ActionRouter"/> propagates <see cref="IStreamDeckConnection.DidReceiveSettings"/> to <see cref="StreamDeckAction.OnDidReceiveSettings(ActionEventArgs{ActionPayload})"/>.
        /// </summary>
        [Test]
        public void Map_DidReceiveSettings()
        {
            // Arrange.
            var (actions, actionFactory, dispatcher, connection, router) = CreateTestCase();
            router.MapAction<StreamDeckAction>(EventArgsBuilder.ACTION_UUID);

            var args = EventArgsBuilder.CreateActionEventArgs();
            connection.Raise(c => c.WillAppear += null, connection.Object, args); // Ensure the instance exists.

            // Act.
            connection.Raise(c => c.DidReceiveSettings += null, connection.Object, args);

            // Assert.
            actionFactory.Verify(a => a.CreateInstance(typeof(StreamDeckAction), It.IsAny<ActionInitializationContext>()), Times.Once);
            dispatcher.Verify(d => d.Invoke(It.IsAny<Func<ActionEventArgs<ActionPayload>, Task>>(), args), Times.Exactly(2)); // This caters for WillAppear.
            actions[args.Context].Verify(a => a.OnDidReceiveSettings(args), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="ActionRouter"/> propagates <see cref="IStreamDeckConnection.KeyDown"/> to <see cref="StreamDeckAction.OnKeyDown(ActionEventArgs{KeyPayload})"/>.
        /// </summary>
        [Test]
        public void Map_KeyDown()
        {
            // Arrange.
            var (actions, actionFactory, dispatcher, connection, router) = CreateTestCase();
            router.MapAction<StreamDeckAction>(EventArgsBuilder.ACTION_UUID);

            connection.Raise(c => c.WillAppear += null, connection.Object, EventArgsBuilder.CreateActionEventArgs()); // Ensure the instance exists.

            // Act.
            var args = EventArgsBuilder.CreateKeyEventArgs();
            connection.Raise(c => c.KeyDown += null, connection.Object, args);

            // Assert.
            actionFactory.Verify(a => a.CreateInstance(typeof(StreamDeckAction), It.IsAny<ActionInitializationContext>()), Times.Once);
            dispatcher.Verify(d => d.Invoke(It.IsAny<Func<ActionEventArgs<KeyPayload>, Task>>(), args), Times.Once);
            actions[args.Context].Verify(a => a.OnKeyDown(args), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="ActionRouter"/> propagates <see cref="IStreamDeckConnection.KeyUp"/> to <see cref="StreamDeckAction.OnKeyUp(ActionEventArgs{KeyPayload})"/>.
        /// </summary>
        [Test]
        public void Map_KeyUp()
        {
            // Arrange.
            var (actions, actionFactory, dispatcher, connection, router) = CreateTestCase();
            router.MapAction<StreamDeckAction>(EventArgsBuilder.ACTION_UUID);

            connection.Raise(c => c.WillAppear += null, connection.Object, EventArgsBuilder.CreateActionEventArgs()); // Ensure the instance exists.

            // Act.
            var args = EventArgsBuilder.CreateKeyEventArgs();
            connection.Raise(c => c.KeyUp += null, connection.Object, args);

            // Assert.
            actionFactory.Verify(a => a.CreateInstance(typeof(StreamDeckAction), It.IsAny<ActionInitializationContext>()), Times.Once);
            dispatcher.Verify(d => d.Invoke(It.IsAny<Func<ActionEventArgs<KeyPayload>, Task>>(), args), Times.Once);
            actions[args.Context].Verify(a => a.OnKeyUp(args), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="ActionRouter"/> propagates <see cref="IStreamDeckConnection.PropertyInspectorDidAppear"/> to <see cref="StreamDeckAction.OnPropertyInspectorDidAppear(ActionEventArgs)"/>.
        /// </summary>
        [Test]
        public void Map_PropertyInspectorDidAppear()
        {
            // Arrange.
            var (actions, actionFactory, dispatcher, connection, router) = CreateTestCase();
            router.MapAction<StreamDeckAction>(EventArgsBuilder.ACTION_UUID);

            connection.Raise(c => c.WillAppear += null, connection.Object, EventArgsBuilder.CreateActionEventArgs()); // Ensure the instance exists.

            // Act.
            var args = new ActionEventArgs("event", EventArgsBuilder.ACTION_UUID, EventArgsBuilder.CONTEXT, "device");
            connection.Raise(c => c.PropertyInspectorDidAppear += null, connection.Object, args);

            // Assert.
            actionFactory.Verify(a => a.CreateInstance(typeof(StreamDeckAction), It.IsAny<ActionInitializationContext>()), Times.Once);
            dispatcher.Verify(d => d.Invoke(It.IsAny<Func<ActionEventArgs, Task>>(), args), Times.Once);
            actions[args.Context].Verify(a => a.OnPropertyInspectorDidAppear(args), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="ActionRouter"/> propagates <see cref="IStreamDeckConnection.PropertyInspectorDidDisappear"/> to <see cref="StreamDeckAction.OnPropertyInspectorDidDisappear(ActionEventArgs)"/>.
        /// </summary>
        [Test]
        public void Map_PropertyInspectorDidDisappear()
        {
            // Arrange.
            var (actions, actionFactory, dispatcher, connection, router) = CreateTestCase();
            router.MapAction<StreamDeckAction>(EventArgsBuilder.ACTION_UUID);

            connection.Raise(c => c.WillAppear += null, connection.Object, EventArgsBuilder.CreateActionEventArgs()); // Ensure the instance exists.

            // Act.
            var args = new ActionEventArgs("event", EventArgsBuilder.ACTION_UUID, EventArgsBuilder.CONTEXT, "device");
            connection.Raise(c => c.PropertyInspectorDidDisappear += null, connection.Object, args);

            // Assert.
            actionFactory.Verify(a => a.CreateInstance(typeof(StreamDeckAction), It.IsAny<ActionInitializationContext>()), Times.Once);
            dispatcher.Verify(d => d.Invoke(It.IsAny<Func<ActionEventArgs, Task>>(), args), Times.Once);
            actions[args.Context].Verify(a => a.OnPropertyInspectorDidDisappear(args), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="ActionRouter"/> propagates <see cref="IStreamDeckConnection.SendToPlugin"/> to <see cref="StreamDeckAction.OnSendToPlugin(PartialActionEventArgs{JsonObject})"/>.
        /// </summary>
        [Test]
        public void Map_SendToPlugin()
        {
            // Arrange.
            var (actions, actionFactory, dispatcher, connection, router) = CreateTestCase();
            router.MapAction<StreamDeckAction>(EventArgsBuilder.ACTION_UUID);

            connection.Raise(c => c.WillAppear += null, connection.Object, EventArgsBuilder.CreateActionEventArgs()); // Ensure the instance exists.

            // Act.
            var args = new PartialActionEventArgs<JsonObject>("event", new JsonObject(), EventArgsBuilder.ACTION_UUID, EventArgsBuilder.CONTEXT);
            connection.Raise(c => c.SendToPlugin += null, connection.Object, args);

            // Assert.
            actionFactory.Verify(a => a.CreateInstance(typeof(StreamDeckAction), It.IsAny<ActionInitializationContext>()), Times.Once);
            dispatcher.Verify(d => d.Invoke(It.IsAny<Func<PartialActionEventArgs<JsonObject>, Task>>(), args), Times.Once);
            actions[args.Context].Verify(a => a.OnSendToPlugin(args), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="ActionRouter"/> propagates <see cref="IStreamDeckConnection.TitleParametersDidChange"/> to <see cref="StreamDeckAction.OnTitleParametersDidChange(ActionEventArgs{TitlePayload})"/>.
        /// </summary>
        [Test]
        public void Map_TitleParametersDidChange()
        {
            // Arrange.
            var (actions, actionFactory, dispatcher, connection, router) = CreateTestCase();
            router.MapAction<StreamDeckAction>(EventArgsBuilder.ACTION_UUID);

            connection.Raise(c => c.WillAppear += null, connection.Object, EventArgsBuilder.CreateActionEventArgs()); // Ensure the instance exists.

            // Act.
            var args = EventArgsBuilder.CreateTitleEventArgs();
            connection.Raise(c => c.TitleParametersDidChange += null, connection.Object, args);

            // Assert.
            actionFactory.Verify(a => a.CreateInstance(typeof(StreamDeckAction), It.IsAny<ActionInitializationContext>()), Times.Once);
            dispatcher.Verify(d => d.Invoke(It.IsAny<Func<ActionEventArgs<TitlePayload>, Task>>(), args), Times.Once);
            actions[args.Context].Verify(a => a.OnTitleParametersDidChange(args), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="ActionRouter"/> propagates <see cref="IStreamDeckConnection.WillAppear"/> to <see cref="StreamDeckAction.OnWillAppear(ActionEventArgs{ActionPayload})"/>.
        /// </summary>
        [Test]
        public void Map_WillAppear()
        {
            // Arrange.
            var (actions, actionFactory, dispatcher, connection, router) = CreateTestCase();
            router.MapAction<StreamDeckAction>(EventArgsBuilder.ACTION_UUID);

            // Act.
            var args = EventArgsBuilder.CreateActionEventArgs();
            connection.Raise(c => c.WillAppear += null, connection.Object, args);

            // Assert.
            actionFactory.Verify(a => a.CreateInstance(typeof(StreamDeckAction), It.IsAny<ActionInitializationContext>()), Times.Once);
            dispatcher.Verify(d => d.Invoke(It.IsAny<Func<ActionEventArgs<ActionPayload>, Task>>(), args), Times.Once);
            actions[args.Context].Verify(a => a.OnWillAppear(args), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="ActionRouter"/> propagates <see cref="IStreamDeckConnection.WillDisappear"/> to <see cref="StreamDeckAction.OnWillDisappear(ActionEventArgs{ActionPayload})"/>.
        /// </summary>
        [Test]
        public void Map_WillDisappear()
        {
            // Arrange.
            var (actions, actionFactory, dispatcher, connection, router) = CreateTestCase();
            router.MapAction<StreamDeckAction>(EventArgsBuilder.ACTION_UUID);

            var args = EventArgsBuilder.CreateActionEventArgs();
            connection.Raise(c => c.WillAppear += null, connection.Object, args); // Ensure the instance exists.

            // Act.
            connection.Raise(c => c.WillDisappear += null, connection.Object, args);

            // Assert.
            actionFactory.Verify(a => a.CreateInstance(typeof(StreamDeckAction), It.IsAny<ActionInitializationContext>()), Times.Once);
            dispatcher.Verify(d => d.Invoke(It.IsAny<Func<ActionEventArgs<ActionPayload>, Task>>(), args), Times.Exactly(2));
            actions[args.Context].Verify(a => a.OnWillDisappear(args), Times.Once);
        }

        /// <summary>
        /// Creates the parameters required to construct a <see cref="ActionRouter"/> and execute a test case.
        /// </summary>
        /// <returns>The parameters.</returns>
        private static (Dictionary<string, Mock<StreamDeckAction>> Actions, Mock<IActionFactory> ActionFactory, Mock<IEventDispatcher> Dispatcher, Mock<IStreamDeckConnection> Connection, ActionRouter router) CreateTestCase()
        {
            var actions = new Dictionary<string, Mock<StreamDeckAction>>();
            var actionFactory = new Mock<IActionFactory>();
            actionFactory
                .Setup(a => a.CreateInstance(It.IsAny<Type>(), It.IsAny<ActionInitializationContext>()))
                .Returns<Type, ActionInitializationContext>((_, ctx) =>
                {
                    actions.Add(ctx.ActionInfo.Context, new Mock<StreamDeckAction>(ctx));
                    return actions[ctx.ActionInfo.Context].Object;
                });

            var dispatcher = new Mock<IEventDispatcher>()
                .SetupInvokeFor<ActionEventArgs>()
                .SetupInvokeFor<ActionEventArgs<ActionPayload>>()
                .SetupInvokeFor<ActionEventArgs<KeyPayload>>()
                .SetupInvokeFor<ActionEventArgs<TitlePayload>>()
                .SetupInvokeFor<PartialActionEventArgs<JsonObject>>();

            var connection = new Mock<IStreamDeckConnection>();
            var router = new ActionRouter(actionFactory.Object, dispatcher.Object, connection.Object, null);

            return (actions, actionFactory, dispatcher, connection, router);
        }
    }
}
