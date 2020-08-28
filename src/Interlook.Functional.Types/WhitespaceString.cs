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
    /// A functional string object that only consists of whitespaces,
    /// i.e. Unicode separators.
    /// </summary>
    public sealed class WhitespaceString : NonEmptyString
    {
        /// <summary>
        /// Tries to create an <see cref="WhitespaceString"/> instance.
        /// </summary>
        /// <param name="value">The non-empty string, only containing whitespace characters.</param>
        /// <returns>A <see cref="Right{Exception, WhitespaceString}"/> instance containing the created <see cref="SomeString"/> object;
        /// otherwise <see cref="Left{Exception, WhitespaceString}"/> containing the occured error.</returns>
        public new Either<Exception, WhitespaceString> Create(string value)
            => CanCreateWhiteSpaceStringFrom(value) is Just<ArgumentException> just
                ? Either.Left<Exception, WhitespaceString>(just.Value)
                : new WhitespaceString(value).ToExceptionEither();

        internal WhitespaceString(string value) : base(value)
        {
        }

        /// <summary>
        /// Concatenates a <see cref="char"/> object to this <see cref="WhitespaceString"/> instance.
        /// </summary>
        /// <param name="c">The character to append.</param>
        /// <returns></returns>
        public new NonEmptyString Append(char c) => CreateDefiniteNonEmptyString(Value + c);

        /// <summary>
        /// Concatenates a <see cref="string"/> object to this <see cref="WhitespaceString"/> instance.
        /// </summary>
        /// <param name="s2">The string to append.</param>
        /// <returns></returns>
        public NonEmptyString Concat(string s2) => CreateDefiniteNonEmptyString(string.Concat(Value, s2 ?? string.Empty));

        /// <summary>
        /// Concatenates two <see cref="string"/> objects to this <see cref="WhitespaceString"/> instance.
        /// </summary>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        public NonEmptyString Concat(string s2, string s3)
            => CreateDefiniteNonEmptyString(string.Concat(Value, s2 ?? string.Empty, s3 ?? string.Empty));

        /// <summary>
        /// Concatenates three <see cref="string"/> objects to this <see cref="WhitespaceString"/> instance.
        /// </summary>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        /// <param name="s4">The third string to append.</param>
        public NonEmptyString Concat(string s2, string s3, string s4)
            => CreateDefiniteNonEmptyString(string.Concat(Value, s2 ?? string.Empty, s3 ?? string.Empty, s4 ?? string.Empty));

        /// <summary>
        /// Concatenates several <see cref="string"/> objects to this <see cref="WhitespaceString"/> instance.
        /// </summary>
        /// <param name="values">The string values to append</param>
        public NonEmptyString Concat(params string[] values)
        {
            string[] final = new string[values.Length + 1];

            final[0] = Value;
            Buffer.BlockCopy(values, 0, final, 1, values.Length);
            string value = string.Concat(final);
            return CreateDefiniteNonEmptyString(value);
        }

        /// <summary>
        /// Concatenates a <see cref="WhitespaceString"/> object to this <see cref="WhitespaceString"/> instance.
        /// </summary>
        /// <param name="s2">The first string to append.</param>
        public WhitespaceString Concat(WhitespaceString s2)
            => new WhitespaceString(string.Concat(Value, s2 ?? string.Empty));

        /// <summary>
        /// Concatenates two <see cref="WhitespaceString"/> objects to this <see cref="WhitespaceString"/> instance.
        /// </summary>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        public WhitespaceString Concat(WhitespaceString s2, WhitespaceString s3)
            => new WhitespaceString(string.Concat(Value, s2 ?? string.Empty, s3 ?? string.Empty));

        /// <summary>
        /// Concatenates three <see cref="WhitespaceString"/> objects to this <see cref="WhitespaceString"/> instance.
        /// </summary>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        /// <param name="s4">The third string to append.</param>
        public WhitespaceString Concat(WhitespaceString s2, WhitespaceString s3, WhitespaceString s4)
            => new WhitespaceString(string.Concat(Value, s2 ?? string.Empty, s3 ?? string.Empty, s4 ?? string.Empty));

        /// <summary>
        /// Concatenates several <see cref="WhitespaceString"/> objects to this <see cref="WhitespaceString"/> instance.
        /// </summary>
        /// <param name="values">The string values to concatenate</param>
        public WhitespaceString Concat(params WhitespaceString[] values)
        {
            string[] final = new string[values.Length + 1];

            final[0] = Value;

            for (int i = 0; i <= values.Length; i++)
                final[i+1] = values[i]?.Value ?? string.Empty;

            return new WhitespaceString(string.Concat(final));
        }

        /// <summary>
        /// Concatenates several <see cref="WhitespaceString"/> objects.
        /// </summary>
        /// <param name="values">The string values to concatenate</param>
        public static WhitespaceString ConcatAll(params WhitespaceString[] values)
        {
            string[] final = new string[values.Length];

            for (int i = 0; i <= values.Length; i++)
                final[i] = values[i]?.Value ?? string.Empty;

            return new WhitespaceString(string.Concat(final));
        }

        /// <summary>
        /// Returns a copy of a <see cref="WhitespaceString"/> object converted to lowercase.
        /// </summary>
        /// <param name="s">The string object to convert.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>A new instance of <see cref="WhitespaceString"/></returns>
        public WhitespaceString ToLower(WhitespaceString s, CultureInfo culture)
            => new WhitespaceString(culture.TextInfo.ToLower(s.Value));

        /// <summary>
        /// Returns a copy of a <see cref="WhitespaceString"/> object converted to lowercase.
        /// </summary>
        /// <param name="s">The string object to convert.</param>
        /// <returns>A new instance of <see cref="WhitespaceString"/></returns>
        public WhitespaceString ToLower(WhitespaceString s)
            => ToLower(s, CultureInfo.CurrentCulture);

        /// <summary>
        /// Returns a copy of this <see cref="WhitespaceString"/> instance converted to uppercase.
        /// </summary>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>A new instance of <see cref="WhitespaceString"/></returns>
        public WhitespaceString ToUpper(CultureInfo culture)
            => new WhitespaceString(culture.TextInfo.ToUpper(Value));

        /// <summary>
        /// Returns a copy of this <see cref="WhitespaceString"/> instance converted to uppercase.
        /// </summary>
        /// <returns>A new instance of <see cref="WhitespaceString"/></returns>
        public WhitespaceString ToUpper()
            => ToUpper(CultureInfo.CurrentCulture);
    }
}