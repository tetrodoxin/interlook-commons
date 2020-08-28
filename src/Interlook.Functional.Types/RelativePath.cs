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
using System.Runtime.CompilerServices;
using io = System.IO;

namespace Interlook.Functional.Types
{
    /// <summary>
    /// A non-empty reference of a relative path, thus having no rooted directory.
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    public abstract class RelativePath : NonEmptyPath
    {
        internal RelativePath()
        { }

        internal static Either<Exception, NonEmptyPathString> CheckRelativeConstraints(NonEmptyPathString validPathString)
            => validPathString.ToExceptionEither()
                .FailIf(path => io.Path.IsPathRooted(path.Path), path => new ArgumentException($"Path '{path}' has a root directory and thus no relative path.", nameof(validPathString)));
    }
}