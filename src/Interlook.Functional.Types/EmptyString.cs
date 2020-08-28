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

namespace Interlook.Functional.Types
{
    /// <summary>
    /// A singleton object for empty strings in functional world.
    /// </summary>
    public sealed class EmptyString : AnyString
    {
        private static readonly Lazy<EmptyString> _instance = new Lazy<EmptyString>(() => new EmptyString());

        /// <summary>
        /// Gets the default and only instance of <see cref="EmptyString"/>.
        /// </summary>
        public static EmptyString Default => _instance.Value;

        private EmptyString() : base(string.Empty)
        {
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public override bool Equals(object? other) => other switch { EmptyString _ => true, string str => Equals(str), _ => false };

        public override bool Equals(string other) => other != null && other.Length == 0;

        public override bool Equals(AnyString other, StringComparison comparisonType) => other is EmptyString;

        public override int GetHashCode() => 0;

        public override string ToString() => string.Empty;

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="value">The string to seek.</param>
        /// <returns>
        /// <c>true</c> if the value in <paramref name="value" /> is empty;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Contains(string value) => string.IsNullOrEmpty(value);

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="value">The string to seek.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>
        /// <c>true</c> if the value in <paramref name="value" /> is empty;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Contains(string value, StringComparison comparisonType) => string.IsNullOrEmpty(value);

        /// <summary>
        /// Determines whether the first character of this string object mathes a specified one.
        /// </summary>
        /// <param name="value">The caracter value to compare with the beginning of this string object.</param>
        /// <returns><c>false</c></returns>
        public override bool StartsWith(char value) => false;

        /// <summary>
        /// Determines whether the last character of this string object mathes a specified one.
        /// </summary>
        /// <param name="value">The caracter value to compare with the end of this string object.</param>
        /// <returns><c>false</c></returns>
        public override bool EndsWith(char value) => false;

        /// <summary>
        /// Determines whether the end of this string object matches a specified string.
        /// </summary>
        /// <param name="value">The string value to compare with the end of this string object.</param>
        /// <returns>
        /// <c>true</c> if the value in <paramref name="value" /> is empty; otherwise, <c>false</c>.
        /// </returns>
        public override bool EndsWith(string value) => string.IsNullOrEmpty(value);

        /// <summary>
        /// Determines whether the beginning of this string object matches a specified string.
        /// </summary>
        /// <param name="value">The string value to compare with the beginning of this string object.</param>
        /// <returns>
        /// <c>true</c> if the value in <paramref name="value" /> is empty; otherwise, <c>false</c>.
        /// </returns>
        public override bool StartsWith(string value) => string.IsNullOrEmpty(value);

        /// <summary>
        /// Determines whether the end of this string object matches a specified string.
        /// </summary>
        /// <param name="value">The string value to compare with the end of this string object.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>
        /// <c>true</c> if the value in <paramref name="value" /> is empty; otherwise, <c>false</c>.
        /// </returns>
        public override bool EndsWith(string value, StringComparison comparisonType) => string.IsNullOrEmpty(value);

        /// <summary>
        /// Determines whether the beginning of this string object matches a specified string.
        /// </summary>
        /// <param name="value">The string value to compare with the beginning of this string object.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>
        /// <c>true</c> if the value in <paramref name="value" /> is empty; otherwise, <c>false</c>.
        /// </returns>
        public override bool StartsWith(string value, StringComparison comparisonType) => string.IsNullOrEmpty(value);

        /// <summary>
        /// Concatenates a <see cref="char"/> object to this <see cref="EmptyString"/> instance.
        /// </summary>
        /// <param name="c">The character to append.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameter necessary for extension method syntax")]
        public NonEmptyString Append(char c)
        {
            string value = c.ToString();
            return CanCreateWhiteSpaceStringFrom(value).IsNothing()
                ? (NonEmptyString)new WhitespaceString(value)
                : new SomeString(value);
        }
    }
}