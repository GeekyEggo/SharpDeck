namespace StreamDeck.Tests.Routing
{
    using Moq;
    using StreamDeck.Routing;
    using StreamDeck.Tests.Helpers;

    /// <summary>
    /// Provides assertions for <see cref="ActionRouter"/>.
    /// </summary>
    [TestFixture]
    public class ActionRouterTests
    {
        private const string ACTION_UUID = "com.tests.plugin.action";

        [Test]
        public void IgnoredUnregistered()
        {
            // Arrange.
            var actionFactory = new Mock<IActionFactory>();
            var connection = new Mock<IStreamDeckConnection>();
            var router = new ActionRouter(actionFactory.Object, connection.Object, null);

            // Act.
            var args = EventArgsBuilder.GetActionEventArgs();
            connection.Raise(c => c.WillAppear += null, connection.Object, args);

            // Assert.
            actionFactory.Verify(a => a.CreateInstance(It.IsAny<Type>(), It.IsAny<ActionInitializationContext>()), Times.Never);
        }

        [Test]
        public void BubblesOnWillAppear()
        {
            // Arrange.
            var actions = new Dictionary<string, Mock<StreamDeckAction>>();
            var actionFactory = new Mock<IActionFactory>();
            actionFactory
                .Setup(a => a.CreateInstance(It.IsAny<Type>(), It.IsAny<ActionInitializationContext>()))
                .Returns<Type, ActionInitializationContext>((_, ctx) =>
                {
                    actions.Add(ctx.ActionInfo.Context, new Mock<StreamDeckAction>(ctx));
                    return actions[ctx.ActionInfo.Context].Object;
                });

            var connection = new Mock<IStreamDeckConnection>();
            var router = new ActionRouter(actionFactory.Object, connection.Object, null);
            router.MapAction<StreamDeckAction>(ACTION_UUID);

            // Act.
            var contextOneArgs = EventArgsBuilder.GetActionEventArgs(ACTION_UUID, context: "1");
            connection.Raise(c => c.WillAppear += null, connection.Object, contextOneArgs);

            var contextTwoArgs = EventArgsBuilder.GetActionEventArgs(ACTION_UUID, context: "2");
            connection.Raise(c => c.WillAppear += null, connection.Object, contextTwoArgs);

            // Assert.
            actionFactory.Verify(a => a.CreateInstance(typeof(StreamDeckAction), It.IsAny<ActionInitializationContext>()), Times.Exactly(2));
            actions[contextOneArgs.Context].Verify(a => a.OnWillAppear(contextOneArgs), Times.Once);
            actions[contextTwoArgs.Context].Verify(a => a.OnWillAppear(contextTwoArgs), Times.Once);
        }
    }
}
