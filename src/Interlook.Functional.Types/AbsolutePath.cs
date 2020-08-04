using Interlook.Monads;
using Interlook.Text;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using io = System.IO;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]
namespace Interlook.Functional.Types
{
    /// <summary>
    /// A non-empty reference of a path containing a rooted directory and where path traversal with ".." is forbidden.
    /// </summary>
    public abstract class AbsolutePath : NonSneakyPath
    {
        internal AbsolutePath(SomeString trimmedPath, bool isDirectory) : base(trimmedPath, isDirectory)
        {
        }

        /// <summary>
        /// Tries to create a path object deriving from <see cref="AbsolutePath"/>,
        /// according to the provided path specifying a directory or a file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsolutePath}"/> containing
        /// either a new instance of <see cref="AbsoluteFilePath"/> if the path provided was a valid relative, non-sneaky file path
        /// or a new instance of <see cref="AbsoluteDirectoryPath"/> if the path provided was a valid relative, non-sneaky directory path;
        /// otherwise <see cref="Left{Exception, AbsolutePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, AbsolutePath> Create(string path)
            => StringBase.CreateSome(path)
                .Bind(Create);

        /// <summary>
        /// Tries to create a path object deriving from <see cref="AbsolutePath"/>,
        /// according to the provided path specifying a directory or a file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsolutePath}"/> containing
        /// either a new instance of <see cref="AbsoluteFilePath"/> if the path provided was a valid relative, non-sneaky file path
        /// or a new instance of <see cref="AbsoluteDirectoryPath"/> if the path provided was a valid relative, non-sneaky directory path;
        /// otherwise <see cref="Left{Exception, AbsolutePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, AbsolutePath> Create(SomeString path)
            => path.ToExceptionEither()
                .FailIf(_ => path == null, new ArgumentNullException(nameof(path)))
                .Bind(_ => CreateNonEmpty(path))
                .Bind(CreateAbsolute);

        /// <summary>
        /// Tries to create a <see cref="AbsoluteDirectoryPath"/> object.
        /// </summary>
        /// <param name="directoryPath">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsoluteDirectoryPath}"/> containing
        /// a new instance of <see cref="AbsoluteDirectoryPath"/> if the path provided was a valid relative, non-sneaky file path;
        /// otherwise <see cref="Left{Exception, AbsoluteDirectoryPath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, AbsoluteDirectoryPath> ReturnDirectoryPath(string directoryPath)
            => directoryPath.ToExceptionEither()
                .FailIf(_ => directoryPath == null, new ArgumentNullException(nameof(directoryPath)))
                .Bind(_ => StringBase.CreateSome(directoryPath))
                .Bind(ReturnDirectoryPath);

        /// <summary>
        /// Tries to create a <see cref="AbsoluteDirectoryPath"/> object.
        /// </summary>
        /// <param name="directoryPath">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsoluteDirectoryPath}"/> containing
        /// a new instance of <see cref="AbsoluteDirectoryPath"/> if the path provided was a valid relative, non-sneaky file path;
        /// otherwise <see cref="Left{Exception, AbsoluteDirectoryPath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, AbsoluteDirectoryPath> ReturnDirectoryPath(SomeString directoryPath)
            => directoryPath.ToExceptionEither()
                .FailIf(_ => directoryPath == null, new ArgumentNullException(nameof(directoryPath)))
                .Bind(_ => EnsureDirectoryPathString(directoryPath))
                .Bind(EnsureRootedNonSneakyPath)
                .Select(path => new AbsoluteDirectoryPath(path.Path));

        /// <summary>
        /// Tries to create a <see cref="AbsoluteFilePath"/> object.
        /// </summary>
        /// <param name="filePath">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsoluteFilePath}"/> containing
        /// a new instance of <see cref="AbsoluteFilePath"/> if the path provided was a valid relative, non-sneaky file path;
        /// otherwise <see cref="Left{Exception, AbsoluteFilePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, AbsoluteFilePath> ReturnFilePath(string filePath)
            => filePath.ToExceptionEither()
                .FailIf(_ => filePath == null, new ArgumentNullException(nameof(filePath)))
                .Bind(_ => StringBase.CreateSome(filePath))
                .Bind(ReturnFilePath);

        /// <summary>
        /// Tries to create a <see cref="AbsoluteFilePath"/> object.
        /// </summary>
        /// <param name="filePath">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsoluteFilePath}"/> containing
        /// a new instance of <see cref="AbsoluteFilePath"/> if the path provided was a valid relative, non-sneaky file path;
        /// otherwise <see cref="Left{Exception, AbsoluteFilePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, AbsoluteFilePath> ReturnFilePath(SomeString filePath)
            => filePath.ToExceptionEither()
                .FailIf(_ => filePath == null, new ArgumentNullException(nameof(filePath)))
                .Bind(_ => GetTrimmedPathString(filePath))
                .Bind(EnsureRootedNonSneakyPath)
                .Bind(validPath => Try.InvokeToExceptionEither(() => io.Path.GetDirectoryName(validPath.Path) ?? string.Empty)
                    .FailIf(parentDirString => parentDirString.IsNullOrEmpty(), new FileNotFoundException($"Path `{validPath.Path}` references no file with a parent directory."))
                    .Bind(parentDirString => ReturnDirectoryPath(parentDirString).AddOuterException(innerEx => new Exception("Invalid parent path of file.", innerEx))
                    .Select(parentDirPath => new AbsoluteFilePath(validPath.TrimmedPathInternal, parentDirPath))));

        internal static Either<Exception, AbsolutePath> CreateAbsolute(NonEmptyPath nonEmptyPath)
            => nonEmptyPath.IsDirectory
                        ? ReturnDirectoryPath(nonEmptyPath.Path).Select(p => (AbsolutePath)p)
                        : ReturnFilePath(nonEmptyPath.Path).Select(p => (AbsolutePath)p);

        internal static Either<Exception, NonSneakyPath> EnsureRootedNonSneakyPath(SomeString pathString)
            => CreateNonEmpty(pathString)
                .Bind(CreateNonSneaky)
                .FailIf(path => !io.Path.IsPathRooted(path), path => new ArgumentException($"Path '{path}' has no root directory and thus no absolute path.", nameof(pathString)));
    }
}