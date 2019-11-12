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

namespace Interlook.Monads
{
    /// <summary>
    /// Represents a monadic writer.
    /// This monad is innately lazy.
    /// </summary>
    /// <typeparam name="TCollection">The type of the elements to be written.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public delegate WriterEntry<TCollection, TValue> Writer<TCollection, TValue>();

    /// <summary>
    /// The result value of the writer monad
    /// </summary>
    /// <typeparam name="TCollection">The type of the elements to be written.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class WriterEntry<TCollection, TValue>
    {
        /// <summary>
        /// Gets the current value.
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// Gets the elements, written so far.
        /// </summary>
        public IEnumerable<TCollection> Elements { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriterEntry{TCollection, TValue}" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="output">The output to be written. Defaults to empty if <c>null</c>.</param>
        public WriterEntry(TValue value, IEnumerable<TCollection> output)
        {
            Value = value;
            Elements = output ?? new TCollection[0];
        }
    }

    /// <summary>
    /// Extension methods for writer monad
    /// </summary>
    public static class Writer
    {
        /// <summary>
        /// Returns a new, empty monadic writer with the specified value.
        /// </summary>
        /// <typeparam name="TCollection">The type of the elements to be written.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Writer<TCollection, TValue> ReturnWriter<TCollection, TValue>(TValue value)
        {
            return () => new WriterEntry<TCollection, TValue>(value, new TCollection[0]);
        }


        /// <summary>
        /// Writes an elements into a new writer with a specified value.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="elementToWrite">The element to write.</param>
        /// <returns></returns>
        public static Writer<TCollection, TValue> Tell<TCollection, TValue>(TValue value, TCollection elementToWrite)
        {
            return () => new WriterEntry<TCollection, TValue>(value, new TCollection[1] { elementToWrite });
        }

        /// <summary>
        /// Writes elements into a new writer with a specified value.
        /// </summary>
        /// <typeparam name="TCollection">The type of the elements to be written.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">elements</exception>
        public static Writer<TCollection, TValue> Tell<TCollection, TValue>(TValue value, IEnumerable<TCollection> elements)
        {
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            return () => new WriterEntry<TCollection, TValue>(value, elements);
        }

        /// <summary>
        /// Writes an element into a writer without a value.
        /// </summary>
        /// <typeparam name="TCollection">The type of the elements to be written.</typeparam>
        /// <param name="elementToWrite">The element to write.</param>
        /// <returns></returns>
        public static Writer<TCollection, Unit> Tell<TCollection>(TCollection elementToWrite) 
            => () => new WriterEntry<TCollection, Unit>(Unit.Default, new TCollection[] { elementToWrite });


        /// <summary>
        /// Maps a writer to a new writer.
        /// </summary>
        /// <typeparam name="TCollection">The type of the elements to be written.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TValueNew">The type of the new value.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="function">The function, that selects a type <typeparamref name="TValueNew" />
        /// from type <typeparamref name="TValue" /></param>
        /// <returns>
        /// A new <see cref="Writer{TArgument, TResultNew}" /> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="obj" /> or <paramref name="function" /> was <c>null</c>.</exception>
        public static Writer<TCollection, TValueNew> Map<TCollection, TValue, TValueNew>(this Writer<TCollection, TValue> obj, Func<TValue, TValueNew> function)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (function == null) throw new ArgumentNullException(nameof(function));

            return () =>
                {
                    var original = obj();
                    var mapped = function(original.Value);
                    return new WriterEntry<TCollection, TValueNew>(mapped, original.Elements);
                };
        }

        /// <summary>
        /// For LINQ support. Just calls <see cref="Writer.Map{TCollection, TValue, TValueNew}(Writer{TCollection, TValue}, Func{TValue, TValueNew})"/>
        /// </summary>
        /// <typeparam name="TCollection">The type of the elements to be written.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TValueNew">The type of the new value.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="function">The function, that selects a type <typeparamref name="TValueNew" />
        /// from type <typeparamref name="TValue" /></param>
        /// <returns>
        /// A new <see cref="Writer{TArgument, TResultNew}" /> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="obj" /> or <paramref name="function" /> was <c>null</c>.</exception>
        public static Writer<TCollection, TValueNew> Select<TCollection, TValue, TValueNew>(this Writer<TCollection, TValue> obj, Func<TValue, TValueNew> function)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (function == null) throw new ArgumentNullException(nameof(function));

            return () =>
            {
                var original = obj();
                var mapped = function(original.Value);
                return new WriterEntry<TCollection, TValueNew>(mapped, original.Elements);
            };
        }

        /// <summary>
        /// Binds a function to a monadic writer.
        /// </summary>
        /// <typeparam name="TCollection">The type of the elements to be written.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TValueNew">The type of the new value.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="function">The function, that selects a type <typeparamref name="TValueNew" />
        /// from type <typeparamref name="TValue" /></param>
        /// <returns>
        /// A new <see cref="Writer{TArgument, TResultNew}" /> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="obj" /> or <paramref name="function" /> was <c>null</c>.</exception>
        public static Writer<TCollection, TValueNew> Bind<TCollection, TValue, TValueNew>(this Writer<TCollection, TValue> obj, Func<TValue, Writer<TCollection, TValueNew>> function)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (function == null) throw new ArgumentNullException(nameof(function));

            return () =>
            {
                var original = obj();
                var mapped = function(original.Value)();
                return new WriterEntry<TCollection, TValueNew>(mapped.Value, original.Elements.Concat(mapped.Elements));
            };
        }

        /// <summary>
        /// For LINQ support.
        /// </summary>
        /// <typeparam name="TCollection">The type of the elements to be written.</typeparam>
        /// <typeparam name="TValue">The type of the value of the first writer.</typeparam>
        /// <typeparam name="TSelect">The type of the value of the second writer.</typeparam>
        /// <typeparam name="TValueNew">The type of the new value.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="functionToBind">A function, selecting a new Writer from the value of <paramref name="obj" /></param>
        /// <param name="resultSelector">A function, selecting the result of the values of the two writer monad objects.</param>
        /// <returns>
        /// A new <see cref="Writer{TCollection, TValueNew}" /> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="obj" /> or <paramref name="resultSelector" /> was <c>null</c>.</exception>
        public static Writer<TCollection, TValueNew> SelectMany<TCollection, TValue, TSelect, TValueNew>(this Writer<TCollection, TValue> obj, Func<TValue, Writer<TCollection, TSelect>> functionToBind, Func<TValue, TSelect, TValueNew> resultSelector)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (functionToBind == null) throw new ArgumentNullException(nameof(functionToBind));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            return () =>
            {
                var original = obj();
                var bound = functionToBind(original.Value)();
                var resultValue = resultSelector(original.Value, bound.Value);

                return new WriterEntry<TCollection, TValueNew>(resultValue, original.Elements.Concat(bound.Elements));
            };
        }
    }
}