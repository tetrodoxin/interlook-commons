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
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    /// <summary>
    /// Contains helper methods for enumerators and LINQ2Objects
    /// </summary>
    public static class EnumerationExtensions
    {
        /// <summary>
        /// Determines, if a sequence contains any elements WITHOUT
        /// throwing a <see cref="ArgumentNullException"/>, if <paramref name="source"/> was <c>null</c>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequencde to check for emptiness.</param>
        /// <returns><c>false</c>, if <paramref name="source"/> is empty or <c>null</c>, otherwise <c>true</c>.</returns>
        public static bool AnyProtected<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                return false;

            return source.Any();
        }

        /// <summary>
        /// Determines, if a sequence contains any elements, with satisfy a condition.
        /// Does NOT THROW <see cref="ArgumentNullException"/>, if <paramref name="source"/> was <c>null</c>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequencde to check for emptiness.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns><c>false</c>, if <paramref name="source"/> is <c>null</c> or contains no elements,
        /// which did not pass the test in the specified predicate; otherwise <c>true</c>.</returns>
        public static bool AnyProtected<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
                return false;

            return source.Any(predicate);
        }

        /// <summary>
        /// Appends a value to the end of the sequence. if a specified condition is met.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values.</param>
        /// <param name="condition">The predicate, which constraints the appending of the new element.</param>
        /// <param name="newElement">The value to append to <paramref name="source"/>.</param>
        /// <returns>A new Sequence that ends with <paramref name="newElement"/>.
        /// Is never <c>null</c>.</returns>
        public static IEnumerable<TSource> AppendIf<TSource>(this IEnumerable<TSource> source, bool condition, TSource newElement)
        {
            if (source != null)
            {
                foreach (var e in source)
                {
                    yield return e;
                }
            }

            if (condition)
                yield return newElement;
        }

        /// <summary>
        /// Appends a value to the end of the sequence. if a specified condition is met.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values.</param>
        /// <param name="condition">The predicate, which constraints the appending of the new element.</param>
        /// <param name="newElementFactory">The factory method, executed if the specified constraint in <paramref name="condition"/> is met,
        /// which creates a value to append to <paramref name="source"/>.</param>
        /// <returns>A new Sequence that ends with the new value created by <paramref name="newElementFactory"/>.
        /// Is never <c>null</c>.</returns>
        public static IEnumerable<TSource> AppendIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource> newElementFactory)
        {
            if (newElementFactory == null)
                throw new ArgumentNullException(nameof(newElementFactory));

            if (source != null)
            {
                foreach (var e in source)
                {
                    yield return e;
                }
            }

            if (condition)
                yield return newElementFactory();
        }

        /// <summary>
        /// Returns a new Sequence containing the value.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the new sequence.</typeparam>
        /// <param name="obj">The object to be added to the new sequence.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> sequence containing <paramref name="obj"/>.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this T obj)
        {
            yield return obj;
        }

        /// <summary>
        /// <para>
        /// Iterates a sequence like the <c>foreach</c> statement
        /// and executes <paramref name="action"/> for every element.
        /// </para>
        /// <para>This <c>foreach</c>-overload does NOT support a <c>break</c>-mechanism.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of the new sequence.</typeparam>
        /// <param name="source">The sequence to iterate.</param>
        /// <param name="action">The Action to be executed for every element.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (action != null && source != null)
            {
                foreach (var e in source)
                {
                    action(e);
                }
            }
        }

        /// <summary>
        /// <para>
        /// Iterates a sequence like the <c>foreach</c> statement
        /// and executes <paramref name="action"/> for every element.
        /// </para>
        /// <para>This <c>foreach</c>-overload does NOT support a <c>break</c>-mechanism.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of the new sequence.</typeparam>
        /// <param name="source">The sequence to iterate.</param>
        /// <param name="action">The Action to be executed for every element. 
        /// The second parameter represents the zero-based index of the source element.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (action != null && source != null)
            {
                int i = -1;
                foreach (var e in source)
                {
                    i += 1;
                    action(e, i);
                }
            }
        }

        /// <summary>
        /// <para>
        /// Iterates a sequence like the <c>foreach</c> statement
        /// and executes <paramref name="action"/> for every element.
        /// </para>
        /// <para>This <c>foreach</c>-overload supports a <c>break</c>-mechanism.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of the new sequence.</typeparam>
        /// <param name="source">The sequence to iterate.</param>
        /// <param name="action">The Action to be executed for every element. 
        /// The second parameter represents the zero-based index of the source element.
        /// The third parameter is an instance of <see cref="CancelProc"/>, which simulates
        /// the <c>break</c> statement of a <c>foreach</c>-loop.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int, CancelProc> action)
        {
            if (action != null && source != null)
            {
                int i = -1;
                var c = new CancelProc();
                foreach (var e in source)
                {
                    i += 1;
                    action(e, i, c);
                    if (c.CancelRequested) break;
                }
            }
        }

        /// <summary>
        /// <para>
        /// Iterates a sequence like the <c>foreach</c> statement
        /// and executes <paramref name="action"/> for every element.
        /// </para>
        /// <para>This <c>foreach</c>-overload supports a <c>break</c>-mechanism.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of the new sequence.</typeparam>
        /// <param name="source">The sequence to iterate.</param>
        /// <param name="action">The Action to be executed for every element. 
        /// The second parameter represents the zero-based index of the source element.</param>
        /// <param name="cancelingPredicate">A function, that is used as break-condition BEFORE every iteration.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action, Func<T, int, bool> cancelingPredicate)
        {
            if (action != null && source != null)
            {
                int i = -1;
                foreach (var e in source)
                {
                    i += 1;
                    if (cancelingPredicate(e, i)) break;
                    action(e, i);
                }
            }
        }

        /// <summary>
        /// Adds a value to the beginning of the sequence. if a specified condition is met.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values.</param>
        /// <param name="condition">The predicate, which constraints the appending of the new element.</param>
        /// <param name="newElement">The value to add to the beginning of <paramref name="source"/>.</param>
        /// <returns>A new Sequence that begins with <paramref name="newElement"/>.
        /// Is never <c>null</c>.</returns>
        public static IEnumerable<TSource> PrependIf<TSource>(this IEnumerable<TSource> source, bool condition, TSource newElement)
        {
            if (condition)
                yield return newElement;

            if (source != null)
            {
                foreach (var e in source)
                {
                    yield return e;
                }
            }
        }

        /// <summary>
        /// Adds a value to the beginning of the sequence. if a specified condition is met.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values.</param>
        /// <param name="condition">The predicate, which constraints the appending of the new element.</param>
        /// <param name="newElementFactory">The factory method, executed if the specified constraint in <paramref name="condition"/> is met,
        /// which creates a value to add to the beginning of <paramref name="source"/>.</param>
        /// <returns>A new Sequence that begins with the new value created by <paramref name="newElementFactory"/>.
        /// Is never <c>null</c>.</returns>
        public static IEnumerable<TSource> PrependIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource> newElementFactory)
        {
            if(newElementFactory == null)
                throw new ArgumentNullException(nameof(newElementFactory));

            if (condition)
                yield return newElementFactory();

            if (source != null)
            {
                foreach (var e in source)
                {
                    yield return e;
                }
            }
        }

        /// <summary>
        /// Helper flag for break support in <see cref="EnumerationExtensions.ForEach{T}(IEnumerable{T}, Action{T, int, CancelProc})"/>
        /// </summary>
        public struct CancelProc
        {
            /// <summary>
            /// Determines, if the current iteration is to be canceled
            /// </summary>
            public bool CancelRequested { get; private set; }

            /// <summary>
            /// Requests to cancel the iteration by setting <see cref="CancelRequested"/> to <c>true</c>.
            /// </summary>
            public void Cancel() { CancelRequested = true; }
        }
    }
}