using Interlook.Monads;
using System;

namespace Interlook.Functional.Types
{
    /// <summary>
    /// Contains extension methods for dealing with paths.
    /// </summary>
    public static class PathExtensions
    {
        /// <summary>
        /// Binds a function to the referenced directory, if it already exists or was created successfully.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="either">The either object, possibly containing the path to a directory.</param>
        /// <param name="func">The function to bind to the already existing directory. If it throws an exception, this exception will be encapsulated by the return value.</param>
        /// <returns>The <see cref="Either{Exception, TResult}"/> object, returned by <paramref name="func"/>.</returns>
        public static Either<Exception, TResult> BindCreatedDirectory<TResult>(this Either<Exception, AbsoluteDirectoryPath> either, Func<ExistingDirectoryPath, Either<Exception, TResult>> func)
            => either.Bind(absolute => absolute.BindCreatedDirectory(func));

        /// <summary>
        /// Binds a function to the referenced directory, if it already exists.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="either">The either object, possibly containing the path to a directory.</param>
        /// <param name="func">The function to bind to the already existing directory. If it throws an exception, this exception will be encapsulated by the return value.</param>
        /// <returns>The <see cref="Either{Exception, TResult}"/> object, returned by <paramref name="func"/>.</returns>
        public static Either<Exception, TResult> BindExistingDirectory<TResult>(this Either<Exception, AbsoluteDirectoryPath> either, Func<ExistingDirectoryPath, Either<Exception, TResult>> func)
            => either.Bind(absolute => absolute.BindExistingDirectory(func));

        /// <summary>
        /// Binds a function to the referenced file, if it already exists.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="either">The either object, possibly containing the path to a file.</param>
        /// <param name="func">The function to bind to the already existing file. If it throws an exception, this exception will be encapsulated by the return value.</param>
        /// <returns>The <see cref="Either{Exception, TResult}"/> object, returned by <paramref name="func"/>.</returns>
        public static Either<Exception, TResult> BindExistingFile<TResult>(this Either<Exception, AbsoluteFilePath> either, Func<ExistingFilePath, Either<Exception, TResult>> func)
            => either.Bind(absolute => absolute.BindExistingFile(func));

        /// <summary>
        /// Gets the directory path of an <see cref="AbsolutePath"/> object,
        /// which is the path value of an <see cref="AbsoluteDirectoryPath"/> itself
        /// or the directory containing the file of an <see cref="AbsoluteFilePath"/>.
        /// </summary>
        /// <param name="path">The path get the directory path from.</param>
        /// <returns></returns>
        public static AbsoluteDirectoryPath GetDirectory(this AbsolutePath path)
            => path switch
            {
                AbsoluteDirectoryPath dir => dir,
                AbsoluteFilePath file => file.Directory,
                AbsolutePath a => new AbsoluteDirectoryPath(path.TrimmedPathInternal)
            };
    }
}