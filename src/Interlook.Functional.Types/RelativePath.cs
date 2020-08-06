using Interlook.Monads;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using io = System.IO;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]
namespace Interlook.Functional.Types
{
    /// <summary>
    /// A non-empty reference of a relative path, thus having no rooted directory.
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    public abstract class RelativePath : NonEmptyPath
    {
        internal RelativePath(SomeString trimmedPath, bool isDirectory) : base(trimmedPath, isDirectory)
        { }

        /// <summary>
        /// Tries to create a <see cref="RelativeDirectoryPath"/> object.
        /// </summary>
        /// <param name="path">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, RelativeDirectoryPath}"/> containing
        /// a new instance of <see cref="RelativeDirectoryPath"/> if the path provided was a valid relative directory path;
        /// otherwise <see cref="Left{Exception, RelativeDirectoryPath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, RelativeDirectoryPath> ReturnDirectoryPath(string path)
            => path.ToExceptionEither()
                .FailIf(_ => path == null, new ArgumentNullException(nameof(path)))
                .Bind(_ => StringBase.CreateSome(path))
                .Bind(ReturnDirectoryPath);

        /// <summary>
        /// Tries to create a <see cref="RelativeDirectoryPath"/> object.
        /// </summary>
        /// <param name="path">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, RelativeDirectoryPath}"/> containing
        /// a new instance of <see cref="RelativeDirectoryPath"/> if the path provided was a valid relative directory path;
        /// otherwise <see cref="Left{Exception, RelativeDirectoryPath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, RelativeDirectoryPath> ReturnDirectoryPath(SomeString path)
            => path.ToExceptionEither()
                .FailIf(_ => path == null, new ArgumentNullException(nameof(path)))
                .Bind(_ => EnsureDirectoryPathString(path))
                .Bind(CreateNonEmpty)
                .Bind(CheckRelativeConstraints)
                .Select(path => new RelativeDirectoryPath(path.TrimmedPathInternal));

        /// <summary>
        /// Tries to create a <see cref="RelativeFilePath"/> object.
        /// </summary>
        /// <param name="path">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, RelativeFilePath}"/> containing
        /// a new instance of <see cref="RelativeFilePath"/> if the path provided was a valid relative file path;
        /// otherwise <see cref="Left{Exception, RelativeFilePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, RelativeFilePath> ReturnFilePath(string path)
            => path.ToExceptionEither()
                .FailIf(_ => path == null, new ArgumentNullException(nameof(path)))
                .Bind(_ => StringBase.CreateSome(path))
                .Bind(ReturnFilePath);

        /// <summary>
        /// Tries to create a <see cref="RelativeFilePath"/> object.
        /// </summary>
        /// <param name="path">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, RelativeFilePath}"/> containing
        /// a new instance of <see cref="RelativeFilePath"/> if the path provided was a valid relative file path;
        /// otherwise <see cref="Left{Exception, RelativeFilePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, RelativeFilePath> ReturnFilePath(SomeString path)
            => path.ToExceptionEither()
                .FailIf(_ => path == null, new ArgumentNullException(nameof(path)))
                .Bind(_ => GetTrimmedPathString(path))
                .Bind(CreateNonEmpty)
                .Bind(CheckRelativeConstraints)
                .Select(path => new RelativeFilePath(path.TrimmedPathInternal));
        
        internal static Either<Exception, T> CheckRelativeConstraints<T>(T validPath)
            where T : NonEmptyPath
            => validPath.ToExceptionEither()
                .FailIf(path => io.Path.IsPathRooted(path.Path), path => new ArgumentException($"Path '{path}' has a root directory and thus no relative path.", nameof(validPath)));
    }
}