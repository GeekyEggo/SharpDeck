namespace StreamDeck.Generators.IO
{
    using System.Text;

    /// <summary>
    /// Provides methods for interacting with the file system.
    /// </summary>
    internal class FileSystem : IFileSystem
    {
        /// <summary>
        /// Removes all <see cref="Path.GetInvalidFileNameChars"/> from the <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The valid file name.</returns>
        public static string RemoveInvalidFileNameChars(string fileName)
        {
            var result = fileName;
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                result = result.Replace(c.ToString(), string.Empty);
            }

            return result;
        }

        /// <inheritdoc/>
        public bool Exists(string path)
            => File.Exists(path);

        /// <inheritdoc/>
        public void WriteAllText(string path, string contents, Encoding encoding)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(path, contents, encoding);
        }
    }
}
