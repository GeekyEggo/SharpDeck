namespace DrillDown
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NLog.Extensions.Logging;
    using SharpDeck;

    /// <summary>
    /// The plugin.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            new PluginHostBuilder()
                .ConfigureServices(services =>
                {
                    // Setup the configuration to read from the App.config file.
                    var config = new ConfigurationBuilder()
                        .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                        .Build();

                    // Configure the logging
                    services.AddScoped<IConfiguration>(_ => config)
                        .AddLogging(loggingBuilder =>
                        {
                            loggingBuilder.ClearProviders()
                                .AddConfiguration(config.GetSection("Logging"))
                                .AddNLog(new NLogLoggingConfiguration(config.GetSection("NLog")));
                        });
                })
                .Build()
                .Start();
        }
    }
}
