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
}