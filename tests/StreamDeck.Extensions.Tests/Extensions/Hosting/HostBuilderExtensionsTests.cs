namespace StreamDeck.Tests.Extensions.Hosting
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using StreamDeck.Extensions.Hosting;

    /// <summary>
    /// Provides assertions for <see cref="HostBuilderExtensions"/>.
    /// </summary>
    [TestFixture]
    public class HostBuilderExtensionsTests
    {
        /// <summary>
        /// Asserts <see cref="HostBuilderExtensions.UsePluginLifetime(IHostBuilder)"/> configures the lifetime of the host.
        /// </summary>
        [Test]
        public void UsePluginLifetime()
        {
            // Arrange.
            var builder = new HostBuilder()
                .ConfigureServices(s => s.AddSingleton(RegistrationParametersTests.MOCK_ARGS));

            // Act.
            var app = builder.UsePluginLifetime()
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