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

namespace Interlook.Functional.Types
{
    /// <summary>
    /// A non-empty reference of a path of a file,
    /// having a rooted directory and never ending with <see cref="Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    public sealed class AbsoluteFilePath : AbsolutePath
    {
        /// <summary>
        /// Returns the full path of the file.
        /// </summary>
        public SomeString FullPath;

        /// <summary>
        /// Returns the path to the directory, that contains the file.
        /// </summary>
        public AbsoluteDirectoryPath Directory { get; }

        /// <summary>
        /// The name of the file itself (leaf of the path, without any separator, but with extension)
        /// </summary>
        public FileName Name { get; }

        internal AbsoluteFilePath(FileName name, AbsoluteDirectoryPath directory)
        {
            Name = name;
            Directory = directory;
            FullPath = directory.Path.Concat(name.Name);
        }

        /// <summary>
        /// Tries to create a <see cref="AbsoluteFilePath"/> object.
        /// </summary>
        /// <param name="filePath">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsoluteFilePath}"/> containing
        /// a new instance of <see cref="AbsoluteFilePath"/> if the path provided was a valid absolute file path;
        /// otherwise <see cref="Left{Exception, AbsoluteFilePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static new Either<Exception, AbsoluteFilePath> Return(string filePath)
            => filePath.ToExceptionEither()
                .Bind(SomeString.Create)
                .Bind(Return);

        /// <summary>
        /// Tries to create a <see cref="AbsoluteFilePath"/> object.
        /// </summary>
        /// <param name="filePath">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsoluteFilePath}"/> containing
        /// a new instance of <see cref="AbsoluteFilePath"/> if the path provided was a valid absolute file path;
        /// otherwise <see cref="Left{Exception, AbsoluteFilePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static new Either<Exception, AbsoluteFilePath> Return(SomeString filePath)
            => filePath.ToExceptionEither()
                .Bind(NonEmptyPathString.Create)
                .FailIf(path => path.EndsInDirectorySeparator(), new FormatException("Provided path seems to specify a directory rather than a file"))
                .Bind(CheckRootedPath)
                .Bind(path => GetDirectoryPath(path.TrimmedPath)
                    .Bind(dirPath => AbsoluteDirectoryPath.Return(dirPath)
                    .Bind(dir => GetFileName(path.TrimmedPath)
                        .Select(fileName => new AbsoluteFilePath(fileName, dir).ToExceptionEither())
                        .GetValue(Either.Left<Exception, AbsoluteFilePath>(new ArgumentException("No directory name found."))))));

        /// <summary>
        /// Returns the path as <see cref="NonEmptyString" />
        /// </summary>
        /// <returns></returns>
        protected override NonEmptyString GetStringValue() => FullPath;

        /// <summary>
        /// Determines whether the file exists.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the caller has the required permissions and the path contains the name of
        /// an existing file; otherwise, <c>false</c>. If the caller does not have sufficient
        /// permissions to read the file, no exception is thrown and the method
        /// returns false regardless of the existence of the file path.
        /// </returns>
        /// <seealso cref="System.IO.File.Exists(string)"/>
        public bool Exists() => File.Exists(FullPath);
    }
}