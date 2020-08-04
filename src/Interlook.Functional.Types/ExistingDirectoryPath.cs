using Interlook.Monads;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]
namespace Interlook.Functional.Types
{
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
}