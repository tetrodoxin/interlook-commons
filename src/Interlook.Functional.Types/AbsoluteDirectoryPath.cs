using Interlook.Monads;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using io = System.IO;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]
namespace Interlook.Functional.Types
{
    /// <summary>
    /// A non-empty reference of a path of a directory, containing no path traversal with "..",
    /// having a rooted directory and always ending with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    /// <seealso cref="NonSneakyPath" />
    public class AbsoluteDirectoryPath : AbsolutePath
    {

        /// <summary>
        /// The name of the directory (leaf of the path, without any separator)
        /// </summary>
        public SomeString Name { get; }

        internal AbsoluteDirectoryPath(SomeString trimmedPath) : base(trimmedPath, true)
        {
            Name = Try.InvokeToExceptionEither(() => io.Path.GetFileName(trimmedPath))
                .Bind(dirname => StringBase.CreateSome(dirname))
                .MapEither(_ => trimmedPath, dirname => dirname);
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
            => nonSneakyRelativePath == null ? this : new AbsoluteDirectoryPath(CombinePathName(nonSneakyRelativePath.TrimmedPathInternal));

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
}