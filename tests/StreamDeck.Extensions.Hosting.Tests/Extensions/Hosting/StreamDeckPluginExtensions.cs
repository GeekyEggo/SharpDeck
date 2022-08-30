namespace StreamDeck.Tests.Extensions.Hosting.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using StreamDeck.Extensions.Hosting;

    /// <summary>
    /// Provides assertions for <see cref="StreamDeckPlugin"/>.
    /// </summary>
    [TestFixture]
    public class StreamDeckPluginExtensions
    {
        /// <summary>
        /// Asserts <see cref="StreamDeckPlugin.CreateBuilder"/> configures the builder.
        /// </summary>
        [Test]
        public void CreateBuilder()
        {
            // Arrange, act.
            var app = StreamDeckPlugin.CreateBuilder()
                .ConfigureServices(s => s.AddSingleton(RegistrationParametersTests.MOCK_ARGS))
                .Build();

            // Assert.
            Assert.Multiple(() =>
            {
                Assert.That(app.Services.GetRequiredService<StreamDeckConnection>(), Is.Not.Null);
                Assert.That(app.Services.GetRequiredService<IStreamDeckConnectionManager>(), Is.Not.Null);
                Assert.That(app.Services.GetRequiredService<IStreamDeckConnection>(), Is.Not.Null);
                Assert.That(app.Services.GetRequiredService<StreamDeckConnection>(), Is.SameAs(app.Services.GetRequiredService<IStreamDeckConnection>()));
                Assert.That(app.Services.GetRequiredService<StreamDeckConnection>(), Is.SameAs(app.Services.GetRequiredService<IStreamDeckConnectionManager>()));
                Assert.That(app.Services.GetRequiredService<IHostLifetime>(), Is.TypeOf<StreamDeckPluginHostLifetime>());
            });
        }
    }
}
