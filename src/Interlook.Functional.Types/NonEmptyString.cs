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
using Interlook.Text;
using System;
using System.Linq;

namespace Interlook.Functional.Types
{
    /// <summary>
    /// An abstract base clase for types of non empty strings.
    /// </summary>
    public abstract class NonEmptyString : AnyString
    {
        /// <summary>
        /// Returns the very first character of the string.
        /// </summary>
        public char FirstChar { get; }

        /// <summary>
        /// Returns the very last character of the string.
        /// </summary>
        public char LastChar { get; }

        /// <summary>
        /// Returns the character at the specified position.
        /// </summary>
        /// <param name="charPosition">The position in the string.
        /// A negative value is subtracted from the strings length, thus indexing backwards,
        /// whereas -1 references the final character.</param>
        /// <returns>A <see cref="Just{Char}"/> instance containing the character at the requested position,
        /// if such is present; otherwise <see cref="Nothing{Char}"/></returns>
        public Maybe<char> this[int charPosition] => GetCharAtPosition(charPosition);

        /// <summary>
        /// Tries to Create a functional string object from an instance of <see cref="string"/>,
        /// that is neither <c>null</c> nor empty.
        /// </summary>
        /// <param name="s">The non-empty string instance.</param>
        /// <returns>An instance of <see cref="WhitespaceString"/> or
        /// <see cref="SomeString"/> according to the content of <paramref name="s"/></returns>
        public static new Either<Exception, NonEmptyString> Create(string s)
            => s.IsNullOrEmpty()
                ? Either.Left<Exception, NonEmptyString>(new ArgumentException("String must neither be <null> nor empty."))
                : CanCreateWhiteSpaceStringFrom(s)
                    .Select(_ => ((NonEmptyString)new SomeString(s)).ToExceptionEither())
                    .GetValue(((NonEmptyString)new WhitespaceString(s)).ToExceptionEither());

        internal NonEmptyString(string value) : base(value)
        {
            FirstChar = value[0];
            LastChar = value[Length - 1];
        }

        /// <summary>
        /// Tries to split a string at the last occurance of a given character.
        /// </summary>
        /// <param name="separator">The character, marking the position to split at.</param>
        /// <returns>If the character in <paramref name="separator"/> was found,
        /// a tuple of <see cref="AnyString"/> is returned, containing the left and the right remainder respectively;
        /// otherwise the original <see cref="NonEmptyString"/> is returned.</returns>
        public Either<NonEmptyString, (AnyString Left, AnyString Right)> SplitAtLastOccurenceOf(char separator)
        {
            int pos = LastIndexOf(separator);   // is always <Length
            return splitAtPos(pos);
        }

        /// <summary>
        /// Tries to split a string at the last occurance of a given character.
        /// </summary>
        /// <param name="separators">An array of characters, each of which can possibly mark the position to split at.</param>
        /// <returns>If any character of <paramref name="separators"/> was found,
        /// a tuple of <see cref="AnyString"/> is returned, containing the left and the right remainder respectively;
        /// otherwise the original <see cref="NonEmptyString"/> is returned.</returns>
        public Either<NonEmptyString, (AnyString Left, AnyString Right)> SplitAtLastOccurenceOfAny(char[] separators)
        {
            int pos = LastIndexOfAny(separators);   // is always <Length
            return splitAtPos(pos);
        }

        /// <summary>
        /// Tries to split a string at the first occurance of a given character.
        /// </summary>
        /// <param name="separator">The character, marking the position to split at.</param>
        /// <returns>If the character in <paramref name="separator"/> was found,
        /// a tuple of <see cref="AnyString"/> is returned, containing the left and the right remainder respectively;
        /// otherwise the original <see cref="NonEmptyString"/> is returned.</returns>
        public Either<NonEmptyString, (AnyString Left, AnyString Right)> SplitAtFirstOccurenceOf(char separator)
        {
            int pos = LastIndexOf(separator);   // is always <Length
            return splitAtPos(pos);
        }

        /// <summary>
        /// Tries to split a string at the first occurance of a given character.
        /// </summary>
        /// <param name="separators">An array of characters, each of which can possibly mark the position to split at.</param>
        /// <returns>If any character of <paramref name="separators"/> was found,
        /// a tuple of <see cref="AnyString"/> is returned, containing the left and the right remainder respectively;
        /// otherwise the original <see cref="NonEmptyString"/> is returned.</returns>
        public Either<NonEmptyString, (AnyString Left, AnyString Right)> SplitAtFirstOccurenceOfAny(char[] separators)
        {
            int pos = LastIndexOfAny(separators);   // is always <Length
            return splitAtPos(pos);
        }

        /// <summary>
        /// Returns an array containing the substrings in this instance
        /// that are delimited by specified characters.
        /// </summary>
        /// <param name="separators">An array of characters that serve as separators</param>
        /// <returns>An array of <see cref="NonEmptyString"/> instances,
        /// that were separated by the delimiters in the original.
        /// Empty parts between delimiters are ignored.</returns>
        public NonEmptyString[] Split(char[] separators)
        {
            try
            {
                var parts = Value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                return parts.Select(p => AnyString.Create(p)).OfType<NonEmptyString>().ToArray();
            }
            catch
            {
                return new NonEmptyString[] { this }; // let's keep this simple
            }
        }

        /// <summary>
        /// Concatenates a <see cref="char"/> object to this <see cref="WhitespaceString"/> instance.
        /// </summary>
        /// <param name="c">The character to append.</param>
        /// <returns></returns>
        public NonEmptyString Append(char c)
            => this switch
            {
                SomeString some => some.Append(c),
                WhitespaceString white => white.Append(c),
                _ => throw new NotImplementedException($"Invalid type or <null>.")
            };

        /// <summary>
        /// Returns an array containing the substrings in this instance
        /// that are delimited by a specified character.
        /// </summary>
        /// <param name="separator">The character that serves as separator.</param>
        /// <returns>An array of <see cref="NonEmptyString"/> instances,
        /// that were separated by the delimiters in the original.
        /// Empty parts between delimiters are ignored.</returns>
        public NonEmptyString[] Split(char separator)
        {
            try
            {
                var parts = Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                return parts.Select(p => AnyString.Create(p)).OfType<NonEmptyString>().ToArray();
            }
            catch
            {
                return new NonEmptyString[] { this }; // let's keep this simple
            }
        }

        private Either<NonEmptyString, (AnyString Left, AnyString Right)> splitAtPos(int pos)
        {
            if (pos < 0)
            {
                return Either.Left<NonEmptyString, (AnyString Left, AnyString Right)>(this);
            }
            else if (pos == 0)
            {
                return Length > 1
                    ? Either.Right<NonEmptyString, (AnyString Left, AnyString Right)>((EmptyString.Default, this))
                    : Either.Right<NonEmptyString, (AnyString Left, AnyString Right)>((EmptyString.Default, EmptyString.Default));
            }
            else if (pos == Length - 1)
            {
                return Either.Right<NonEmptyString, (AnyString Left, AnyString Right)>((this, EmptyString.Default));
            }
            else
            {
                var left = AnyString.Create(Value.Substring(0, pos));
                var right = AnyString.Create(Value.Substring(pos + 1));  // secure since pos<Length-1 always here
                return Either.Right<NonEmptyString, (AnyString Left, AnyString Right)>((left, right));
            }
        }

        internal static NonEmptyString CreateDefiniteNonEmptyString(string value)
        {
            return CanCreateWhiteSpaceStringFrom(value).HasValue()
                ? (NonEmptyString)new WhitespaceString(value)
                : new SomeString(value);
        }
    }

    /// <summary>
    /// Extension methods for <see cref="NonEmptyString"/> objects
    /// </summary>
    public static class NonEmptyStringExtensions
    {
        /// <summary>
        /// Concatenates a <see cref="string"/> object to a <see cref="NonEmptyString"/> instance.
        /// </summary>
        /// <param name="s">The original non empty string</param>
        /// <param name="s2">The first string to append.</param>
        public static NonEmptyString Concat(this NonEmptyString s, string s2)
            => NonEmptyString.CreateDefiniteNonEmptyString(string.Concat(s.Value, s2 ?? string.Empty));

        /// <summary>
        /// Concatenates two <see cref="string"/> objects to a <see cref="NonEmptyString"/> instance.
        /// </summary>
        /// <param name="s">The original non empty string</param>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        public static NonEmptyString Concat(this NonEmptyString s, string s2, string s3)
            => NonEmptyString.CreateDefiniteNonEmptyString(string.Concat(s.Value, s2 ?? string.Empty, s3 ?? string.Empty));

        /// <summary>
        /// Concatenates three <see cref="string"/> objects to a <see cref="NonEmptyString"/> instance.
        /// </summary>
        /// <param name="s">The original non empty string</param>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        /// <param name="s4">The third string to append.</param>
        public static NonEmptyString Concat(this NonEmptyString s, string s2, string s3, string s4)
            => NonEmptyString.CreateDefiniteNonEmptyString(string.Concat(s.Value, s2 ?? string.Empty, s3 ?? string.Empty, s4 ?? string.Empty));

        /// <summary>
        /// Concatenates several <see cref="string"/> objects to a <see cref="NonEmptyString"/> instance.
        /// </summary>
        /// <param name="s">The original non empty string</param>
        /// <param name="values">The string values to append</param>
        public static NonEmptyString Concat(this NonEmptyString s, params string[] values)
        {
            string[] final = new string[values.Length + 1];

            final[0] = s.Value;
            Buffer.BlockCopy(values, 0, final, 1, values.Length);
            string value = string.Concat(final);
            return NonEmptyString.CreateDefiniteNonEmptyString(value);
        }
    }
}