namespace StreamDeck.Generators.IO
{
    using System.Text;

    /// <summary>
    /// Provides methods for interacting with the file system.
    /// </summary>
    internal class FileSystem : IFileSystem
    {
        /// <inheritdoc/>
        public bool Exists(string path)
            => File.Exists(path);

        /// <inheritdoc/>
        public void WriteAllText(string path, string contents, Encoding encoding)
            => File.WriteAllText(path, contents, encoding);
    }
}
