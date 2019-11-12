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

namespace Interlook.Functional
{
    /// <summary>
    /// Defines helper methods for functional programming
    /// </summary>
    public static class FunctionalExtensions
    {
        /// <summary>
        /// Creates a function, returning the value/object.
        /// </summary>
        /// <typeparam name="T">Type of the value/object.</typeparam>
        /// <param name="value">A value.</param>
        /// <returns>A function, that returns the specified value.</returns>
        public static Func<T> AsFactoryFunc<T>(this T value) => () => value;

        /// <summary>
        /// Swaps the order of the arguments of a function with two arguments.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument.</typeparam>
        /// <typeparam name="T2">Type of the second argument.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="function">A function, whose arguments are to be reversed in order.</param>
        /// <returns>A new function, executing the original function with reversed arguments.</returns>
        public static Func<T2, T1, TResult> Flip<T1, T2, TResult>(this Func<T1, T2, TResult> function) => (a, b) => function(b, a);

        /// <summary>
        /// Swaps the order of the arguments of an action with two arguments.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument.</typeparam>
        /// <typeparam name="T2">Type of the second argument.</typeparam>
        /// <param name="action">An action, whose arguments are to be reversed in order.</param>
        /// <returns>A new action, executing the original action with reversed arguments.</returns>
        public static Action<T2, T1> Flip<T1, T2>(this Action<T1, T2> action) => (a, b) => action(b, a);

        /// <summary>
        /// Composition/chaining of two functions.
        /// </summary>
        /// <typeparam name="T1">Type of the input of the first function.</typeparam>
        /// <typeparam name="T2">Type of the result of the first function and the input of the second function.</typeparam>
        /// <typeparam name="T3">Type of the result of the second function.</typeparam>
        /// <param name="f">The first function.</param>
        /// <param name="g">The second function.</param>
        /// <returns></returns>
        public static Func<T1, T3> Compose<T1, T2, T3>(Func<T1, T2> f, Func<T2, T3> g) => (T1 a) => g(f(a));

        /// <summary>
        /// Creates a function, calling the given function
        /// with a specified value.
        /// </summary>
        /// <typeparam name="T1">Type of the function's parameter.</typeparam>
        /// <typeparam name="TResult">Type of the function's result.</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="arg">The argument-value for the function.</param>
        /// <returns>A new function with reduced arity.</returns>
        public static Func<TResult> LazyApply<T1, TResult>(this Func<T1, TResult> function, T1 arg) => () => function(arg);

        /// <summary>
        /// Assigns a value to the first argument of the function
        /// and returns a new function with only one argument
        /// (originally the second one), thus reduced arity.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument.</typeparam>
        /// <typeparam name="T2">Type of the second argument</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="arg">The argument-value to bind.</param>
        /// <returns>A new function with reduced arity.</returns>
        public static Func<T2, TResult> PartialApply<T1, T2, TResult>(this Func<T1, T2, TResult> function, T1 arg) => (T2 b) => function(arg, b);


        /// <summary>
        /// Assigns a value to the first argument of the function
        /// and returns a new function with the remaining arguments.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument.</typeparam>
        /// <typeparam name="T2">Type of the second argument</typeparam>
        /// <typeparam name="T3">Type of the third argument</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="arg">The argument-value to bind.</param>
        /// <returns>A new function with reduced arity.</returns>
        public static Func<T2, T3, TResult> PartialApply<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> function, T1 arg) 
            => (T2 b, T3 c) => function(arg, b, c);

        /// <summary>
        /// Assigns a value to the first argument of the function
        /// and returns a new function with the remaining arguments.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument.</typeparam>
        /// <typeparam name="T2">Type of the second argument</typeparam>
        /// <typeparam name="T3">Type of the third argument</typeparam>
        /// <typeparam name="T4">Type of the third argument</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="arg">The argument-value to bind.</param>
        /// <returns>A new function with reduced arity.</returns>
        public static Func<T2, T3, T4, TResult> PartialApply<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> function, T1 arg)
            => (T2 b, T3 c, T4 d) => function(arg, b, c, d);


        /// <summary>
        /// Assigns a value to the first argument of the function
        /// and returns a new function with the remaining arguments.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument.</typeparam>
        /// <typeparam name="T2">Type of the second argument</typeparam>
        /// <typeparam name="T3">Type of the third argument</typeparam>
        /// <typeparam name="T4">Type of the third argument</typeparam>
        /// <typeparam name="T5">Type of the third argument</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="arg">The argument-value to bind.</param>
        /// <returns>A new function with reduced arity.</returns>
        public static Func<T2, T3, T4, T5, TResult> PartialApply<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> function, T1 arg)
            => (T2 b, T3 c, T4 d, T5 e) => function(arg, b, c, d, e);

        /// <summary>
        /// Returns a curry function of the function,
        /// meaning a function with the first original argument,
        /// returning a function for the second original argument.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument.</typeparam>
        /// <typeparam name="T2">Type of the second argument</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>The curry function.</returns>
        /// <example>
        /// // principle of currying in C# functions
        /// Func&lt;int, int, int&gt; f = (x, y, n) => (x + y) * n;
        /// var curried = f.Curry();
        /// 
        /// var result1 = f(3, 2, 4);       // returns 20
        /// var result2 = curry(3)(2)(4)
        /// </example>
        public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(this Func<T1, T2, TResult> function)
            => (T1 a) => (T2 b) => function(a, b);

        /// <summary>
        /// Returns a curry function of the function,
        /// meaning a function with the first original argument,
        /// returning a function for the second original argument.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument.</typeparam>
        /// <typeparam name="T2">Type of the second argument</typeparam>
        /// <typeparam name="T3">Type of the third argument</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>The curry function.</returns>
        /// <example>
        /// // principle of currying in C# functions
        /// Func&lt;int, int, int&gt; f = (x, y, n) => (x + y) * n;
        /// var curried = f.Curry();
        /// 
        /// var result1 = f(3, 2, 4);       // returns 20
        /// var result2 = curry(3)(2)(4)
        /// </example>
        public static Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> function)
            => (T1 a) => (T2 b) => (T3 c) => function(a, b, c);

        /// <summary>
        /// Returns a curry function of the function,
        /// meaning a function with the first original argument,
        /// returning a function for the second original argument.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument.</typeparam>
        /// <typeparam name="T2">Type of the second argument</typeparam>
        /// <typeparam name="T3">Type of the third argument</typeparam>
        /// <typeparam name="T4">Type of the third argument</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>The curry function.</returns>
        /// <example>
        /// // principle of currying in C# functions
        /// Func&lt;int, int, int&gt; f = (x, y, n) => (x + y) * n;
        /// var curried = f.Curry();
        /// 
        /// var result1 = f(3, 2, 4);       // returns 20
        /// var result2 = curry(3)(2)(4)
        /// </example>
        public static Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> Curry<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> function)
            => (T1 a) => (T2 b) => (T3 c) => (T4 d) => function(a, b, c, d);

        /// <summary>
        /// Returns a curry function of the function,
        /// meaning a function with the first original argument,
        /// returning a function for the second original argument.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument.</typeparam>
        /// <typeparam name="T2">Type of the second argument</typeparam>
        /// <typeparam name="T3">Type of the third argument</typeparam>
        /// <typeparam name="T4">Type of the third argument</typeparam>
        /// <typeparam name="T5">Type of the third argument</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>The curry function.</returns>
        /// <example>
        /// // principle of currying in C# functions
        /// Func&lt;int, int, int&gt; f = (x, y, n) => (x + y) * n;
        /// var curried = f.Curry();
        /// 
        /// var result1 = f(3, 2, 4);       // returns 20
        /// var result2 = curry(3)(2)(4)
        /// </example>
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, TResult>>>>> Curry<T1, T2, T3, T4, T5,TResult>(this Func<T1, T2, T3, T4, T5, TResult> function)
            => (T1 a) => (T2 b) => (T3 c) => (T4 d) => (T5 e) => function(a, b, c, d, e);

    }
}
