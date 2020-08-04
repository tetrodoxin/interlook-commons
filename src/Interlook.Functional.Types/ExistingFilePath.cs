using Interlook.Monads;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]
namespace Interlook.Functional.Types
{
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
}