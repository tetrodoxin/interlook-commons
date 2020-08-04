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

        public override bool Equals(object other) => other is EmptyString || (other is string str && str.Length == 0);

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
}