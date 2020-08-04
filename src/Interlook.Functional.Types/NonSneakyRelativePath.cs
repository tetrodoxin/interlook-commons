using Interlook.Monads;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]
namespace Interlook.Functional.Types
{
    /// <summary>
    /// A non-empty reference of a relative path, where path traversal with ".." is forbidden.
    /// </summary>
    /// <seealso cref="NonEmptyPath"/>
    /// <seealso cref="NonSneakyPath"/>
    /// <seealso cref="RelativePath"/>
    public class NonSneakyRelativePath : NonSneakyPath
    {
        internal NonSneakyRelativePath(SomeString trimmedPath, bool isDirectory) : base(trimmedPath, isDirectory)
        { }

        /// <summary>
        /// Tries to create a <see cref="NonSneakyRelativeDirectoryPath"/> object.
        /// </summary>
        /// <param name="path">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, NonSneakyRelativeDirectoryPath}"/> containing
        /// a new instance of <see cref="NonSneakyRelativeDirectoryPath"/> if the path provided was a valid relative, non-sneaky directory path;
        /// otherwise <see cref="Left{Exception, NonSneakyRelativeDirectoryPath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, NonSneakyRelativeDirectoryPath> ReturnDirectoryPath(string path)
            => path.ToExceptionEither()
                .FailIf(_ => path == null, new ArgumentNullException(nameof(path)))
                .Bind(_ => StringBase.CreateSome(path))
                .Bind(ReturnDirectoryPath);

        /// <summary>
        /// Tries to create a <see cref="NonSneakyRelativeDirectoryPath"/> object.
        /// </summary>
        /// <param name="path">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, NonSneakyRelativeDirectoryPath}"/> containing
        /// a new instance of <see cref="NonSneakyRelativeDirectoryPath"/> if the path provided was a valid relative, non-sneaky directory path;
        /// otherwise <see cref="Left{Exception, NonSneakyRelativeDirectoryPath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, NonSneakyRelativeDirectoryPath> ReturnDirectoryPath(SomeString path)
            => path.ToExceptionEither()
                .FailIf(_ => path == null, new ArgumentNullException(nameof(path)))
                .Bind(_ => EnsureDirectoryPathString(path))
                .Bind(CreateNonEmpty)
                .Bind(CreateNonSneaky)
                .Bind(RelativePath.CheckRelativeConstraints)
                .Select(path => new NonSneakyRelativeDirectoryPath(path.TrimmedPathInternal));

        /// <summary>
        /// Tries to create a <see cref="NonSneakyRelativeFilePath"/> object.
        /// </summary>
        /// <param name="path">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, NonSneakyRelativeFilePath}"/> containing
        /// a new instance of <see cref="NonSneakyRelativeFilePath"/> if the path provided was a valid relative, non-sneaky file path;
        /// otherwise <see cref="Left{Exception, NonSneakyRelativeFilePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, NonSneakyRelativeFilePath> ReturnFilePath(string path)
            => path.ToExceptionEither()
                .FailIf(_ => path == null, new ArgumentNullException(nameof(path)))
                .Bind(_ => StringBase.CreateSome(path))
                .Bind(ReturnFilePath);

        /// <summary>
        /// Tries to create a <see cref="NonSneakyRelativeFilePath"/> object.
        /// </summary>
        /// <param name="path">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, NonSneakyRelativeFilePath}"/> containing
        /// a new instance of <see cref="NonSneakyRelativeFilePath"/> if the path provided was a valid relative, non-sneaky file path;
        /// otherwise <see cref="Left{Exception, NonSneakyRelativeFilePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, NonSneakyRelativeFilePath> ReturnFilePath(SomeString path)
            => path.ToExceptionEither()
                .FailIf(_ => path == null, new ArgumentNullException(nameof(path)))
                .Bind(_ => GetTrimmedPathString(path))
                .Bind(CreateNonEmpty)
                .Bind(CreateNonSneaky)
                .Bind(RelativePath.CheckRelativeConstraints)
                .Select(path => new NonSneakyRelativeFilePath(path.TrimmedPathInternal));
    }
}