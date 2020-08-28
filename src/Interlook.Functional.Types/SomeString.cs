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
using System.Globalization;

namespace Interlook.Functional.Types
{
    /// <summary>
    /// A string object containing actual characters (at least one),
    /// rather than whitespace-characters only.
    /// </summary>
    public sealed class SomeString : NonEmptyString
    {
        internal SomeString(string value) : base(value)
        {
        }

        /// <summary>
        /// Concatenates several <see cref="SomeString"/> instances.
        /// </summary>
        /// <param name="values">The string values to concatenate</param>
        public static Either<Exception, SomeString> ConcatAll(params SomeString[] values)
        {
            string[] final = new string[values.Length];

            for (int i = 0; i <= values.Length; i++)
                final[i] = values[i]?.Value ?? string.Empty;

            return Create(string.Concat(final));
        }

        /// <summary>
        /// Tries to create an <see cref="SomeString"/> instance.
        /// </summary>
        /// <param name="value">The non-empty string, containing non-whitespace characters.</param>
        /// <returns>A <see cref="Right{Exception, SomeString}"/> instance containing the created <see cref="SomeString"/> object;
        /// otherwise <see cref="Left{Exception, SomeString}"/> containing the occured error.</returns>
        public static new Either<Exception, SomeString> Create(string value)
            => CanCreateSomeStringFrom(value) is Just<ArgumentException> just
                ? Either.Left<Exception, SomeString>(just.Value)
                : new SomeString(value).ToExceptionEither();

        /// <summary>
        /// Concatenates a a <see cref="char"/> object to this <see cref="SomeString"/> instance
        /// </summary>
        /// <param name="c">The character to append.</param>
        /// <returns></returns>
        public new SomeString Append(char c) => new SomeString(Value + c);

        /// <summary>
        /// Concatenates a <see cref="string"/> object to this <see cref="SomeString"/> instance.
        /// </summary>
        /// <param name="s2">The string to append.</param>
        /// <returns></returns>
        public SomeString Concat(string s2)
            => new SomeString(string.Concat(Value, s2 ?? string.Empty));

        /// <summary>
        /// Concatenates two <see cref="string"/> objects to this <see cref="SomeString"/> instance.
        /// </summary>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        public SomeString Concat(string s2, string s3)
            => new SomeString(string.Concat(Value, s2 ?? string.Empty, s3 ?? string.Empty));

        /// <summary>
        /// Concatenates three <see cref="string"/> objects to this <see cref="SomeString"/> instance.
        /// </summary>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        /// <param name="s4">The third string to append.</param>
        public SomeString Concat(string s2, string s3, string s4)
            => new SomeString(string.Concat(Value, s2 ?? string.Empty, s3 ?? string.Empty, s4 ?? string.Empty));

        /// <summary>
        /// Concatenates several <see cref="string"/> objects to this a <see cref="SomeString"/> instance.
        /// </summary>
        /// <param name="values">The string values to append</param>
        public SomeString Concat(params string[] values)
        {
            string[] final = new string[values.Length + 1];

            final[0] = Value;
            Buffer.BlockCopy(values, 0, final, 1, values.Length);
            return new SomeString(string.Concat(final));
        }

        /// <summary>
        /// Concatenates a <see cref="AnyString"/> instance and this <see cref="SomeString"/> instance.
        /// to a new <see cref="SomeString"/> instance.
        /// </summary>
        /// <param name="s2">The string to prepend.</param>
        public SomeString Prepend(AnyString s2) => new SomeString(string.Concat(s2 ?? string.Empty, Value));

        /// <summary>
        /// Returns a copy of this <see cref="SomeString"/> instance converted to lowercase.
        /// </summary>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>A new instance of <see cref="SomeString"/></returns>
        public SomeString ToLower(CultureInfo culture)
            => new SomeString(culture.TextInfo.ToLower(Value));

        /// <summary>
        /// Returns a copy of this <see cref="SomeString"/> instance converted to lowercase.
        /// </summary>
        /// <returns>A new instance of <see cref="SomeString"/></returns>
        public SomeString ToLower() => ToLower(CultureInfo.CurrentCulture);

        /// <summary>
        /// Returns a copy of this <see cref="SomeString"/> instance converted to uppercase.
        /// </summary>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>A new instance of <see cref="SomeString"/></returns>
        public SomeString ToUpper(CultureInfo culture)
            => new SomeString((culture ?? CultureInfo.CurrentCulture).TextInfo.ToUpper(Value));

        /// <summary>
        /// Returns a copy of this <see cref="SomeString"/> instance converted to uppercase.
        /// </summary>
        /// <returns>A new instance of <see cref="SomeString"/></returns>
        public SomeString ToUpper()
            => ToUpper(CultureInfo.CurrentCulture);
    }
}