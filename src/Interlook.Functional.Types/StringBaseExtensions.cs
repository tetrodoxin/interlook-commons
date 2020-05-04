using System;
using System.Globalization;

namespace Interlook.Functional.Types
{
    /// <summary>
    /// Defines extensions methods for classes deriving from <see cref="StringBase"/>
    /// </summary>
    public static class StringBaseExtensions
    {
        /// <summary>
        /// Returns a copy of a <see cref="SomeString"/> object converted to uppercase.
        /// </summary>
        /// <param name="s">The string object to convert.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>A new instance of <see cref="SomeString"/></returns>
        internal static SomeString ToUpper(this SomeString s, CultureInfo culture)
            => new SomeString(culture.TextInfo.ToUpper(s.Value));

        /// <summary>
        /// Returns a copy of a <see cref="SomeString"/> object converted to uppercase.
        /// </summary>
        /// <param name="s">The string object to convert.</param>
        /// <returns>A new instance of <see cref="SomeString"/></returns>
        internal static SomeString ToUpper(this SomeString s)
            => ToUpper(s, CultureInfo.CurrentCulture);

        /// <summary>
        /// Returns a copy of a <see cref="SomeString"/> object converted to lowercase.
        /// </summary>
        /// <param name="s">The string object to convert.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>A new instance of <see cref="SomeString"/></returns>
        internal static SomeString ToLower(this SomeString s, CultureInfo culture)
            => new SomeString(culture.TextInfo.ToLower(s.Value));

        /// <summary>
        /// Returns a copy of a <see cref="SomeString"/> object converted to lowercase.
        /// </summary>
        /// <param name="s">The string object to convert.</param>
        /// <returns>A new instance of <see cref="SomeString"/></returns>
        internal static SomeString ToLower(this SomeString s)
            => ToLower(s, CultureInfo.CurrentCulture);

        /// <summary>
        /// Returns a copy of a <see cref="NonEmptyString"/> object converted to uppercase.
        /// </summary>
        /// <param name="s">The string object to convert.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>A new instance of <see cref="NonEmptyString"/></returns>
        internal static NonEmptyString ToUpper(NonEmptyString s, CultureInfo culture)
            => new NonEmptyString(culture.TextInfo.ToUpper(s.Value));

        /// <summary>
        /// Returns a copy of a <see cref="NonEmptyString"/> object converted to uppercase.
        /// </summary>
        /// <param name="s">The string object to convert.</param>
        /// <returns>A new instance of <see cref="NonEmptyString"/></returns>
        internal static NonEmptyString ToUpper(NonEmptyString s)
            => ToUpper(s, CultureInfo.CurrentCulture);

        /// <summary>
        /// Returns a copy of a <see cref="NonEmptyString"/> object converted to lowercase.
        /// </summary>
        /// <param name="s">The string object to convert.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>A new instance of <see cref="NonEmptyString"/></returns>
        internal static NonEmptyString ToLower(NonEmptyString s, CultureInfo culture)
            => new NonEmptyString(culture.TextInfo.ToLower(s.Value));

        /// <summary>
        /// Returns a copy of a <see cref="NonEmptyString"/> object converted to lowercase.
        /// </summary>
        /// <param name="s">The string object to convert.</param>
        /// <returns>A new instance of <see cref="NonEmptyString"/></returns>
        internal static NonEmptyString ToLower(NonEmptyString s)
            => ToLower(s, CultureInfo.CurrentCulture);

        /// <summary>
        /// Concatenates a <see cref="SomeString"/> instance and a <see cref="string"/> object.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="s2">The string to append.</param>
        /// <returns></returns>
        internal static SomeString Concat(this SomeString s, string s2)
            => new SomeString(string.Concat(s.Value, s2));

        /// <summary>
        /// Concatenates a <see cref="SomeString"/> instance and a <see cref="char"/> object.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="c">The character to append.</param>
        /// <returns></returns>
        internal static SomeString Concat(this SomeString s, char c)
            => new SomeString(s.Value + c);

        /// <summary>
        /// Concatenates a <see cref="NonEmptyString"/> instance and a <see cref="char"/> object.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="c">The character to append.</param>
        /// <returns></returns>
        internal static NonEmptyString Concat(this NonEmptyString s, char c)
            => new NonEmptyString(s.Value + c);

        /// <summary>
        /// Concatenates a <see cref="EmptyString"/> instance and a <see cref="char"/> object.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="c">The character to append.</param>
        /// <returns></returns>
        internal static NonEmptyString Concat(this EmptyString s, char c)
            => new NonEmptyString(c.ToString());

        /// <summary>
        /// Concatenates a <see cref="SomeString"/> instance and two <see cref="string"/> objects.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        internal static SomeString Concat(this SomeString s, string s2, string s3)
            => new SomeString(string.Concat(s.Value, s2, s3));

        /// <summary>
        /// Concatenates a <see cref="SomeString"/> instance and three <see cref="string"/> objects.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        /// <param name="s4">The third string to append.</param>
        internal static SomeString Concat(this SomeString s, string s2, string s3, string s4)
            => new SomeString(string.Concat(s.Value, s2, s3, s4));

        /// <summary>
        /// Concatenates a <see cref="SomeString"/> instance and several <see cref="string"/> objects.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="values">The string values to append</param>
        internal static SomeString Concat(this SomeString s, params string[] values)
        {
            string[] final = new string[values.Length + 1];

            final[0] = s.Value;
            Buffer.BlockCopy(values, 0, final, 1, values.Length);
            return new SomeString(string.Concat(final));
        }

        /// <summary>
        /// Concatenates two <see cref="SomeString"/> instances.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="s2">The first string to append.</param>
        internal static SomeString Concat(this SomeString s, SomeString s2)
            => new SomeString(string.Concat(s.Value, s2));

        /// <summary>
        /// Concatenates three <see cref="SomeString"/> instances.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        internal static SomeString Concat(this SomeString s, SomeString s2, SomeString s3)
            => new SomeString(string.Concat(s.Value, s2, s3));

        /// <summary>
        /// Concatenates four <see cref="SomeString"/> instances.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        /// <param name="s4">The third string to append.</param>
        internal static SomeString Concat(this SomeString s, SomeString s2, SomeString s3, SomeString s4)
            => new SomeString(string.Concat(s.Value, s2, s3, s4));

        /// <summary>
        /// Concatenates several <see cref="SomeString"/> instances.
        /// </summary>
        /// <param name="values">The string values to concatenate</param>
        internal static SomeString Concat(params SomeString[] values)
        {
            string[] final = new string[values.Length];

            for (int i = 0; i <= values.Length; i++)
                final[i] = values[i].Value;

            return new SomeString(string.Concat(final));
        }

        /// <summary>
        /// Concatenates a <see cref="NonEmptyString"/> instance and a <see cref="string"/> object.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="s2">The string to append.</param>
        /// <returns></returns>
        internal static NonEmptyString Concat(this NonEmptyString s, string s2)
            => new NonEmptyString(string.Concat(s.Value, s2));

        /// <summary>
        /// Concatenates a <see cref="NonEmptyString"/> instance and two <see cref="string"/> objects.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        internal static NonEmptyString Concat(this NonEmptyString s, string s2, string s3)
            => new NonEmptyString(string.Concat(s.Value, s2, s3));

        /// <summary>
        /// Concatenates a <see cref="NonEmptyString"/> instance and three <see cref="string"/> objects.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        /// <param name="s4">The third string to append.</param>
        internal static NonEmptyString Concat(this NonEmptyString s, string s2, string s3, string s4)
            => new NonEmptyString(string.Concat(s.Value, s2, s3, s4));

        /// <summary>
        /// Concatenates a <see cref="NonEmptyString"/> instance and several <see cref="string"/> objects.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="values">The string values to append</param>
        internal static NonEmptyString Concat(this NonEmptyString s, params string[] values)
        {
            string[] final = new string[values.Length + 1];

            final[0] = s.Value;
            Buffer.BlockCopy(values, 0, final, 1, values.Length);
            return new NonEmptyString(string.Concat(final));
        }

        /// <summary>
        /// Concatenates two <see cref="NonEmptyString"/> instances.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="s2">The first string to append.</param>
        internal static NonEmptyString Concat(this NonEmptyString s, NonEmptyString s2)
            => new NonEmptyString(string.Concat(s.Value, s2));

        /// <summary>
        /// Concatenates three <see cref="NonEmptyString"/> instances.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        internal static NonEmptyString Concat(this NonEmptyString s, NonEmptyString s2, NonEmptyString s3)
            => new NonEmptyString(string.Concat(s.Value, s2, s3));

        /// <summary>
        /// Concatenates four <see cref="NonEmptyString"/> instances.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="s2">The first string to append.</param>
        /// <param name="s3">The second string to append.</param>
        /// <param name="s4">The third string to append.</param>
        internal static NonEmptyString Concat(this NonEmptyString s, NonEmptyString s2, NonEmptyString s3, NonEmptyString s4)
            => new NonEmptyString(string.Concat(s.Value, s2, s3, s4));

        /// <summary>
        /// Concatenates several <see cref="NonEmptyString"/> instances.
        /// </summary>
        /// <param name="values">The string values to concatenate</param>
        internal static NonEmptyString Concat(params NonEmptyString[] values)
        {
            string[] final = new string[values.Length];

            for (int i = 0; i <= values.Length; i++)
                final[i] = values[i].Value;

            return new NonEmptyString(string.Concat(final));
        }
    }
}