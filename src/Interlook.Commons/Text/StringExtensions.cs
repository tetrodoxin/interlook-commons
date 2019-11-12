#region license

//MIT License

//Copyright(c) 2013-2019 Andreas Hübner

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

namespace Interlook.Text
{
    /// <summary>
    /// A helper class providing methods for checking and manipulating string objects.
    /// </summary>
    public static class StringExtensions
    {
        private static readonly char[] maximumStringArray = new char[0xa000];

        private static Dictionary<string, string> latinNormCharsArray = new Dictionary<string, string>()
        {
            { "Ä", "AE" },
            { "Á", "A" },
            { "À", "A" },
            { "Â", "A" },
            { "Ã", "A" },
            { "Å", "A" },
            { "Æ", "AE" },
            { "Ç", "C" },
            { "È", "E" },
            { "É", "E" },
            { "Ê", "E" },
            { "Ë", "E" },
            { "Ì", "I" },
            { "Í", "I" },
            { "Î", "I" },
            { "Ï", "I" },
            { "Ñ", "N" },
            { "Ò", "O" },
            { "Ó", "O" },
            { "Ô", "O" },
            { "Õ", "O" },
            { "Ö", "OE" },
            { "Ø", "OE" },
            { "Ù", "U" },
            { "Ú", "U" },
            { "Û", "U" },
            { "Ü", "UE" },
            { "Ý", "Y" },
            { "Ÿ", "Y" },
            { "Š", "S" },
            { "Œ", "OE" },
            { "Ž", "Z" },
            { "ß", "SS" }
        };

        /// <summary>
        /// Indicates whether the string is neither <c>null</c> nor an empty string.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns><c>true</c> if the given string actually contains characters. Otherwise, if it's empty or even <c>null</c>, <c>false</c> is returned.</returns>
        public static bool AintNullNorEmpty(this string str)
        {
            return !(String.IsNullOrEmpty(str));
        }

        /// <summary>
        /// Returns a string, that always begins with a capital letter.
        /// </summary>
        /// <param name="str">String, where first letter must be capital.</param>
        /// <param name="forceLowercaseRemainder">If <c>true</c>, all characters following the first are guaranteed to be lower case, otherwise left untouched.</param>
        /// <returns>A string with a guaranteed upper case first character.</returns>
        public static string CapitalizedFirstCharacter(this string str, bool forceLowercaseRemainder = false)
        {
            string result = String.Empty;
            if (!String.IsNullOrEmpty(str))
            {
                result = str.Substring(0, 1).ToUpper();
                if (str.Length > 1)
                {
                    if (forceLowercaseRemainder)
                    {
                        result += str.Substring(1).ToLower();
                    }
                    else
                    {
                        result += str.Substring(1);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Ensures, that the corresponding string object is no <c>null</c>-reference.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns>The given string itself, if it was not <c>null</c>, otherwise an empty string is returned.</returns>
        public static string Ensure(this string str) => str ?? string.Empty;

        /// <summary>
        /// Checks if a string is <c>null</c>, empty or just
        /// contains whitespace characters.
        /// </summary>
        /// <param name="str">String object to check.</param>
        public static bool IsNothing(this string str) => string.IsNullOrWhiteSpace(str);

        /// <summary>
        /// Indicates whether the string is <c>null</c> or an empty string.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns><c>true</c> if the given string is <c>null</c> or an empty string. Otherwise <c>false</c>.</returns>
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        /// <summary>
        /// Indicates whether the string only contains decimal numbers.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns><c>true</c> if the given string only contains decimal numbers.</returns>
        public static bool IsNumericOnly(this string str) => IsNumericOnly(str, new char[0]);

        /// <summary>
        /// Indicates whether the string only contains decimal numbers or special additional characters.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <param name="acceptedAdditionalChars">A string, containing additional accepted characters.</param>
        /// <returns><c>true</c> if the given string only contains decimal numbers or the provided additional characters.</returns>
        public static bool IsNumericOnly(this string str, string acceptedAdditionalChars)
        {
            if (String.IsNullOrEmpty(acceptedAdditionalChars))
            {
                return IsNumericOnly(str, new char[0]);
            }

            return IsNumericOnly(str, acceptedAdditionalChars.ToCharArray());
        }

        /// <summary>
        /// Indicates whether the string only contains decimal numbers or special additional characters.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <param name="acceptedAdditionalChars">An array of additional accepted characters.</param>
        /// <returns><c>true</c> if the given string only contains decimal numbers or the provided additional characters.</returns>
        public static bool IsNumericOnly(this string str, char[] acceptedAdditionalChars)
        {
            if (acceptedAdditionalChars == null)
            {
                acceptedAdditionalChars = new char[0];
            }

            for (int i = 0; i < str.Length; i++)
            {
                if (acceptedAdditionalChars.Contains(str[i])) continue;
                if ((str[i] < '0') || (str[i] > '9')) return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the first <c>n</c> characters of a string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="length">The maximum number of characters to return.</param>
        /// <returns>A <see cref="string"/>, containing at most the requested number oft characters
        /// from the beginning of the given string.</returns>
        public static string Left(this string str, int length) => Left(str, length, false);

        /// <summary>
        /// Returns the first <c>n</c> characters of a string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="length">The maximum number of characters to return.</param>
        /// <param name="strict">if set to <c>true</c> an exception is thrown if <c>length</c>
        /// exceeds the length of the given string. Otherwise behavior is equal to <see cref="Left(string, int)"/></param>
        /// <returns>
        /// A <see cref="string" />, containing at most the requested number oft characters
        /// from the beginning of the given string.
        /// </returns>
        public static string Left(this string str, int length, bool strict)
        {
            if (length <= 0)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            else if (str.Length < length)
            {
                if (strict)
                {
                    throw new ArgumentOutOfRangeException(nameof(length), "String must no be shorter than given length.");
                }

                return str;
            }
            else
            {
                return str.Substring(0, length);
            }
        }

        /// <summary>
        /// Limits a string to a specific length.
        /// </summary>
        /// <param name="str">String to limit in length.</param>
        /// <param name="maxLength">Maximum length of the string.</param>
        /// <returns>
        /// A <see cref="string"/>, whose length never exceeds given <c>maxLength</c>.
        /// </returns>
        public static string Limit(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
            {
                return str;
            }
            else
            {
                return str.Substring(0, maxLength);
            }
        }

        /// <summary>
        /// Checks if a string matches a regular expression.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <param name="pattern">The regular expression pattern for checking.</param>
        public static bool MatchesRegEx(this string value, string pattern)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            return System.Text.RegularExpressions.Regex.IsMatch(value ?? string.Empty, pattern);
        }

        /// <summary>
        /// Returns a normalized copy of a string, meaning only capitals without any accent signs (e.g. accent circumflex).
        /// </summary>
        /// <param name="str">The string to normalize.</param>
        /// <returns>A capitalized string that represents the provided string in a way to be comparable case- and accent-insensitive.</returns>
        public static string NormalizeLatinChars(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }
            else
            {
                str = str.ToUpper();
                foreach (var pair in latinNormCharsArray)
                {
                    str = str.Replace(pair.Key, pair.Value);
                }

                return str;
            }
        }

        /// <summary>
        /// Returns an alternative string, if this instance is <c>null</c> or empty.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <param name="alternativeString">Alternative string, if the original was empty or <c>null</c>.</param>
        /// <returns>The original string or, if it was <c>null</c> or empty, the alternative string.</returns>
        public static string Otherwise(this string str, string alternativeString)
        {
            if (string.IsNullOrEmpty(str))
            {
                return alternativeString;
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// Returns an alternative string, if this instance satisfies a given condition.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <param name="predicate">The condition, that has to be true to use <paramref name="alternativeString"/>.</param>
        /// <param name="alternativeString">Alternative string, if the original satisfied the condition.</param>
        /// <returns>
        /// The original string or, if it satisfied the condition, the alternative string.
        /// </returns>
        public static string Otherwise(this string str, Func<string, bool> predicate, string alternativeString)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            if (predicate(str))
            {
                return alternativeString;
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// Returns an alternative string, if this instance satisfies a given condition.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <param name="predicate">The condition, that has to be true to use <paramref name="alternativeStringFactory"/>.</param>
        /// <param name="alternativeStringFactory">A function, getting the current string to create the alternative string.</param>
        /// <returns>
        /// The original string or, if it satisfied the condition, the alternative string.
        /// </returns>
        public static string Otherwise(this string str, Func<string, bool> predicate, Func<string, string> alternativeStringFactory)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (alternativeStringFactory == null) throw new ArgumentNullException(nameof(alternativeStringFactory));

            return predicate(str) ? alternativeStringFactory(str) : str;
        }

        /// <summary>
        /// Parses a string into  <see cref="double"/>
        /// or returns <c>0.0</c>, if an error occured.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>Converted  <see cref="double"/> value or <c>0</c>.</returns>
        public static double ParseDoubleOrDefault(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return 0.0;
            }
            else
            {
                return double.TryParse(str.Trim(), out double result) ? result : 0.0;
            }
        }

        /// <summary>
        /// Parses a string into <see cref="int"/>
        /// or returns <c>0.0</c>, if an error occured.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>Converted  <see cref="int"/> value or <c>0</c>.</returns>
        public static int ParseIntOrDefault(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return 0;
            }
            else
            {
                return int.TryParse(str.Trim(), out int result) ? result : 0;
            }
        }

        /// <summary>
        /// Parses a string into <see cref="long"/>
        /// or returns <c>0.0</c>, if an error occured.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>Converted  <see cref="long"/> value or <c>0</c>.</returns>
        public static long ParseLongOrDefault(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return 0;
            }
            else
            {
                return long.TryParse(str.Trim(), out long result) ? result : 0;
            }
        }

        /// <summary>
        /// Parses a string into <see cref="short"/>
        /// or returns <c>0.0</c>, if an error occured.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>Converted  <see cref="short"/> value or <c>0</c>.</returns>
        public static short ParseShortOrDefault(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return 0;
            }
            else
            {
                return short.TryParse(str.Trim(), out short result) ? result : (short)0;
            }
        }

        /// <summary>
        /// Parses a string into <see cref="TimeSpan"/>
        /// or returns <c>0</c> seconds, if an error occured.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>Converted <see cref="TimeSpan"/> value or <c>0</c> seconds.</returns>
        public static TimeSpan ParseTimeSpanOrDefault(this string str)
        {
            if (string.IsNullOrEmpty(str.TrimProtected()))
            {
                return TimeSpan.FromSeconds(0);
            }
            else
            {
                return TimeSpan.TryParse(str, out TimeSpan result) ? result : TimeSpan.FromSeconds(0);
            }
        }

        /// <summary>
        /// Returns the last <c>n</c> characters of a string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="length">The maximum number of characters to return.</param>
        /// <returns>A <see cref="string"/>, containing at most the requested number oft characters
        /// from the end of the given string.</returns>
        public static string Right(this string str, int length) => Right(str, length, false);

        /// <summary>
        /// Returns the last <c>n</c> characters of a string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="length">The maximum number of characters to return.</param>
        /// <param name="strict">if set to <c>true</c> an exception is thrown if <c>length</c>
        /// exceeds the length of the given string. Otherwise behavior is equal to <see cref="Right(string, int)"/></param>
        /// <returns>
        /// A <see cref="string" />, containing at most the requested number oft characters
        /// from the end of the given string.
        /// </returns>
        public static string Right(this string str, int length, bool strict)
        {
            if (length <= 0)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            else if (str.Length < length)
            {
                if (strict)
                {
                    throw new ArgumentOutOfRangeException(nameof(length), "String must not be shorter than given length.");
                }

                return str;
            }
            else
            {
                return str.Substring(str.Length - length);
            }
        }

        /// <summary>
        /// Compares a string to another, by trying to achieve an almost constant time,
        /// to prevent side channel attacks (timing attacks)
        /// </summary>
        /// <param name="original">The original string</param>
        /// <param name="candidate">The string to compare to</param>
        /// <returns><c>true</c> if the two strings match exactly (so even case sensitive), otherwise <c>false</c></returns>
        public static bool SecureEquals(this string original, string candidate) => SecureEquals(original.ToCharArray(), candidate.ToCharArray());

        /// <summary>
        /// Compares an array of characters to another, by trying to achieve an almost constant time,
        /// to mitigate side channel attacks (timing attacks)
        /// </summary>
        /// <param name="original">The original characters.</param>
        /// <param name="candidate">The char array to compare.</param>
        /// <returns><c>true</c> if the two arrays match exactly (so even case sensitive), otherwise <c>false</c></returns>
        public static bool SecureEquals(this char[] original, char[] candidate)
        {
            int result = 0;
            int origPos = 0;
            int candPos = 0;
            int dummy = 0;

            int origLastPos = original.Length - 1;
            int candLastPos = candidate.Length - 1;

            if (original == null || candidate == null) return false;

            while (true)
            {
                result |= original[origPos] ^ candidate[candPos];

                if (origPos == origLastPos) break;

                origPos++;

                if (candPos != candLastPos) candPos++;
                if (candPos == candLastPos) dummy++;
            }

            return result == 0;
        }

        /// <summary>
        /// Skips <c>n</c> characters of the string an returns the remainder.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="length">Number of characters to skip.</param>
        /// <returns>The remainder of the string, which may be empty,
        /// if the original string was shorter than <c>n</c>.</returns>
        public static string Skip(this string str, int length)
        {
            if (string.IsNullOrEmpty(str) || length <= 0)
            {
                return str;
            }

            int startIndex = Math.Min(length, str.Length);
            return str.Substring(startIndex - 1);
        }

        /// <summary>
        /// Removes all leading and trailing white-space characters from a string object, that even may be <c>null</c>.
        /// </summary>
        /// <param name="str">The string object to trim safely (may be null).</param>
        /// <returns>A trimmed copy of the provided string or an empty string, if the parameter string was <c>null</c>.
        /// Is never <c>null</c></returns>
        public static string TrimProtected(this string str) => string.IsNullOrEmpty(str) ? str : str.Trim();
    }
}