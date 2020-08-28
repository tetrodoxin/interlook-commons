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

namespace Interlook.Functional.Types
{

    /// <summary>
    /// A base class for string objects in functional world.
    /// </summary>
    public abstract class AnyString
    {
        /// <summary>
        /// The length of the string.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// The actual string value
        /// </summary>
        public string Value { get; }

        internal AnyString(string value)
        {
            Length = value.Length;
            Value = value;
        }

        /// <summary>
        /// Creates a functional string object from an instance of <see cref="string"/>.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <returns>An instance of <see cref="EmptyString"/>, <see cref="WhitespaceString"/> or
        /// <see cref="SomeString"/> according to the content of <paramref name="s"/></returns>
        public static AnyString Create(string s)
            => s.IsNullOrEmpty()
                ? EmptyString.Default
                : CanCreateWhiteSpaceStringFrom(s)
                    .Select(_ => (AnyString)new SomeString(s))
                    .GetValue(new WhitespaceString(s));


        /// <summary>
        /// Determines if two string objects have the same value.
        /// A parameter specifies the culture, case, and sort rules used in the comparison.
        /// </summary>
        /// <param name="x">The first string object</param>
        /// <param name="y">The second string object</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
        /// <returns></returns>
        public static bool Equals(AnyString x, AnyString y, StringComparison comparisonType)
            => string.Equals(x.Value, y.Value, comparisonType);

        /// <summary>
        /// Converts a <see cref="string"/> to a functional string object
        /// using <see cref="Create(string)"/>
        /// </summary>
        /// <param name="s">The <see cref="string"/> to convert implicitly</param>
        public static implicit operator AnyString(string s) => Create(s);

        /// <summary>
        /// Determines whether this string object contains a specified string.
        /// </summary>
        /// <param name="value">The string to seek.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> is empty or occurs in this string object;
        ///   otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Contains(string value)
            => value != null && Value.Contains(value);

        /// <summary>
        /// Determines whether this string object contains a specified string.
        /// </summary>
        /// <param name="value">The string to seek.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> is empty or occurs in this string object;
        ///   otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Contains(string value, StringComparison comparisonType)
            => value != null && Value.Contains(value, comparisonType);


        /// <summary>
        /// Determines whether this string object contains a specified character.
        /// </summary>
        /// <param name="value">The character to seek.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> occurs in this string object;
        ///   otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Contains(char value)
            => Value.Contains(value);

        /// <summary>
        /// Determines whether this string object contains a specified character.
        /// </summary>
        /// <param name="value">The character to seek.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> occurs in this string object;
        ///   otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Contains(char value, StringComparison comparisonType)
            => Value.Contains(value, comparisonType);


        /// <summary>
        /// Determines whether the end of this string object matches a specified string.
        /// </summary>
        /// <param name="value">The string value to compare with the end of this string object.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> matches with
        ///   the end of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool EndsWith(string value)
            => value != null && Value.EndsWith(value);

        /// <summary>
        /// Determines whether the end of this string object matches a specified string.
        /// </summary>
        /// <param name="value">The string value to compare with the end of this string object.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> matches with
        ///   the end of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool EndsWith(string value, StringComparison comparisonType)
            => value != null && Value.EndsWith(value, comparisonType);

        /// <summary>
        /// Determines whether the last character of this string object mathes a specified one.
        /// </summary>
        /// <param name="value">The caracter value to compare with the end of this string object.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> matches with
        ///   the end of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool EndsWith(char value)
            => Value.EndsWith(value);

        /// <summary>
        /// Determines whether this string object has the same content as a specified one.
        /// </summary>
        /// <param name="other">The string object to compare this instance to.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="other"/> matches with
        ///   the content of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Equals(AnyString other, StringComparison comparisonType)
            => other != null && string.Equals(Value, other.Value, comparisonType);

        /// <summary>
        /// Determines whether this string object has the same content as a specified one.
        /// </summary>
        /// <param name="other">The string object to compare this instance to.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="other"/> matches with
        ///   the content of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Equals(AnyString other)
            => other != null && string.Equals(Value, other.Value);

        /// <summary>
        /// Determines whether this string object has the same content as a specified string.
        /// </summary>
        /// <param name="other">The string to compare this instance to.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="other"/> matches with
        ///   the content of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Equals(string other, StringComparison comparisonType)
            => other != null && string.Equals(Value, other, comparisonType);

        /// <summary>
        /// Determines whether this string object has the same content as a specified string.
        /// </summary>
        /// <param name="other">The string to compare this instance to.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="other"/> matches with
        ///   the content of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Equals(string other)
            => other != null && string.Equals(Value, other);


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString() => Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override bool Equals(object? obj) 
            => obj switch
                {
                    AnyString f => Equals(f),
                    string s => Equals(s),
                    _ => false
                };
#pragma warning restore CS1591 

        /// <summary>
        /// Determines whether the beginning of this string object matches a specified string.
        /// </summary>
        /// <param name="value">The string value to compare with the beginning of this string object.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> matches with
        ///   the beginning of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool StartsWith(string value)
            => value != null && Value.StartsWith(value);

        /// <summary>
        /// Determines whether the beginning of this string object matches a specified string.
        /// </summary>
        /// <param name="value">The string value to compare with the beginning of this string object.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> matches with
        ///   the beginning of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool StartsWith(string value, StringComparison comparisonType)
            => value != null && Value.StartsWith(value, comparisonType);

        /// <summary>
        /// Determines whether the first character of this string object mathes a specified one.
        /// </summary>
        /// <param name="value">The caracter value to compare with the beginning of this string object.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> matches with
        ///   the first character of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool StartsWith(char value)
            => Value.StartsWith(value);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string
        /// in the current string object. Parameters specify the starting search position
        /// in the current string, the number of characters in the current string to search,
        /// and the type of search to use for the specified string.
        /// </summary>
        /// <param name="value">The string so seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found,
        /// or <c>-1</c> if it is not. If <paramref name="value"/> is an empty string, the return value is <paramref name="startIndex"/>.</returns>
        public virtual int IndexOf(string value, int startIndex, int count, StringComparison comparisonType)
            => value != null
            ? Value.IndexOf(value, startIndex, count, comparisonType)
            : -1;

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string
        /// in the current string object. Parameters specify the starting search position
        /// in the current string, and the type of search to use for the specified string.
        /// </summary>
        /// <param name="value">The string so seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found,
        /// or <c>-1</c> if it is not. If <paramref name="value"/> is an empty string, the return value is <paramref name="startIndex"/>.</returns>
        public virtual int IndexOf(string value, int startIndex, StringComparison comparisonType)
            => value != null
                ? Value.IndexOf(value, startIndex, comparisonType)
                : -1;

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string
        /// in the current string object. Parameters specify the type of search to use for the specified string.
        /// </summary>
        /// <param name="value">The string so seek.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found,
        /// or <c>-1</c> if it is not. If <paramref name="value"/> is an empty string, the return value is <c>0</c>.</returns>
        public virtual int IndexOf(string value, StringComparison comparisonType)
            => value != null
                ? Value.IndexOf(value, comparisonType)
                : -1;

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string
        /// in the current string object. Parameters specify the starting search position
        /// in the current string, the number of characters in the current string to search.
        /// </summary>
        /// <param name="value">The string so seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found,
        /// or <c>-1</c> if it is not. If <paramref name="value"/> is an empty string, the return value is <paramref name="startIndex"/>.</returns>
        public virtual int IndexOf(string value, int startIndex, int count)
            => value != null
                ? Value.IndexOf(value, startIndex, count)
                : -1;

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string
        /// in the current string object. Parameters specify the starting search position
        /// in the current string.
        /// </summary>
        /// <param name="value">The string so seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found,
        /// or <c>-1</c> if it is not. If <paramref name="value"/> is an empty string, the return value is <paramref name="startIndex"/>.</returns>
        public virtual int IndexOf(string value, int startIndex)
            => value != null
                ? Value.IndexOf(value, startIndex)
                : -1;

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string
        /// in the current string object.
        /// </summary>
        /// <param name="value">The string so seek.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found,
        /// or <c>-1</c> if it is not. If <paramref name="value"/> is an empty string, the return value is <c>0</c>.</returns>
        public virtual int IndexOf(string value)
            => value != null
                ? Value.IndexOf(value)
                : -1;

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified character
        /// in this instance. The search starts at a specified character position and examines
        /// a specified number of character positions.
        /// </summary>
        /// <param name="value">The character so seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>The zero-based index position of the value parameter if that character is found,
        /// or <c>-1</c> if it is not.</returns>
        public virtual int IndexOf(char value, int startIndex, int count)
            => Value.IndexOf(value, startIndex, count);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified character
        /// in this instance. The search starts at a specified character position.
        /// </summary>
        /// <param name="value">The character so seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of the value parameter if that character is found,
        /// or <c>-1</c> if it is not.</returns>
        public virtual int IndexOf(char value, int startIndex)
            => Value.IndexOf(value, startIndex);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified character
        /// in this instance.
        /// </summary>
        /// <param name="value">The character so seek.</param>
        /// <returns>The zero-based index position of the value parameter if that character is found,
        /// or <c>-1</c> if it is not.</returns>
        public virtual int IndexOf(char value)
            => Value.IndexOf(value);

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of any
        /// character in a specified array of Unicode characters. The search starts at a
        /// specified character position and examines a specified number of character positions.
        /// </summary>
        /// <param name="anyOf"> A Unicode character array containing one or more characters to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>The zero-based index position of the first occurrence in this instance where
        /// any character in <paramref name="anyOf"/> was found; <c>-1</c> if no character in <paramref name="anyOf"/> was found.</returns>
        public virtual int IndexOfAny(char[] anyOf, int startIndex, int count)
            => Value.IndexOfAny(anyOf, startIndex, count);

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of any
        /// character in a specified array of Unicode characters. The search starts at a
        /// specified character position.
        /// </summary>
        /// <param name="anyOf"> A Unicode character array containing one or more characters to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of the first occurrence in this instance where
        /// any character in <paramref name="anyOf"/> was found; <c>-1</c> if no character in <paramref name="anyOf"/> was found.</returns>
        public virtual int IndexOfAny(char[] anyOf, int startIndex)
            => Value.IndexOfAny(anyOf, startIndex);

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of any
        /// character in a specified array of Unicode characters.
        /// </summary>
        /// <param name="anyOf"> A Unicode character array containing one or more characters to seek.</param>
        /// <returns>The zero-based index position of the first occurrence in this instance where
        /// any character in <paramref name="anyOf"/> was found; <c>-1</c> if no character in <paramref name="anyOf"/> was found.</returns>
        public virtual int IndexOfAny(char[] anyOf)
            => Value.IndexOfAny(anyOf);

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string
        /// in the current string object. The search starts at a specified character position and
        /// proceeds backward toward the beginning of the string for the specified number
        /// of character positions. A parameter specifies the type of comparison to perform
        /// when searching for the specified string.
        /// </summary>
        /// <param name="value">The string so seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found,
        /// or <c>-1</c> if it is not. If <paramref name="value"/> is an empty string, the return value is <paramref name="startIndex"/>.</returns>
        public virtual int LastIndexOf(string value, int startIndex, int count, StringComparison comparisonType)
            => value != null
                ? Value.LastIndexOf(value, startIndex, count, comparisonType)
                : -1;

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string
        /// in the current string object. The search starts at a specified character position and
        /// proceeds backward toward the beginning of the string. A parameter specifies the type of comparison to perform
        /// when searching for the specified string.
        /// </summary>
        /// <param name="value">The string so seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found,
        /// or <c>-1</c> if it is not. If <paramref name="value"/> is an empty string, the return value is <paramref name="startIndex"/>.</returns>
        public virtual int LastIndexOf(string value, int startIndex, StringComparison comparisonType)
            => value != null
                ? Value.LastIndexOf(value, startIndex, comparisonType)
                : -1;

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string
        /// in the current string object. A parameter specifies the type of comparison to perform
        /// when searching for the specified string.
        /// </summary>
        /// <param name="value">The string so seek.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found,
        /// or <c>-1</c> if it is not. If <paramref name="value"/> is an empty string, the return value is <c>0</c>.</returns>
        public virtual int LastIndexOf(string value, StringComparison comparisonType)
            => value != null
                ? Value.LastIndexOf(value, comparisonType)
                : -1;

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string
        /// in the current string object. The search starts at a specified character position and
        /// proceeds backward toward the beginning of the string for the specified number
        /// of character positions.
        /// </summary>
        /// <param name="value">The string so seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found,
        /// or <c>-1</c> if it is not. If <paramref name="value"/> is an empty string, the return value is <paramref name="startIndex"/>.</returns>
        public virtual int LastIndexOf(string value, int startIndex, int count)
            => value != null
                ? Value.LastIndexOf(value, startIndex, count)
                : -1;

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string
        /// in the current string object. The search starts at a specified character position and
        /// proceeds backward toward the beginning of the string.
        /// </summary>
        /// <param name="value">The string so seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found,
        /// or <c>-1</c> if it is not. If <paramref name="value"/> is an empty string, the return value is <paramref name="startIndex"/>.</returns>
        public virtual int LastIndexOf(string value, int startIndex)
            => Value.LastIndexOf(value, startIndex);

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string
        /// in the current string object.
        /// </summary>
        /// <param name="value">The string so seek.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found,
        /// or <c>-1</c> if it is not. If <paramref name="value"/> is an empty string, the return value is <c>0</c>.</returns>
        public virtual int LastIndexOf(string value)
            => value != null
                ? Value.LastIndexOf(value)
                : -1;

        /// <summary>
        /// Reports the zero-based index position of the last occurrence of the specified
        /// Unicode character in a substring within this instance. The search starts at a
        /// specified character position and proceeds backward toward the beginning of the
        /// string for a specified number of character positions.
        /// </summary>
        /// <param name="value">The character so seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>The zero-based index position of the value parameter if that character is found,
        /// or <c>-1</c> if it is not.</returns>
        public virtual int LastIndexOf(char value, int startIndex, int count)
            => Value.LastIndexOf(value, startIndex, count);

        /// <summary>
        /// Reports the zero-based index position of the last occurrence of the specified
        /// Unicode character in a substring within this instance. The search starts at a
        /// specified character position and proceeds backward toward the beginning of the string.
        /// </summary>
        /// <param name="value">The character so seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of the value parameter if that character is found,
        /// or <c>-1</c> if it is not.</returns>
        public virtual int LastIndexOf(char value, int startIndex)
            => Value.LastIndexOf(value, startIndex);

        /// <summary>
        /// Reports the zero-based index position of the last occurrence of the specified
        /// Unicode character in a substring within this instance.
        /// </summary>
        /// <param name="value">The character so seek.</param>
        /// <returns>The zero-based index position of the value parameter if that character is found,
        /// or <c>-1</c> if it is not.</returns>
        public virtual int LastIndexOf(char value)
            => Value.LastIndexOf(value);

        /// <summary>
        /// Reports the zero-based index position of the last occurrence in this instance
        /// of one or more characters specified in a Unicode array. The search starts at
        /// a specified character position and proceeds backward toward the beginning of
        /// the string for a specified number of character positions.
        /// </summary>
        /// <param name="anyOf"> A Unicode character array containing one or more characters to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>The zero-based index position of the last occurrence in this instance where
        /// any character in <paramref name="anyOf"/> was found; <c>-1</c> if no character in <paramref name="anyOf"/> was found.</returns>
        public virtual int LastIndexOfAny(char[] anyOf, int startIndex, int count)
            => Value.LastIndexOfAny(anyOf, startIndex, count);

        /// <summary>
        /// Reports the zero-based index position of the last occurrence in this instance
        /// of one or more characters specified in a Unicode array. The search starts at
        /// a specified character position and proceeds backward toward the beginning of
        /// the string.
        /// </summary>
        /// <param name="anyOf"> A Unicode character array containing one or more characters to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of the last occurrence in this instance where
        /// any character in <paramref name="anyOf"/> was found; <c>-1</c> if no character in <paramref name="anyOf"/> was found.</returns>
        public virtual int LastIndexOfAny(char[] anyOf, int startIndex)
            => Value.LastIndexOfAny(anyOf, startIndex);

        /// <summary>
        /// Reports the zero-based index position of the last occurrence in this instance
        /// of one or more characters specified in a Unicode array.
        /// </summary>
        /// <param name="anyOf"> A Unicode character array containing one or more characters to seek.</param>
        /// <returns>The zero-based index position of the last occurrence in this instance where
        /// any character in <paramref name="anyOf"/> was found; <c>-1</c> if no character in <paramref name="anyOf"/> was found.</returns>
        public virtual int LastIndexOfAny(char[] anyOf)
            => anyOf != null
                ? Value.LastIndexOfAny(anyOf)
                : -1;

        /// <summary>
        /// Returns the char at a specified position,
        /// if that position is valid.
        /// </summary>
        /// <param name="charPosition">The position in the string.
        /// A negative value is subtracted from the strings length, thus indexing backwards, 
        /// whereas -1 references the final character.</param>
        /// <returns>A <see cref="Just{Char}"/> instance containing the character at the requested position,
        /// if such is present; otherwise <see cref="Nothing{Char}"/></returns>
        protected Maybe<char> GetCharAtPosition(int charPosition) 
            => Math.Abs(charPosition) >= Length
                ? Nothing<char>.Instance
                : (charPosition >= 0 ? Value[charPosition] : Value[Value.Length - charPosition]).ToMaybe();

        /// <summary>
        /// Checks, if a string meets the requirements of <see cref="WhitespaceString"/>
        /// </summary>
        /// <param name="s">The string to create a <see cref="WhitespaceString"/> from.</param>
        /// <returns>An instance of <see cref="Just{ArgumentException}"/> containing the violated requirement;
        /// otherwise <see cref="Nothing{ArgumentException}"/>, if a <see cref="WhitespaceString"/> instance
        /// can be created from <paramref name="s"/></returns>
        internal static Maybe<ArgumentException> CanCreateWhiteSpaceStringFrom(string s)
        {
            if (s.IsNullOrEmpty()) return new ArgumentException("Provided string must neither be <null> nor empty.").ToMaybe();
            else if (!string.IsNullOrWhiteSpace(s)) new ArgumentException("Provided string must only contain whitespace characters, meaning any Unicode separators.");
            return Nothing<ArgumentException>.Instance;
        }


        /// <summary>
        /// Checks, if a string meets the requirements of <see cref="SomeString"/>
        /// </summary>
        /// <param name="s">The string to create a <see cref="SomeString"/> from.</param>
        /// <returns>An instance of <see cref="Just{ArgumentException}"/> containing the violated requirement;
        /// otherwise <see cref="Nothing{ArgumentException}"/>, if a <see cref="SomeString"/> instance
        /// can be created from <paramref name="s"/></returns>
        internal static Maybe<ArgumentException> CanCreateSomeStringFrom(string s) 
            => s.IsNothing()
                ? new ArgumentException("Provided string must neither be <null> nor consist only of whitespace characters.").ToMaybe()
                : Nothing<ArgumentException>.Instance;

        /// <summary>
        /// Implicitely converts an <see cref="AnyString"/> into a <see cref="string"/>
        /// </summary>
        /// <param name="s">The string object to convert</param>
#pragma warning disable CS8603 // Possible null reference return.
        public static implicit operator string(AnyString s) => s != null ? s.Value ?? string.Empty : string.Empty;
#pragma warning restore CS8603 // Possible null reference return.

    }
}