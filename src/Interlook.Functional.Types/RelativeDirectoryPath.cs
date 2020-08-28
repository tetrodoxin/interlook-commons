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
    /// A non-empty reference of a relative path of a directory, thus having no rooted directory
    /// and always ends with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    public sealed class RelativeDirectoryPath : RelativePath
    {
        /// <summary>
        /// The name of the directory (leaf of the path, without any separator)
        /// </summary>
        public NonEmptyString Name { get; }

        /// <summary>
        /// The complete path of the directory,
        /// always ending with the <see cref="System.IO.Path.DirectorySeparatorChar"/>
        /// </summary>
        public SomeString Path { get; }

        internal RelativeDirectoryPath(SomeString path, NonEmptyString name)
        {
            Name = name;
            Path = path;
        }

        /// <summary>
        /// Tries to create a <see cref="RelativeDirectoryPath"/> object.
        /// </summary>
        /// <param name="path">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, RelativeDirectoryPath}"/> containing
        /// a new instance of <see cref="RelativeDirectoryPath"/> if the path provided was a valid relative directory path;
        /// otherwise <see cref="Left{Exception, RelativeDirectoryPath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, RelativeDirectoryPath> Return(string path)
            => path.ToExceptionEither()
                .Bind(NonEmptyString.Create)
                .Bind(Return);

        /// <summary>
        /// Tries to create a <see cref="RelativeDirectoryPath"/> object.
        /// </summary>
        /// <param name="path">The path as string.</param>
        /// <returns>An instance of <see cref="Right{Exception, RelativeDirectoryPath}"/> containing
        /// a new instance of <see cref="RelativeDirectoryPath"/> if the path provided was a valid relative directory path;
        /// otherwise <see cref="Left{Exception, RelativeDirectoryPath}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, RelativeDirectoryPath> Return(NonEmptyString path)
            => path.ToExceptionEither()
                .Bind(NonEmptyPathString.Create)
                .Bind(CheckRelativeConstraints)
                .Bind(path => GetFileName(path.TrimmedPath)
                    .Select(name => (Path: path.GetPathWithEnsuredDirectorySeparatorTail(), name.Name).ToExceptionEither())
                    .GetValue(Either.Left<Exception, (SomeString Path, NonEmptyString Name)>(new ArgumentException("No directory name found."))))
                .Select(p => new RelativeDirectoryPath(p.Path, p.Name));

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
        public bool Exists()
        {
            try
            {
                var fullPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), Path);

                return Directory.Exists(fullPath);
            }
            catch
            {
                return false;
            }
        }
    }
}