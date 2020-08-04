using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]
namespace Interlook.Functional.Types
{
    /// <summary>
    /// Represents an empty path.
    /// </summary>
    /// <seealso cref="IPath" />
    public sealed class EmptyPath : IPath
    {
        private static readonly Lazy<EmptyPath> _instance = new Lazy<EmptyPath>(() => new EmptyPath());

        /// <summary>
        /// Gets the default and only instance of this type
        /// </summary>
        /// <value>
        /// The default.
        /// </value>
        public static EmptyPath Default => _instance.Value;

        /// <summary>
        /// An empty string.
        /// </summary>
        public string Path => string.Empty;

        private EmptyPath()
        { }

        /// <summary>
        /// Performs an implicit conversion from <see cref="EmptyPath"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// An empty string.
        /// </returns>
        public static implicit operator string(EmptyPath path) => string.Empty;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other) => other is EmptyPath;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => 0;

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => string.Empty;
    }
}