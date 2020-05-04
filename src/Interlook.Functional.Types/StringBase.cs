using Interlook.Monads;
using Interlook.Text;
using System;

namespace Interlook.Functional.Types
{
    /// <summary>
    /// A singleton object for empty strings in functional world.
    /// </summary>
    public sealed class EmptyString : StringBase
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

        public override bool Equals(object other) => other is EmptyString;

        public override bool Equals(StringBase other, StringComparison comparisonType) => other is EmptyString;

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
    }

    /// <summary>
    /// A functional string object that is non-empty,
    /// thus having a length > 0.
    /// </summary>
    public class NonEmptyString : StringBase
    {
        /// <summary>
        /// Returns the character at the specified position.
        /// </summary>
        /// <param name="charPosition">The position in the string.
        /// A negative value is subtracted from the strings length, thus indexing backwards.</param>
        /// <returns></returns>
        public char this[int charPosition] => charPosition >= 0 ? Value[charPosition] : Value[Value.Length - charPosition];

        internal NonEmptyString(string value) : base(value)
        {
        }
    }

    /// <summary>
    /// A functional string object containing actual characters,
    /// rather than whitespace-characters only.
    /// </summary>
    public sealed class SomeString : NonEmptyString
    {
        internal SomeString(string value) : base(value)
        {
        }
    }

    /// <summary>
    /// A base class for string objects in functional world.
    /// </summary>
    public abstract class StringBase : ObjectBase<string>
    {
        /// <summary>
        /// The length of the string.
        /// </summary>
        public int Length { get; }

        internal StringBase(string value) : base(value)
        {
            Length = value.Length;
        }

        /// <summary>
        /// Creates a functional string object from an instance of <see cref="string"/>.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <returns>An instance of <see cref="EmptyString"/>, <see cref="NonEmptyString"/> or
        /// <see cref="SomeString"/> according to the content of <paramref name="s"/></returns>
        public static StringBase Create(string s)
            => CreateNonEmpty(s)
                .MapEither(
                    _ => EmptyString.Default,
                    nonEmpty => CreateSome(nonEmpty)
                        .MapEither(_ => (StringBase)nonEmpty, some => some));

        /// <summary>
        /// Creates an instance of a non-empty string object.
        /// </summary>
        /// <param name="value">The non-empty content the string object.</param>
        /// <returns></returns>
        public static Either<Exception, NonEmptyString> CreateNonEmpty(string value)
            => value.ToExceptionEither()
                .FailIf(s => s.IsNullOrEmpty(), new ArgumentException("Provided string must not be empty", nameof(value)))
                .Select(s => new NonEmptyString(s));

        /// <summary>
        /// Creates an instance of a non-empty string object, that contains
        /// actual characters, rather than whitespace-characters only.
        /// </summary>
        /// <param name="value">The actual content the string object.</param>
        /// <returns></returns>
        public static Either<Exception, SomeString> CreateSome(NonEmptyString value)
            => value.ToExceptionEither()
                .FailIf(s => s.Value.IsNothing(), new ArgumentException("Provided string must contain actual characters not only whitespaces/tabs", nameof(value)))
                .Select(s => new SomeString(s));

        /// <summary>
        /// Creates an instance of a non-empty string object, that contains
        /// actual characters, rather than whitespace-characters only.
        /// </summary>
        /// <param name="value">The actual content the string object.</param>
        /// <returns></returns>
        public static Either<Exception, SomeString> CreateSome(string value)
            => CreateNonEmpty(value).Bind(CreateSome);

        /// <summary>
        /// Determines if two string objects have the same value.
        /// A parameter specifies the culture, case, and sort rules used in the comparison.
        /// </summary>
        /// <param name="x">The first string object</param>
        /// <param name="y">The second string object</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
        /// <returns></returns>
        public static bool Equals(StringBase x, StringBase y, StringComparison comparisonType)
            => string.Equals(x.Value, y.Value, comparisonType);

        /// <summary>
        /// Converts a <see cref="string"/> to a functional string object
        /// using <see cref="Create(string)"/>
        /// </summary>
        /// <param name="s">The <see cref="string"/> to convert implicitly</param>
        public static implicit operator StringBase(string s) => Create(s);

        /// <summary>
        /// Determines whether this string object contains a specified string.
        /// </summary>
        /// <param name="value">The string to seek.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> is empty or occurs in this string object;
        ///   otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Contains(string value)
            => Value.Contains(value);

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
#if NETCORE
            => Value.Contains(value, comparisonType);
#else
            => Value.IndexOf(value, comparisonType) >= 0;

#endif

        /// <summary>
        /// Determines whether the end of this string object matches a specified string.
        /// </summary>
        /// <param name="value">The string value to compare with the end of this string object.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> matches with
        ///   the end of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool EndsWith(string value)
            => Value.EndsWith(value);

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
            => Value.EndsWith(value, comparisonType);

        /// <summary>
        /// Determines whether the last character of this string object mathes a specified one.
        /// </summary>
        /// <param name="value">The caracter value to compare with the end of this string object.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> matches with
        ///   the end of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool EndsWith(char value)
#if NETCORE
            => Value.EndsWith(value);
#else
            => Value.EndsWith(value.ToString());

#endif

        /// <summary>
        /// Determines whether this string object has the same content as a specified one.
        /// </summary>
        /// <param name="other">The a string object to compare this instance to.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="other"/> matches with
        ///   the content of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Equals(StringBase other, StringComparison comparisonType)
            => string.Equals(Value, other.Value, comparisonType);

        /// <summary>
        /// Determines whether the beginning of this string object matches a specified string.
        /// </summary>
        /// <param name="value">The string value to compare with the beginning of this string object.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> matches with
        ///   the beginning of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool StartsWith(string value)
            => Value.StartsWith(value);

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
            => Value.StartsWith(value, comparisonType);

        /// <summary>
        /// Determines whether the first character of this string object mathes a specified one.
        /// </summary>
        /// <param name="value">The caracter value to compare with the beginning of this string object.</param>
        /// <returns>
        ///   <c>true</c> if the value in <paramref name="value"/> matches with
        ///   the first character of this string object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool StartsWith(char value)
#if NETCORE
            => Value.StartsWith(value);
#else
            => Value.StartsWith(value.ToString());

#endif

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
            => Value.IndexOf(value, startIndex, count, comparisonType);

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
            => Value.IndexOf(value, startIndex, comparisonType);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string
        /// in the current string object. Parameters specify the type of search to use for the specified string.
        /// </summary>
        /// <param name="value">The string so seek.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found,
        /// or <c>-1</c> if it is not. If <paramref name="value"/> is an empty string, the return value is <c>0</c>.</returns>
        public virtual int IndexOf(string value, StringComparison comparisonType)
            => Value.IndexOf(value, comparisonType);

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
            => Value.IndexOf(value, startIndex, count);

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
            => Value.IndexOf(value, startIndex);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string
        /// in the current string object.
        /// </summary>
        /// <param name="value">The string so seek.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found,
        /// or <c>-1</c> if it is not. If <paramref name="value"/> is an empty string, the return value is <c>0</c>.</returns>
        public virtual int IndexOf(string value)
            => Value.IndexOf(value);

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
            => Value.LastIndexOf(value, startIndex, count, comparisonType);

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
            => Value.LastIndexOf(value, startIndex, comparisonType);

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
            => Value.LastIndexOf(value, comparisonType);

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
            => Value.LastIndexOf(value, startIndex, count);

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
            => Value.LastIndexOf(value);

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
            => Value.LastIndexOfAny(anyOf);
    }
}