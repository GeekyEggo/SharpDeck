namespace SharpDeck.Manifest
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides a helper class for writing a manifest file based on the definition of an assembly.
    /// </summary>
    public static class ManifestWriter
    {
        /// <summary>
        /// Attempts to write a manifest file based on the current arguments.
        /// </summary>
        /// <param name="args">The arguments, typically passed in from a CLI.</param>
        /// <param name="result">The result of executing the generation.</param>
        /// <returns><c>true</c> when the arguments allowed for the attempted generation of the manifest file, this will be <c>true</c> if generation also fails; otherwise <c>false</c>.</returns>
        [Obsolete("The manifest writer will soon be deprecated in favour of CLI tools")]
        public static bool TryWrite(string[] args, out int result)
        {
            // read the parameters
            using (var app = new CommandLineApplication(throwOnUnexpectedArg: false))
            {
                var parameters = new ManifestWriterParameters(app);

                // execute the parameters; bailing out swiftly if the args dont permit it
                var didRun = false;
                app.OnExecute(() => TryExecute(parameters, out didRun));
                result = app.Execute(args);

                return didRun;
            }
        }

        /// <summary>
        /// Gets the path, based on the assembly location.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="option">The parameter containing the relative path.</param>
        /// <param name="default">The default path, when no value is specified as part of the <paramref name="option"/></param>
        /// <returns>The path.</returns>
        private static string GetPath(Assembly assembly, CommandOption option, string @default)
        {
            var assemblyCodeBase = new Uri(assembly.GetName().CodeBase);
            var dir = new FileInfo(assemblyCodeBase.AbsolutePath).Directory.FullName;
            var path = option.HasValue() ? option.Value() : @default;

            return Path.Combine(dir, path);
        }

        /// <summary>
        /// Attempts to execute a manifest write based on the arguments.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="didExecute">Indicates whether execution was attempted; this will return <c>false</c> if the parameters did not specify generation, i.e. -m was missing.</param>
        /// <returns>The result if execution.</returns>
        private static int TryExecute(ManifestWriterParameters parameters, out bool didExecute)
        {
            // do nothing when the user did not specify generation
            didExecute = parameters.WriteManifest.HasValue();
            if (!didExecute)
            {
                return 0;
            }

            // get the assembly and output path
            var assembly = parameters.AssemblyPath.HasValue() ? Assembly.LoadFile(parameters.AssemblyPath.Value()) : Assembly.GetEntryAssembly();
            var outputPath = GetPath(assembly, parameters.OutputPath, "manifest.json");

            WriteManifest(assembly, outputPath, GetDefaultValues(assembly, parameters.NpmPackagePath));
            return 0;
        }

        /// <summary>
        /// Builds a manifest based on the specified assembly, and write the JSON result to the specified path.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="outputPath">The output path.</param>
        /// <param name="defaultValues">The default values.</param>
        private static void WriteManifest(Assembly assembly, string outputPath, StreamDeckPluginAttribute defaultValues)
        {
            var builder = new ManifestBuilder(assembly, defaultValues);

            new FileInfo(outputPath).Directory.Create();
            File.WriteAllText(outputPath, builder.ToJson(), Encoding.UTF8);
        }

        /// <summary>
        /// Gets the default values for the manifest; when a package file is supplied, these values are used.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="pkgOption">The package file path option.</param>
        /// <returns>The default values for the top level manifest information.</returns>
        private static StreamDeckPluginAttribute GetDefaultValues(Assembly assembly, CommandOption pkgOption)
        {
            var path = GetPath(assembly, pkgOption, "package.json");
            if (File.Exists(path))
            {
                var pkgContents = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<StreamDeckPluginAttribute>(pkgContents);
            }

            return new StreamDeckPluginAttribute();
        }
    }
}
