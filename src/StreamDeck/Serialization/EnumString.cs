namespace StreamDeck.Serialization
{
    using System;

    /// <summary>
    /// Provides a representation of a <see cref="Enum"/> that is capable of converting to/from a <see cref="string"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Value}")]
    public class EnumString<T> : IEquatable<T>
        where T : EnumString<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumString{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private protected EnumString(string value)
            => this.Value = value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        internal string Value { get; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="EnumString{T}"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="enumString">The enum string.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(EnumString<T> enumString)
            => enumString.Value;

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
            => obj is EnumString<T> other && this.Equals(other);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
            => this.Value?.GetHashCode() ?? 0;

        /// <summary>
        /// Converts the enum representation to a <see cref="string"/>.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
            => this.Value;

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>true</c> if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, <c>false</c>.</returns>
        public bool Equals(T? other)
            => other?.Value?.Equals(this.Value) == true;
    }
}
