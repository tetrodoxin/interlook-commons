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
using io = System.IO;

namespace Interlook.Functional.Types
{
    /// <summary>
    /// A type representing a valid file name,
    /// thus being non-empty and not containing
    /// characters, that are not valid for file names (see <see cref="io.Path.GetInvalidFileNameChars"/>)
    /// </summary>
    public class FileName
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        public NonEmptyString Name { get; }

        internal FileName(NonEmptyString name) => Name = name;

        /// <summary>
        /// Tries to create an instance of <see cref="FileName"/>
        /// </summary>
        /// <param name="name">The name of the file. Must not be empty and not conatin any characters returned by <see cref="io.Path.GetInvalidFileNameChars"/></param>
        /// <returns>An instance of <see cref="Right{Exception, FileName}"/> containing
        /// a new instance of <see cref="FileName"/> if the path provided was a valid file name;
        /// otherwise <see cref="Left{Exception, FileName}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, FileName> Create(string name)
            => name.ToExceptionEither()
                .FailIf(_ => name == null, new ArgumentNullException(nameof(name)))
                .Select(_ => AnyString.Create(name))
                .Bind(s => s switch
                    {
                        SomeString some => Create(some),
                        WhitespaceString white => Create(white),
                        _ => Either.Left<Exception, FileName>(new ArgumentException("Filename must not be empty!"))
                    });

        /// <summary>
        /// Tries to create an instance of <see cref="FileName"/>
        /// </summary>
        /// <param name="name">The name of the file. Must not be empty and not conatin any characters returned by <see cref="io.Path.GetInvalidFileNameChars"/></param>
        /// <returns>An instance of <see cref="Right{Exception, FileName}"/> containing
        /// a new instance of <see cref="FileName"/> if the path provided was a valid file name;
        /// otherwise <see cref="Left{Exception, FileName}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, FileName> Create(SomeString name)
            => name.ToExceptionEither()
                .FailIf(_ => name == null, new ArgumentNullException(nameof(name)))
                .Bind(fileName => fileName.Value.IndexOfAny(io.Path.GetInvalidFileNameChars()).ToExceptionEither()
                    .FailIf(illegalPos => illegalPos >= 0, illegalPos => new FormatException($"Filename contains illegal character at position {illegalPos}"))
                    .Select(_ => new FileName(fileName)));

        /// <summary>
        /// Tries to create an instance of <see cref="FileName"/> from a name of whitespace characters.
        /// </summary>
        /// <param name="name">A whitespace string></param>
        /// <returns>An instance of <see cref="Right{Exception, FileName}"/> containing
        /// a new instance of <see cref="FileName"/> if the path provided was a valid file name;
        /// otherwise <see cref="Left{Exception, FileName}"/> containing the respective error as <see cref="Exception"/>.</returns>
        internal static Either<Exception, FileName> Create(WhitespaceString name)
            => name.ToExceptionEither()
                .Bind(fileName => fileName.Value.IndexOfAny(io.Path.GetInvalidFileNameChars()).ToExceptionEither()
                    .FailIf(illegalPos => illegalPos >= 0, illegalPos => new FormatException($"Filename contains illegal character at position {illegalPos}"))
                    .Select(_ => new FileName(fileName)));
    }
}