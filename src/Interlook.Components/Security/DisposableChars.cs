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
using System;
using System.Linq;
using System.Text;

namespace Interlook.Security
{
    /// <summary>
    /// Encapsulates a byte array into an <see cref="IDisposable"/> object.
    /// The content will be destroyed when the object is disposed.
    /// <para>
    /// This class can be casted to <see cref="char"/>[].
    /// </para>
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public sealed class DisposableChars : DisposableArray<char>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableChars"/> class
        /// with an existing char array, which is NOT copied, but just referenced,
        /// thus external changes will affect this instance too.
        /// </summary>
        /// <param name="content">The content to reference.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="content"/> was <c>null</c>.</exception>
        public DisposableChars(char[] content) : base(content)
        { }


        /// <summary>
        /// Gets the bytes encoding by the contained characters
        /// inside an <see cref="DisposableBytes"/> object,
        /// that will be disposed, too when this instance is disposed.
        /// </summary>
        /// <param name="e">The character encoding to be used.</param>
        /// <returns>A <see cref="DisposableBytes"/> object.</returns>
        public DisposableBytes GetBytes(Encoding e)
        {
            var chars = e.GetBytes(OriginalArray);
            var result = new DisposableBytes(chars);
            AddAdditionalDisposable(result);
            return result;
        }

    }
}