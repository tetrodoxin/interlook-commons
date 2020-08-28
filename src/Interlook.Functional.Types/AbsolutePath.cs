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
using System.Linq;
using System.Runtime.CompilerServices;
using io = System.IO;

namespace Interlook.Functional.Types
{
    /// <summary>
    /// Base type of a non-empty reference of a path
    /// containing a rooted directory.
    /// </summary>
    public abstract class AbsolutePath : NonEmptyPath
    {
        internal AbsolutePath()
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
        public static Either<Exception, AbsolutePath> Return(string path)
            => SomeString.Create(path)
                .Bind(Return);

        /// <summary>
        /// Tries to create a path object deriving from <see cref="AbsolutePath"/>,
        /// according to the provided path specifying a directory or a file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>An instance of <see cref="Right{Exception, AbsolutePath}"/> containing
        /// either a new instance of <see cref="AbsoluteFilePath"/> if the path provided was a valid relative, non-sneaky file path
        /// or a new instance of <see cref="AbsoluteDirectoryPath"/> if the path provided was a valid relative, non-sneaky directory path;
        /// otherwise <see cref="Left{Exception, AbsolutePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, AbsolutePath> Return(SomeString path)
            => path.ToExceptionEither()
                .FailIf(_ => path == null, new ArgumentNullException(nameof(path)))
                .Bind(NonEmptyPathString.Create)
                .Bind(Return);

        internal static Either<Exception, NonEmptyPathString> CheckRootedPath(NonEmptyPathString pathString)
            => pathString.ToExceptionEither()
                .FailIf(path => !io.Path.IsPathRooted(path.Path), path => new ArgumentException($"Path '{path.Path}' has no root directory and thus no absolute path.", nameof(pathString)));

        internal static Either<Exception, AbsolutePath> Return(NonEmptyPathString nonEmptyPath)
                    => nonEmptyPath.EndsInDirectorySeparator()
                        ? AbsoluteDirectoryPath.Return(nonEmptyPath.Path).Select(p => (AbsolutePath)p)
                        : AbsoluteFilePath.Return(nonEmptyPath.Path).Select(p => (AbsolutePath)p);
    }
}