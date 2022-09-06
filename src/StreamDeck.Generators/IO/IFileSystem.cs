namespace StreamDeck.Generators.IO
{
    using System.Text;

    /// <summary>
    /// Provides methods for interacting with the file system.
    /// </summary>
    internal interface IFileSystem
    {
        /// <summary>
        /// Determines whether the specified file exists; see <see cref="File.Exists(string)"/>.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns><c>true</c> when the file exists; otherwise <c>false</c>.</returns>
        bool Exists(string path);

        /// <summary>
        /// Writes the <paramref name="contents" /> to the specified <paramref name="path" />; see <see cref="File.WriteAllText(string, string, Encoding)" />.
        /// </summary>
        /// <param name="path">The path to write to.</param>
        /// <param name="contents">The contents to write.</param>
        /// <param name="encoding">The encoding.</param>
        void WriteAllText(string path, string contents, Encoding encoding);
    }
}
