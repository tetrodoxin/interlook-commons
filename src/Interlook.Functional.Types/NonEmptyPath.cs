using Interlook.Monads;
using Interlook.Text;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using io = System.IO;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]
namespace Interlook.Functional.Types
{
    /// <summary>
    /// Base type of non-empty path references.
    /// </summary>
    /// <seealso cref="IPath" />
    public class NonEmptyPath : IPath
    {
        internal static readonly char[] InvalidPathChars =
            io.Path.GetInvalidPathChars()   // system defined invalid path chars
            .Concat("*?".ToCharArray())     // also append wildcard chars
            .ToArray();

        private int _hash;

        /// <summary>
        /// Determines if the path references a directory.
        /// </summary>
        public bool IsDirectory { get; }

        /// <summary>
        /// The referenced path as <see cref="SomeString"/>,
        /// where directories always end with <see cref="System.IO.Path.DirectorySeparatorChar"/>
        /// and file paths do not.
        /// </summary>
        public SomeString Path { get; }

        string IPath.Path => Path.Value;

        internal SomeString TrimmedPathInternal { get; }

        internal NonEmptyPath(SomeString trimmedPath, bool isDirectory)
        {
            IsDirectory = isDirectory;
            TrimmedPathInternal = trimmedPath;
            Path = isDirectory ? TrimmedPathInternal.Concat(io.Path.DirectorySeparatorChar) : TrimmedPathInternal;
            _hash = Path.GetHashCode();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="NonEmptyPath"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// The content of <see cref="Path"/> as <see cref="string"/>
        /// </returns>
        public static implicit operator string(NonEmptyPath path) => path.Path;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other) => other is NonEmptyPath dir ? dir._hash.Equals(_hash) && dir.Path.Equals(Path) : false;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => _hash;

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => Path;

        internal static Either<Exception, NonEmptyPath> CreateNonEmpty(string path)
                                            => SomeString.CreateSome(path)
            .AddOuterException(ex => new Exception("Path must not be empty and consist of actual characters, not just whitespaces", ex))
            .Bind(path => CreateNonEmpty(path));

        internal static Either<Exception, NonEmptyPath> CreateNonEmpty(SomeString path)
            => path.ToExceptionEither()
            .Bind(origPath => origPath.Value.IndexOfAny(InvalidPathChars).ToExceptionEither()
                .FailIf(pos => pos >= 0, pos => new ArgumentException($"Path contains invalid character '{origPath.Value[pos]}' at position {pos}.", nameof(path)))
                .Select(pos => origPath))
            .Bind(origPath => GetTrimmedPathString(origPath).Select(trimmedPath => (Path: trimmedPath, IsDirectory: EndsInDirectorySeparator(origPath))))
            .FailIf(p => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && pathEndsWithPeriodOrSpace(p.Path), p => new ArgumentException("In Windows paths must not end with '.'", nameof(path)))
            .Select(p => new NonEmptyPath(p.Path, p.IsDirectory));

        internal static bool EndsInDirectorySeparator(SomeString path) => isDirectorySeparatorChar(path[path.Length - 1]);

        internal static Either<Exception, SomeString> EnsureDirectoryPathString(SomeString path)
            => (EndsInDirectorySeparator(path) ? path : path.Concat(io.Path.DirectorySeparatorChar)).ToExceptionEither();

        internal static Either<Exception, SomeString> GetTrimmedPathString(SomeString filePath)
            => filePath.ToExceptionEither()
                .Bind(path => EndsInDirectorySeparator(path)
                    ? StringBase.CreateSome(path.Value.Substring(0, path.Length - 1))
                    : filePath.ToExceptionEither()); 

        internal static bool IsDirectoryPath(SomeString s) => EndsInDirectorySeparator(s);

        internal SomeString CombinePathName(SomeString pathSuffix)
            => TrimmedPathInternal.Concat($"{io.Path.DirectorySeparatorChar}{pathSuffix}");

        internal SomeString CombinePathName(string pathSuffix)
            => TrimmedPathInternal.Concat($"{io.Path.DirectorySeparatorChar}{pathSuffix}");

        internal SomeString CombinePathName(char pathSuffix)
            => TrimmedPathInternal.Concat($"{io.Path.DirectorySeparatorChar}{pathSuffix}");

        private static bool isDirectorySeparatorChar(char c) => c == io.Path.DirectorySeparatorChar || c == io.Path.AltDirectorySeparatorChar;

        private static bool pathEndsWithPeriodOrSpace(SomeString trimmedPathString)
                                                                            => Try.Invoke(() => io.Path.GetFileName(trimmedPathString.Value))
                .Then(p => p.IsNeitherNullNorEmpty() && p.Length > 1 && p[p.Length - 1] == '.')
                .MapTry(succ => succ, error => true);
    }
}