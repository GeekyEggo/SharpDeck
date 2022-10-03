namespace StreamDeck.Extensions.Tests.Routing
{
    /// <summary>
    /// Provides assertions for <see cref="ActivatorUtilitiesActionFactory"/>.
    /// </summary>
    [TestFixture]
    public class ActivatorUtilitiesActionFactoryTests
    {
        /// <summary>
        /// Asserts <see cref="ActivatorUtilitiesActionFactory.CreateInstance(Type, ActionInitializationContext)"/>.
        /// </summary>
        [Test]
        public void CreateInstance()
        {
            // Arrange.
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(s => s.GetService(typeof(IServiceProvider))).Returns(serviceProvider.Object);

            var connection = new Mock<IStreamDeckConnection>();
            var context = new ActionInitializationContext(connection.Object, EventArgsBuilder.CreateActionEventArgs());

            // Act.
            var factory = new ActivatorUtilitiesActionFactory(serviceProvider.Object);
            var action = factory.CreateInstance(typeof(FooAction), context);

            // Assert.
            serviceProvider.Verify(s => s.GetService(typeof(IServiceProvider)), Times.Once);
            Assert.Multiple(() =>
            {
                Assert.That(action.ActionUUID, Is.SameAs(context.ActionInfo.Action));
                Assert.That(action.Connection, Is.SameAs(connection.Object));
                Assert.That(action.Context, Is.SameAs(context.ActionInfo.Context));

                Assert.That(action, Is.TypeOf<FooAction>());
                Assert.That(((FooAction)action).ServiceProvider, Is.EqualTo(serviceProvider.Object));
            });
        }

        /// <summary>
        /// A mock <see cref="StreamDeckAction"/>.
        /// </summary>
        public class FooAction : StreamDeckAction
        {
            public FooAction(ActionInitializationContext context, IServiceProvider serviceProvider)
                : base(context) => this.ServiceProvider = serviceProvider;

            public IServiceProvider ServiceProvider { get; }
        }
    }
}
