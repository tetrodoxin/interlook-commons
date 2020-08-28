#region license

//MIT License

//Copyright(c) 2013-2020 Andreas Hübner

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

#endregion 
using Interlook.Monads;
using System;
using System.IO;
using System.Linq;

using io = System.IO;

namespace Interlook.Functional.Types
{
    /// <summary>
    /// A non-empty reference of a path of a directory,
    /// that is rooted and always ending with <see cref="Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    public sealed class AbsoluteDirectoryPath : AbsolutePath
    {
        private NonEmptyPathString _path;

        /// <summary>
        /// The name of the directory (leaf of the path, without any separator)
        /// </summary>
        public NonEmptyString Name { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a root itself.
        /// <para>
        /// For root directories, the <see cref="Name"/> property strictly
        /// would be empty, but is equal to <see cref="Path"/> in this special case.
        /// </para>
        /// </summary>
        public bool IsRoot { get; }

        /// <summary>
        /// The complete absolute path of the directory,
        /// always ending with the <see cref="Path.DirectorySeparatorChar"/>
        /// </summary>
        public SomeString Path { get; }

        internal AbsoluteDirectoryPath(NonEmptyPathString path, NonEmptyString name, bool isRoot = false)
        {
            Name = name;
            Path = path.GetPathWithEnsuredDirectorySeparatorTail();
            _path = path;
            IsRoot = isRoot;
        }

        /// <summary>
        /// Tries to create a <see cref="AbsoluteDirectoryPath"/> object.
        /// </summary>
        /// <param name="directoryPath">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsoluteDirectoryPath}"/> containing
        /// a new instance of <see cref="AbsoluteDirectoryPath"/> if the path provided was a valid relative, non-sneaky file path;
        /// otherwise <see cref="Left{Exception, AbsoluteDirectoryPath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static new Either<Exception, AbsoluteDirectoryPath> Return(string directoryPath)
            => directoryPath.ToExceptionEither()
                .Bind(SomeString.Create)
                .Bind(Return);

        /// <summary>
        /// Tries to create a <see cref="AbsoluteDirectoryPath"/> object.
        /// </summary>
        /// <param name="directoryPath">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsoluteDirectoryPath}"/> containing
        /// a new instance of <see cref="AbsoluteDirectoryPath"/> if the path provided was a valid relative, non-sneaky file path;
        /// otherwise <see cref="Left{Exception, AbsoluteDirectoryPath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static new Either<Exception, AbsoluteDirectoryPath> Return(SomeString directoryPath)
            => directoryPath.ToExceptionEither()
                .Bind(NonEmptyPathString.Create)
                .Bind(CheckRootedPath)
                .Bind(path => GetFileName(path.TrimmedPath)
                    .Select(name => new AbsoluteDirectoryPath(path, name.Name).ToExceptionEither())
                    .GetValue(path.RootLength == path.Path.Length
                        ? new AbsoluteDirectoryPath(path, path.Path, true).ToExceptionEither()
                        : new ArgumentException("No directory name found.").ToExceptionEitherLeft<AbsoluteDirectoryPath>()));

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
                .Select(_ => new AbsoluteFilePath(file, this));

        /// <summary>
        /// Combines the direcotry path with a <see cref="RelativeDirectoryPath"/>
        /// to an <see cref="AbsoluteDirectoryPath"/>.
        /// </summary>
        /// <param name="relativeDirectoryPath">The non-sneaky, relative directory path.</param>
        /// <returns>A new instance of <see cref="AbsoluteDirectoryPath"/> or this instance,
        /// if <paramref name="relativeDirectoryPath"/> was <c>null</c>.</returns>
        public AbsoluteDirectoryPath Combine(RelativeDirectoryPath relativeDirectoryPath)
        {
            var combinedPathString = _path.Combine(relativeDirectoryPath.Path);
            return relativeDirectoryPath == null ? this : new AbsoluteDirectoryPath(combinedPathString, relativeDirectoryPath.Name);
        }

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
                .Bind(fp => AbsoluteFilePath.Return(Path.Concat(fp.Path)));

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
                .FailIf(rp => rp == null, new ArgumentNullException(nameof(relativePath)))
                .Bind(rp => rp switch
                {
                    RelativeFilePath fp => Combine(fp).Select(p => (AbsolutePath)p),
                    RelativeDirectoryPath dp => ((AbsolutePath)Combine(dp)).ToExceptionEither(),
                    _ => Either.Left<Exception, AbsolutePath>(new NotImplementedException())
                });

        /// <summary>
        /// Tries to return a path to the parent directory of the current path object.
        /// </summary>
        /// <returns>An instance of <see cref="Right{Exception, AbsoluteDirectoryPath}"/> containing
        /// a new instance of <see cref="AbsoluteDirectoryPath"/> the parent directory
        /// could be determined without errors;
        /// otherwise <see cref="Left{Exception, AbsoluteDirectoryPath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public Either<Exception, AbsoluteDirectoryPath> GetParentPath()
            => Try.InvokeToExceptionEither(() => io.Path.GetDirectoryName(_path.TrimmedPath))
                .Bind(parentPath => Return(parentPath ?? string.Empty));

        /// <summary>
        /// Returns the path as <see cref="NonEmptyString" />
        /// </summary>
        /// <returns></returns>
        protected override NonEmptyString GetStringValue() => Path;

        /// <summary>
        /// Determines whether the directory exists.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the path refers to an existing directory; false if the directory does not
        /// exist or an error occurs when trying to determine if the specified directory exists.
        /// </returns>
        /// <seealso cref="System.IO.Directory.Exists(string)"/>
        public bool Exists() => Directory.Exists(Path);


        /// <summary>
        /// Checks if the directory exists and tries to create it otherwise.
        /// </summary>
        /// <returns>
        /// A <see cref="Left{Exception, Unit}"/> instance containing
        /// an exception, that occured while trying to create the directory.
        /// </returns>
        /// <seealso cref="System.IO.Directory.Exists(string)"/>
        public Either<Exception, Unit> EnsureExist()
        {
            if(!Directory.Exists(Path))
            {
                return Try.InvokeToExceptionEither(() => Directory.CreateDirectory(Path))
                    .Select(_ => Unit.Default);
            }

            return Unit.Default.ToExceptionEither();
        }
    }
}