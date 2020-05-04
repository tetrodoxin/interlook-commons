using Interlook.Monads;
using Interlook.Text;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using io = System.IO;

namespace Interlook.Functional.Types
{
    /// <summary>
    /// Interface for path-reference-string objects.
    /// </summary>
    public interface IPath
    {
        /// <summary>Gets the path as <see cref="string"/></summary>
        string Path { get; }
    }

    /// <summary>
    /// A non-empty reference of a path of a directory, containing no path traversal with "..",
    /// having a rooted directory and always ending with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    /// <seealso cref="NonSneakyPath" />
    public class AbsoluteDirectoryPath : AbsolutePath
    {
        internal AbsoluteDirectoryPath(SomeString trimmedPath) : base(trimmedPath, true)
        {
        }

        /// <summary>
        /// Binds a function to the referenced directory, which gets
        /// an instance of <see cref="ExistingDirectoryPath"/>, containing the path
        /// of the directory if it already exists or was created successfully.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The function to bin to the created/existing directory. If it throws an exception, this exception will be encapsulated by the return value.</param>
        /// <returns>The <see cref="Either{Exception, TResult}"/> object, returned by <paramref name="func"/>.</returns>
        public Either<Exception, TResult> BindCreatedDirectory<TResult>(Func<ExistingDirectoryPath, Either<Exception, TResult>> func)
            => func.ToExceptionEither()
                .FailIf(_ => func == null, new ArgumentNullException(nameof(func)))
                .FailIf(_ => File.Exists(TrimmedPathInternal),
                    new FileNotFoundException("Provided path specifies an existing file rather than a directory."))
                .Bind(_ => Try.InvokeToExceptionEither(() => io.Directory.CreateDirectory(Path)))
                .Select(_ => new ExistingDirectoryPath(this))
                .Bind(existingDirectory => Try.Invoke(() => func(existingDirectory))
                    .MapTry(succ => succ, err => Either.Left<Exception, TResult>(err is Exception ex ? ex : new Exception(err?.ToString() ?? string.Empty))));

        /// <summary>
        /// Binds a function to the referenced directory, which gets
        /// an instance of <see cref="ExistingDirectoryPath"/>, containing the path
        /// of the directory if it already exists.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The function to bind to the already existing directory. If it throws an exception, this exception will be encapsulated by the return value.</param>
        /// <returns>The <see cref="Either{Exception, TResult}"/> object, returned by <paramref name="func"/>.</returns>
        public Either<Exception, TResult> BindExistingDirectory<TResult>(Func<ExistingDirectoryPath, Either<Exception, TResult>> func)
            => this.ToExceptionEither()
                .FailIf(_ => func == null, new ArgumentNullException(nameof(func)))
                .FailIf(_ => io.File.Exists(TrimmedPathInternal),
                    new FileNotFoundException("Provided path specifies an existing file rather than a directory."))
                .FailIf(_ => !io.Directory.Exists(Path),
                    new io.FileNotFoundException($"Directory `{Path}` does not exist.", Path))
                .Select(_ => new ExistingDirectoryPath(this))
                .Bind(existingDirectory => Try.Invoke(() => func(existingDirectory))
                    .MapTry(succ => succ, err => Either.Left<Exception, TResult>(err is Exception ex ? ex : new Exception(err?.ToString() ?? string.Empty))));

        /// <summary>
        /// Combines the direcotry path with a filename to an <see cref="AbsoluteFilePath"/>.
        /// </summary>
        /// <param name="file">The filename.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsoluteFilePath}"/> containing
        /// a new instance of <see cref="AbsoluteFilePath"/> if no error occured;
        /// otherwise <see cref="Left{Exception, AbsoluteFilePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public Either<Exception, AbsoluteFilePath> Combine(FileName file)
            => file.ToExceptionEither()
                .FailIf(f => f == null, new ArgumentNullException(nameof(file)))
                .Select(_ => new AbsoluteFilePath(CombinePathName(file.Name), this));

        /// <summary>
        /// Combines the direcotry path with a <see cref="NonSneakyRelativeDirectoryPath"/>
        /// to an <see cref="AbsoluteDirectoryPath"/>.
        /// </summary>
        /// <param name="nonSneakyRelativePath">The non-sneaky, relative directory path.</param>
        /// <returns>A new instance of <see cref="AbsoluteDirectoryPath"/> or this instance, 
        /// if <paramref name="nonSneakyRelativePath"/> was <c>null</c>.</returns>
        public AbsoluteDirectoryPath Combine(NonSneakyRelativeDirectoryPath nonSneakyRelativePath)
            => nonSneakyRelativePath == null ? this : new AbsoluteDirectoryPath(CombinePathName(nonSneakyRelativePath.Path));

        /// <summary>
        /// Combines the direcotry path with a <see cref="NonSneakyRelativeFilePath"/>
        /// to an <see cref="AbsoluteFilePath"/>.
        /// This method is deterministic.
        /// </summary>
        /// <param name="nonSneakyRelativePath">The non-sneaky, relative file path.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsoluteFilePath}"/> containing
        /// a new instance of <see cref="AbsoluteFilePath"/> if no error occured;
        /// otherwise <see cref="Left{Exception, AbsoluteFilePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public Either<Exception, AbsoluteFilePath> Combine(NonSneakyRelativeFilePath nonSneakyRelativePath)
            => nonSneakyRelativePath.ToExceptionEither()
                .FailIf(f => f == null, new ArgumentNullException(nameof(nonSneakyRelativePath)))
                .Select(_ => new AbsoluteFilePath(CombinePathName(nonSneakyRelativePath.Path), this));

        /// <summary>
        /// Tries to create an instance of <see cref="AbsoluteFilePath"/> containing
        /// the combination of the current path with a given relative directory path.
        /// </summary>
        /// <param name="relativeFilePath">The relative file path to append.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsoluteFilePath}"/> containing
        /// a new instance of <see cref="AbsoluteFilePath"/> if the directory path provided 
        /// in <paramref name="relativeFilePath"/> could be appended without errors;
        /// otherwise <see cref="Left{Exception, AbsoluteFilePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public Either<Exception, AbsoluteFilePath> Combine(RelativeFilePath relativeFilePath)
            => relativeFilePath.ToExceptionEither()
                .FailIf(fp => fp == null, new ArgumentNullException(nameof(relativeFilePath)))
                .Bind(_ => ReturnFilePath(CombinePathName(relativeFilePath.Path)));

        /// <summary>
        /// Tries to create an instance of <see cref="AbsolutePath"/> containing
        /// the combination of the current path with a given relative path.
        /// </summary>
        /// <param name="relativePath">The relative path to append.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsolutePath}"/> containing
        /// a new instance of <see cref="AbsolutePath"/> if the path provided 
        /// in <paramref name="relativePath"/> could be appended without errors;
        /// otherwise <see cref="Left{Exception, AbsolutePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public Either<Exception, AbsolutePath> Combine(RelativePath relativePath)
            => relativePath.ToExceptionEither()
                .FailIf(fp => fp == null, new ArgumentNullException(nameof(relativePath)))
                .Bind(_ => Create(CombinePathName(relativePath.Path)));


        /// <summary>
        /// Tries to return a path to the parent directory of the current path object.
        /// </summary>
        /// <returns>An instance of <see cref="Right{Exception, AbsoluteDirectoryPath}"/> containing
        /// a new instance of <see cref="AbsoluteDirectoryPath"/> the parent directory 
        /// could be determined without errors;
        /// otherwise <see cref="Left{Exception, AbsoluteDirectoryPath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public Either<Exception, AbsoluteDirectoryPath> GetParentPath()
            => Try.InvokeToExceptionEither(() => io.Path.GetDirectoryName(TrimmedPathInternal))
                .Bind(parentPath => ReturnDirectoryPath(parentPath ?? string.Empty));
    }

    /// <summary>
    /// A non-empty reference of a path of a file, containing no path traversal with "..",
    /// having a rooted directory and never ending with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    /// <seealso cref="NonSneakyPath" />
    public class AbsoluteFilePath : AbsolutePath
    {
        /// <summary>
        /// Returns the path to the directory, that contains the file.
        /// </summary>
        public AbsoluteDirectoryPath Directory { get; protected set; }

        internal AbsoluteFilePath(SomeString trimmedPath, AbsoluteDirectoryPath directory) : base(trimmedPath, false)
        {
            Directory = directory;
        }


        /// <summary>
        /// Binds a function to the referenced file, which gets
        /// an instance of <see cref="ExistingDirectoryPath"/>, containing
        /// the path if the file already exists.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The function to bind to the already existing file. If it throws an exception, this exception will be encapsulated by the return value.</param>
        /// <returns>The <see cref="Either{Exception, TResult}"/> object, returned by <paramref name="func"/>.</returns>
        public Either<Exception, TResult> BindExistingFile<TResult>(Func<ExistingFilePath, Either<Exception, TResult>> func)
            => this.ToExceptionEither()
                .FailIf(_ => func==null, new ArgumentNullException(nameof(func)))
                .FailIf(_ => io.Directory.Exists(CombinePathName(io.Path.DirectorySeparatorChar)),
                    new FileNotFoundException("Provided path specifies an existing directory rather than a file."))
                .FailIf(_ => !io.File.Exists(Path),
                    new io.FileNotFoundException($"File `{Path}` does not exist.", Path))
                .Select(_ => new ExistingFilePath(this))
                .Bind(existingFile => Try.Invoke(() => func(existingFile))
                    .MapTry(succ => succ, err => Either.Left<Exception, TResult>(err is Exception ex ? ex : new Exception(err?.ToString() ?? string.Empty))));
    }

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

    /// <summary>
    /// An absolute path to an existing directory, containing no path traversal with "..",
    /// always ending with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    /// <seealso cref="NonSneakyPath" />
    /// <seealso cref="AbsoluteDirectoryPath" />
    public sealed class ExistingDirectoryPath : AbsoluteDirectoryPath
    {
        internal ExistingDirectoryPath(AbsoluteDirectoryPath path) : base(path.TrimmedPathInternal)
        { }


        /// <summary>
        /// Binds a function to a directory, referenced by a specified path, if it already exists or was created successfully.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="directoryPath">The path as string.</param>
        /// <param name="func">The function to bind to the already existing directory. If it throws an exception, this exception will be encapsulated by the return value.</param>
        /// <returns>The <see cref="Either{Exception, TResult}"/> object, returned by <paramref name="func"/>.</returns>
        public static Either<Exception, TResult> BindCreatedDirectory<TResult>(SomeString directoryPath, Func<ExistingDirectoryPath, Either<Exception, TResult>> func)
            => directoryPath.ToExceptionEither()
                .FailIf(_ => directoryPath == null, new ArgumentNullException(nameof(directoryPath)))
                .FailIf(_ => func == null, new ArgumentNullException(nameof(func)))
                .Bind(_ => ReturnDirectoryPath(directoryPath))
                .BindCreatedDirectory(func);


        /// <summary>
        /// Binds a function to a directory, referenced by a specified path, if it already exists or was created successfully.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="directoryPath">The path as string.</param>
        /// <param name="func">The function to bind to the already existing directory. If it throws an exception, this exception will be encapsulated by the return value.</param>
        /// <returns>The <see cref="Either{Exception, TResult}"/> object, returned by <paramref name="func"/>.</returns>
        public static Either<Exception, TResult> BindCreatedDirectory<TResult>(string directoryPath, Func<ExistingDirectoryPath, Either<Exception, TResult>> func)
            => directoryPath.ToExceptionEither()
                .FailIf(_ => directoryPath == null, new ArgumentNullException(nameof(directoryPath)))
                .FailIf(_ => func == null, new ArgumentNullException(nameof(func)))
                .Bind(_ => ReturnDirectoryPath(directoryPath))
                .BindCreatedDirectory(func);

        /// <summary>
        /// Binds a function to a directory, referenced by a specified path, if that directory already exists.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="directoryPath">The path as string.</param>
        /// <param name="func">The function to bind to the already existing directory. If it throws an exception, this exception will be encapsulated by the return value.</param>
        /// <returns>The <see cref="Either{Exception, TResult}"/> object, returned by <paramref name="func"/>.</returns>
        public static Either<Exception, TResult> BindExistingDirectory<TResult>(SomeString directoryPath, Func<ExistingDirectoryPath, Either<Exception, TResult>> func)
            => directoryPath.ToExceptionEither()
                .FailIf(_ => directoryPath == null, new ArgumentNullException(nameof(directoryPath)))
                .FailIf(_ => func == null, new ArgumentNullException(nameof(func)))
                .Bind(_ => ReturnDirectoryPath(directoryPath))
                .BindExistingDirectory(func);

        /// <summary>
        /// Binds a function to a directory, referenced by a specified path, if that directory already exists.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="directoryPath">The path as string.</param>
        /// <param name="func">The function to bind to the already existing directory. If it throws an exception, this exception will be encapsulated by the return value.</param>
        /// <returns>The <see cref="Either{Exception, TResult}"/> object, returned by <paramref name="func"/>.</returns>
        public static Either<Exception, TResult> BindExistingDirectory<TResult>(string directoryPath, Func<ExistingDirectoryPath, Either<Exception, TResult>> func)
            => directoryPath.ToExceptionEither()
                .FailIf(_ => directoryPath == null, new ArgumentNullException(nameof(directoryPath)))
                .FailIf(_ => func == null, new ArgumentNullException(nameof(func)))
                .Bind(_ => ReturnDirectoryPath(directoryPath))
                .BindExistingDirectory(func);

    }

    /// <summary>
    /// An absolute path to an existing file, containing no path traversal with "..",
    /// never ending with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    /// <seealso cref="NonSneakyPath" />
    /// <seealso cref="AbsoluteFilePath" />
    public sealed class ExistingFilePath : AbsoluteFilePath
    {
        internal ExistingFilePath(AbsoluteFilePath path) : base(path.TrimmedPathInternal, path.Directory)
        { }

        /// <summary>
        /// Binds a function to a file, referenced by a specified path, if that file exists.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="filePath">The path as string.</param>
        /// <param name="func">The function to bind to the already existing file. If it throws an exception, this exception will be encapsulated by the return value.</param>
        /// <returns>The <see cref="Either{Exception, TResult}"/> object, returned by <paramref name="func"/>.</returns>
        public static Either<Exception, TResult> BindExistingFile<TResult>(SomeString filePath, Func<ExistingFilePath, Either<Exception, TResult>> func)
            => filePath.ToExceptionEither()
                .FailIf(_ => filePath == null, new ArgumentNullException(nameof(filePath)))
                .FailIf(_ => func == null, new ArgumentNullException(nameof(func)))
                .Bind(_ => ReturnFilePath(filePath))
                .BindExistingFile(func);


        /// <summary>
        /// Binds a function to a file, referenced by a specified path, if that file exists.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="filePath">The path as string.</param>
        /// <param name="func">The function to bind to the already existing file. If it throws an exception, this exception will be encapsulated by the return value.</param>
        /// <returns>The <see cref="Either{Exception, TResult}"/> object, returned by <paramref name="func"/>.</returns>
        public static Either<Exception, TResult> BindExistingFile<TResult>(string filePath, Func<ExistingFilePath, Either<Exception, TResult>> func)
            => filePath.ToExceptionEither()
                .FailIf(_ => filePath == null, new ArgumentNullException(nameof(filePath)))
                .FailIf(_ => func == null, new ArgumentNullException(nameof(func)))
                .Bind(_ => ReturnFilePath(filePath))
                .BindExistingFile(func);
    }

    /// <summary>
    /// A type representing a valid file name,
    /// thus being non-empty and not containing
    /// characters, that are not valid for file names (see <see cref="Path.GetInvalidFileNameChars"/>)
    /// </summary>
    public class FileName
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        public SomeString Name { get; }

        private FileName(SomeString name) => Name = name;

        /// <summary>
        /// Tries to create an instance of <see cref="FileName"/>
        /// </summary>
        /// <param name="name">The name of the file. Must not be empty and not conatin any characters returned by <see cref="Path.GetInvalidFileNameChars"/></param>
        /// <returns>An instance of <see cref="Right{Exception, FileName}"/> containing
        /// a new instance of <see cref="FileName"/> if the path provided was a valid file name;
        /// otherwise <see cref="Left{Exception, FileName}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, FileName> Create(string name)
            => name.ToExceptionEither()
                .FailIf(_ => name == null, new ArgumentNullException(nameof(name)))
                .Bind(_ => StringBase.CreateSome(name))
                .Bind(Create);

        /// <summary>
        /// Tries to create an instance of <see cref="FileName"/>
        /// </summary>
        /// <param name="name">The name of the file. Must not be empty and not conatin any characters returned by <see cref="Path.GetInvalidFileNameChars"/></param>
        /// <returns>An instance of <see cref="Right{Exception, FileName}"/> containing
        /// a new instance of <see cref="FileName"/> if the path provided was a valid file name;
        /// otherwise <see cref="Left{Exception, FileName}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, FileName> Create(SomeString name)
            => name.ToExceptionEither()
                .FailIf(_ => name == null, new ArgumentNullException(nameof(name)))
                .Bind(fileName => fileName.Value.IndexOfAny(io.Path.GetInvalidFileNameChars()).ToExceptionEither()
                    .FailIf(illegalPos => illegalPos >= 0, illegalPos => new FormatException($"Filename contains illegal character at position {illegalPos}"))
                    .Select(_ => new FileName(fileName)));
    }

    /// <summary>
    /// Base type of non-empty path references.
    /// </summary>
    /// <seealso cref="IPath" />
    public class NonEmptyPath : IPath
    {
        private static readonly char[] _invalidPathChars =
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
            .Bind(origPath => origPath.Value.IndexOfAny(_invalidPathChars).ToExceptionEither()
                .FailIf(pos => pos >= 0, pos => new ArgumentException($"Path contains invalid character '{origPath.Value[pos]}' at position {pos}.", nameof(path)))
                .Select(pos => origPath))
            .Bind(origPath => GetTrimmedPathString(origPath).Select(trimmedPath => (Path: trimmedPath, IsDirectory: EndsInDirectorySeparator(origPath))))
            .FailIf(p => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && pathEndsWithPeriodOrSpace(p.Path), p => new ArgumentException("In Windows paths must not end with '.'", nameof(path)))
            .Select(p => new NonEmptyPath(p.Path, p.IsDirectory));

        internal static bool EndsInDirectorySeparator(SomeString path) => isDirectorySeparatorChar(path[-1]);

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

    /// <summary>
    /// A non-empty path reference, where path traversal with ".." is forbidden.
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    public class NonSneakyPath : NonEmptyPath
    {
        private const string DoubleDot = "..";
        private static string SneakyBegin = DoubleDot + io.Path.DirectorySeparatorChar;
        private static string SneakyEnd = io.Path.DirectorySeparatorChar + DoubleDot;
        private static string SneakyMiddle = string.Concat(io.Path.DirectorySeparatorChar, DoubleDot, io.Path.DirectorySeparatorChar);

        internal NonSneakyPath(SomeString trimmedPath, bool isDirectory) : base(trimmedPath, isDirectory)
        {
        }

        internal static Either<Exception, NonSneakyPath> CreateNonSneaky(NonEmptyPath path)
                    => path.ToExceptionEither()
            .FailIf(p => isSneakyPath(p), p => new ArgumentException($"'{p}' is a sneaky path (contains '..'-notation).", nameof(path)))
            .Select(p => new NonSneakyPath(p.TrimmedPathInternal, p.IsDirectory));
       
        private static bool isSneakyPath(NonEmptyPath path)
            => path.Path.StartsWith(SneakyBegin)
                || path.Path.EndsWith(SneakyEnd)
                || path.Path.Contains(SneakyMiddle)
                || path.Path.Equals(DoubleDot);
    }

    /// <summary>
    /// A non-empty reference of a relative path of a directory, containing no path traversal with "..",
    /// having no rooted directory and always ending with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonSneakyRelativePath" />
    public sealed class NonSneakyRelativeDirectoryPath : NonSneakyRelativePath
    {
        internal NonSneakyRelativeDirectoryPath(SomeString trimmedPath) : base(trimmedPath, true)
        {
        }
    }

    /// <summary>
    /// A non-empty reference of a relative path of a file, containing no path traversal with "..",
    /// having no rooted directory and never ending with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonSneakyRelativePath" />
    public sealed class NonSneakyRelativeFilePath : NonSneakyRelativePath
    {
        internal NonSneakyRelativeFilePath(SomeString trimmedPath) : base(trimmedPath, false)
        {
        }
    }

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

    /// <summary>
    /// A non-empty reference of a relative path of a directory, thus having no rooted directory
    /// and always ends with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    public sealed class RelativeDirectoryPath : RelativePath
    {
        internal RelativeDirectoryPath(SomeString trimmedPath) : base(trimmedPath, true)
        {
        }
    }

    /// <summary>
    /// A non-empty reference of a relative path of a file, thus having no rooted directory
    /// and does not end with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    public sealed class RelativeFilePath : RelativePath
    {
        internal RelativeFilePath(SomeString trimmedPath) : base(trimmedPath, false)
        {
        }
    }

    /// <summary>
    /// A non-empty reference of a relative path, thus having no rooted directory.
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    public class RelativePath : NonEmptyPath
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