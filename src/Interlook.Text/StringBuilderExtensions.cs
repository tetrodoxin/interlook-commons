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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interlook.Text
{
    /// <summary>
    /// Contains extension methods for manipulating <see cref="StringBuilder"/> instances even easier.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends a text, if a certain condition is met.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> instance.</param>
        /// <param name="condition">If <c>true</c>, the provided text will be appended to the StringBuilder.</param>
        /// <param name="textFactory">A function, returning the text to append, if <paramref name="condition"/> is <c>true</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendIf(this StringBuilder sb, bool condition, Func<string> textFactory)
        {
            if (sb == null) sb = new StringBuilder();

            if (condition) sb.Append(textFactory());

            return sb;
        }

        /// <summary>
        /// Appends a text, if a certain condition is met.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> instance.</param>
        /// <param name="condition">If <c>true</c>, the provided text will be appended to the StringBuilder.</param>
        /// <param name="textToAppend">A text to append, if <paramref name="condition"/> is <c>true</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendIf(this StringBuilder sb, bool condition, string textToAppend)
        {
            if (sb == null) sb = new StringBuilder();

            if (condition) sb.Append(textToAppend);

            return sb;
        }

        /// <summary>
        /// Appends only the default line terminator, if a certain condition is met.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> instance.</param>
        /// <param name="condition">If <c>true</c>, the provided text will be appended to the StringBuilder.</param>
        /// <returns></returns>
        public static StringBuilder AppendLineIf(this StringBuilder sb, bool condition)
            => AppendLineIf(sb ?? new StringBuilder(), condition, string.Empty);

        /// <summary>
        /// Appends a text followed by the default line terminator, if a certain condition is met.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> instance.</param>
        /// <param name="condition">If <c>true</c>, the provided text will be appended to the StringBuilder.</param>
        /// <param name="textFactory">A function, returning the text to append, if <paramref name="condition"/> is <c>true</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendLineIf(this StringBuilder sb, bool condition, Func<string> textFactory)
        {
            if (sb == null) sb = new StringBuilder();

            if (condition) sb.AppendLine(textFactory());

            return sb;
        }

        /// <summary>
        /// Appends a text followed by the default line terminator, if a certain condition is met.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> instance.</param>
        /// <param name="condition">If <c>true</c>, the provided text will be appended to the StringBuilder.</param>
        /// <param name="textToAppend">A text to append, if <paramref name="condition"/> is <c>true</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendLineIf(this StringBuilder sb, bool condition, string textToAppend)
        {
            if (sb == null) sb = new StringBuilder();

            if (condition) sb.Append(textToAppend);

            return sb;
        }
    }
}