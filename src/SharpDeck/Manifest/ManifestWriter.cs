namespace SharpDeck.Manifest
{
    using Microsoft.Extensions.CommandLineUtils;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;

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
        public static bool TryWrite(string[] args, out int result)
        {
            // read the parameters
            var app = new CommandLineApplication(throwOnUnexpectedArg: false);
            var parameters = new ManifestWriterParameters(app);

            // execute the parameters; bailing out swiftly if the args dont permit it
            var didRun = false;
            app.OnExecute(() => TryExecute(parameters, out didRun));
            result = app.Execute(args);

            return didRun;
        }

        /// <summary>
        /// Gets the manifest full file path.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="outputPath">The output path.</param>
        /// <returns>The file path of the manifest file.</returns>
        private static string GetManifestFilePath(Assembly assembly, CommandOption outputPath)
        {
            var assemblyCodeBase = new Uri(assembly.GetName().CodeBase);
            var dir = new FileInfo(assemblyCodeBase.AbsolutePath).Directory.FullName;
            var path = outputPath.HasValue() ? outputPath.Value() : "manifest.json";

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
            var path = GetManifestFilePath(assembly, parameters.OutputPath);

            WriteManifest(assembly, path);
            return 0;
        }

        /// <summary>
        /// Builds a manifest based on the specified assembly, and write the JSON result to the specified path.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="path">The path.</param>
        private static void WriteManifest(Assembly assembly, string path)
        {
            var builder = new ManifestBuilder(assembly);
            new FileInfo(path).Directory.Create();
            File.WriteAllText(path, builder.ToJson(), Encoding.UTF8);
        }
    }
}
