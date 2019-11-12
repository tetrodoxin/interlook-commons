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

namespace Interlook.Monads
{
    /// <summary>
    /// Represents a monadic reader.
    /// This monad is innately lazy.
    /// </summary>
    /// <typeparam name="TArgument">The type of the argument.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class Reader<TArgument, TResult>
    {
        private Func<TArgument, TResult> _func;

        /// <summary>
        /// Initializes a new instance of the <see cref="Reader{TArgument, TResult}"/> class.
        /// </summary>
        /// <param name="function">The function to be encapsluated.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="function"/> was <c>null</c>.</exception>
        public Reader(Func<TArgument, TResult> function)
        {
            _func = function ?? throw new ArgumentNullException(nameof(function));
        }

        /// <summary>
        /// Maps a reader to a new reader.
        /// </summary>
        /// <typeparam name="TResultNew">The type of the result new.</typeparam>
        /// <param name="function">The function, that selects a type <typeparamref name="TResultNew"/>
        /// from type <typeparamref name="TResult"/></param>
        /// <returns>A new <see cref="Reader{TArgument, TResultNew}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="function" /> was <c>null</c>.</exception>
        public Reader<TArgument, TResultNew> Map<TResultNew>(Func<TResult, TResultNew> function)
        {
            if (function == null) throw new ArgumentNullException(nameof(function));

            return new Reader<TArgument, TResultNew>((TArgument arg) => function(GetResult(arg)));
        }

        /// <summary>
        /// Binds a function to a reader.
        /// </summary>
        /// <typeparam name="TResultNew">The type of the result new.</typeparam>
        /// <param name="function">The function, that selects a type <typeparamref name="TResultNew"/>
        /// from type <typeparamref name="TResult"/></param>
        /// <returns>A new <see cref="Reader{TArgument, TResultNew}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="function" /> was <c>null</c>.</exception>
        public Reader<TArgument, TResultNew> Bind<TResultNew>(Func<TResult, Reader<TArgument, TResultNew>> function)
        {
            if (function == null) throw new ArgumentNullException(nameof(function));

            return new Reader<TArgument, TResultNew>(arg => function(GetResult(arg)).GetResult(arg));
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <param name="arg">The argument.</param>
        public TResult GetResult(TArgument arg) => _func(arg);

        /// <summary>
        /// Lazily evaluates the reader.
        /// </summary>
        /// <param name="arg">The argument.</param>
        public Func<TResult> GetResultLazy(TArgument arg)
        {
            // not inside lambda-expression, otherwise every invocation
            // would create a new lazy instance
            var result = new Lazy<TResult>(() => GetResult(arg));

            return () => result.Value;
        }
    }

    /// <summary>
    /// Contains static extension classes for <see cref="Reader{TArgument, TResult}"/>
    /// </summary>
    public static class Reader
    {
        /// <summary>
        /// Creates a new reader instance for the specified argument type,
        /// which accepts and returns a value of this very type.
        /// </summary>
        /// <typeparam name="TArgument">The argument type.</typeparam>
        /// <returns>A new reader instance which returns solely the object it was given.</returns>
        public static Reader<TArgument, TArgument> Ask<TArgument>() => new Reader<TArgument, TArgument>((TArgument env) => env);

        /// <summary>
        /// Creates a new reader instance for the specified argument type,
        /// which accepts and returns a value of this very type.
        /// </summary>
        /// <typeparam name="TArgument">The argument type.</typeparam>
        /// <returns>A new reader instance which returns the provided object
        /// after it was processed by <paramref name="func"/>.</returns>
        public static Reader<TArgument, TArgument> Ask<TArgument>(Func<TArgument, TArgument> func)
            => new Reader<TArgument, TArgument>((TArgument env) => func(env));

        /// <summary>
        /// Returns a new reader instance that always returns a specified value,
        /// independent of the provided argument.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="value">The value to return. Defaults to <c>default&lt;TResult&gt;</c>, if not specified.</param>
        /// <returns>A new reader with constant return value.</returns>
        public static Reader<TArgument, TResult> ReturnReader<TArgument, TResult>(this TResult value)
            => new Reader<TArgument, TResult>(arg => value);

        /// <summary>
        /// Returns a new reader instance that always returns the default of a specified type,
        /// independent of the provided argument.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>A new reader with constant return value.</returns>
        public static Reader<TArgument, TResult> ReturnReader<TArgument, TResult>()
            => new Reader<TArgument, TResult>(arg => default);

        /// <summary>
        /// For LINQ-Query-Support
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <typeparam name="TResult">The type of the result of the specified reader</typeparam>
        /// <typeparam name="TCollector">The result-type of the reader returned by <paramref name="collector"/>.</typeparam>
        /// <typeparam name="TResultNew">The result-type, returned by <paramref name="selector"/>.</typeparam>
        /// <param name="obj">A reader object.</param>
        /// <param name="collector">A function, selecting another reader object from the result of <paramref name="obj"/>.</param>
        /// <param name="selector">A function, selecting the final result from the results of
        ///     <paramref name="obj"/> and the reader returned by <paramref name="collector"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="obj"/>, <paramref name="collector"/> or <paramref name="selector"/> was <c>null</c>.
        /// </exception>
        public static Reader<TArgument, TResultNew> SelectMany<TArgument, TResult, TCollector, TResultNew>(this Reader<TArgument, TResult> obj, Func<TResult, Reader<TArgument, TCollector>> collector, Func<TResult, TCollector, TResultNew> selector)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return new Reader<TArgument, TResultNew>(arg => selector(obj.GetResult(arg), collector(obj.GetResult(arg)).GetResult(arg)));
        }

        /// <summary>
        /// For LINQ support. Just calls <see cref="Reader{TArgument, TResult}.Map{TResultNew}(Func{TResult, TResultNew})"/>
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <typeparam name="TResult">The type of the result of the specified reader</typeparam>
        /// <typeparam name="TResultNew">The type of the result new.</typeparam>
        /// <param name="obj">A reader object.</param>
        /// <param name="function">The function, that selects a type <typeparamref name="TResultNew"/>
        /// from type <typeparamref name="TResult"/></param>
        /// <returns>A new <see cref="Reader{TArgument, TResultNew}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="obj"/> or <paramref name="function"/> was <c>null</c>.
        /// </exception>
        public static Reader<TArgument, TResultNew> Select<TArgument, TResult, TResultNew>(this Reader<TArgument, TResult> obj, Func<TResult, TResultNew> function)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (function == null) throw new ArgumentNullException(nameof(function));

            return obj.Map(function);
        }
    }
}