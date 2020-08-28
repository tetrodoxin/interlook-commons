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
    /// A non-empty reference of a relative path of a file, thus having no rooted directory
    /// and does not end with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    public sealed class RelativeFilePath : RelativePath
    {
        /// <summary>
        /// The name of the File, with extension (leaf of the path, without any separator)
        /// </summary>
        public FileName Name { get; }

        /// <summary>
        /// The complete path of the file
        /// </summary>
        public NonEmptyString Path { get; }

        internal RelativeFilePath(NonEmptyString path, FileName name)
        {
            Name = name;
            Path = path;
        }

        /// <summary>
        /// Tries to create a <see cref="RelativeFilePath"/> object.
        /// </summary>
        /// <param name="path">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, RelativeFilePath}"/> containing
        /// a new instance of <see cref="RelativeFilePath"/> if the path provided was a valid relative file path;
        /// otherwise <see cref="Left{Exception, RelativeFilePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, RelativeFilePath> Return(string path)
            => path.ToExceptionEither()
                .Bind(NonEmptyString.Create)
                .Bind(Return);

        /// <summary>
        /// Tries to create a <see cref="RelativeFilePath"/> object.
        /// </summary>
        /// <param name="path">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, RelativeFilePath}"/> containing
        /// a new instance of <see cref="RelativeFilePath"/> if the path provided was a valid relative file path;
        /// otherwise <see cref="Left{Exception, RelativeFilePath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, RelativeFilePath> Return(NonEmptyString path)
            => path.ToExceptionEither()
                .Bind(NonEmptyPathString.Create)
                .Bind(CheckRelativeConstraints)
                .Bind(path => GetFileName(path.TrimmedPath)
                    .Select(name => (Path: path.Path, Name: name).ToExceptionEither())
                    .GetValue(Either.Left<Exception, (NonEmptyString Path, FileName Name)>(new ArgumentException("No directory name found."))))
                .Select(p => new RelativeFilePath(p.Path, p.Name));

        /// <summary>
        /// Returns the path as <see cref="NonEmptyString" /></summary>
        /// <returns></returns>
        protected override NonEmptyString GetStringValue() => Path;

        /// <summary>
        /// Determines whether the file exists, relative to the current directory.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the caller has the required permissions and the path contains the name of
        /// an existing file; otherwise, <c>false</c>. If the caller does not have sufficient
        /// permissions to read the file, no exception is thrown and the method
        /// returns false regardless of the existence of the file path.
        /// </returns>
        /// <seealso cref="System.IO.File.Exists(string)"/>
        public bool Exists()
        {
            try
            {
                var fullPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), Path);
                return File.Exists(fullPath);
            }
            catch
            {
                return false;
            }
        }
    }
}