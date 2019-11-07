﻿using Interlook.Monads;
using System;

namespace Interlook.Monads
{
    /// <summary>
    /// Implementation of the Try-Monad,
    /// which encapsulates a method/function-call.
    /// </summary>
    /// <typeparam name="T">Encapsulated data type.</typeparam>
    /// <returns></returns>
    public delegate Try<T> TryLazy<T>();

    /// <summary>
    /// Class with factory methods for <see cref="Try{T}"/>-Delegates
    /// </summary>
    public static class TryLazy
    {
        /// <summary>
        /// <para>
        /// Creates a <see cref="Try{T}"/>-instance, encapsulating a method invocation,
        /// that either returns a value of <c>TResult</c> or
        /// throws an exception.
        /// </para>
        /// The method call itself will yet not be executed.
        /// </summary>
        /// <typeparam name="TResult">Data type of the result value.</typeparam>
        /// <param name="func">The method call to execute.</param>
        /// <returns>A <see cref="Try{T}"/>-instance.</returns>
        public static TryLazy<TResult> WrapCall<TResult>(Func<TResult> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            return () =>
            {
                try
                {
                    return new Success<TResult>(func());
                }
                catch (Exception ex)
                {
                    return new Failure<TResult>(ex);
                }
            };
        }

        /// <summary>
        /// Safely executes the encapsulated method call.
        /// </summary>
        /// <typeparam name="TResult">Data type of the return value.</typeparam>
        /// <param name="obj">The <see cref="Try{T}"/>-monad, whose encapsulated call
        /// is to be executed.</param>
        /// <returns></returns>
        public static Try<TResult> Invoke<TResult>(this TryLazy<TResult> obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            try
            {
                return obj();
            }
            catch (Exception ex)
            {
                return new Failure<TResult>(ex);
            }
        }

        /// <summary>
        /// <para>
        /// Returns an <see cref="TryLazy"/>-monad encapsulating
        /// all underlying lazy method calls, thus caching
        /// the result in a new lazy monad.
        /// </para>
        /// No instant evaluation or exceptions will occur.
        /// </summary>
        /// <typeparam name="TResult">Data type of the return value.</typeparam>
        /// <param name="obj">The <see cref="TryLazy{T}"/>-monad, whose encapsulated call
        /// is to be executed.</param>
        /// <returns></returns>
        public static TryLazy<TResult> Memoize<TResult>(this TryLazy<TResult> obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            try
            {
                var result = obj();
                return () => result;
            }
            catch (Exception e)
            {
                return () => new Failure<TResult>(e);
            }
        }

        /// <summary>
        /// For LINQ support
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">A <see cref="TryLazy{T}"/>-monad.</param>
        /// <param name="selector">Selector function, that shall be applied to the return value of the method call.</param>
        /// <returns>A <see cref="TryLazy{T}"/>-monad, encapsulating the chained (still unexecuted) calls of both,
        /// the original method call and the selector function.</returns>
        public static TryLazy<TResult> Select<TSource, TResult>(this TryLazy<TSource> obj, Func<TSource, TResult> selector)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return Then(obj, selector);
        }

        /// <summary>
        /// For LINQ Query Syntax support with multiple FROM clauses.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="obj">A <see cref="TryLazy{T}"/>-monad.</param>
        /// <param name="func">A function to be bound in an additional <see cref="TryLazy{T}"/>-monad.</param>
        /// <param name="selector">A selector function for the result of both functions.</param>
        /// <returns></returns>
        public static TryLazy<TResult> SelectMany<T1, T2, TResult>(this TryLazy<T1> obj, Func<T1, TryLazy<T2>> func, Func<T1, T2, TResult> selector)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return () => obj().SelectMany(p => func(p)(), selector);
        }

        /// <summary>
        /// Chains two method calls.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">A <see cref="TryLazy{T}"/>-monad.</param>
        /// <param name="selector">Selector function, that shall be applied to
        /// the return value of the encapsulated method call.</param>
        /// <returns>
        /// A new <see cref="TryLazy{T}"/>-monad, encapsulating the chained, but still
        /// not executed, calls of the monad's method and the selector function.
        /// </returns>
        public static TryLazy<TResult> Then<TSource, TResult>(this TryLazy<TSource> obj, Func<TSource, TResult> selector)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return () => obj().Then(selector);
        }

        /// <summary>
        /// Similar to <see cref="SelectMany{T1, T2, TResult}(TryLazy{T1}, Func{T1, TryLazy{T2}}, Func{T1, T2, TResult})"/>.
        /// Safely invokes the method calls of the current and the provided monad (in that order)
        /// and provides the return values of both to a combiner function. The result of
        /// that function will be returned.
        /// </summary>
        /// <typeparam name="T">Data type of the return value of both monads.</typeparam>
        /// <param name="obj">A <see cref="TryLazy{T}"/>-monad.</param>
        /// <param name="other">Another (the second) <see cref="TryLazy{T}"/>-monad.</param>
        /// <param name="combiner">A function, combining the return values of the first and the second monad, if both succeed.</param>
        /// <returns></returns>
        public static TryLazy<T> CombineWith<T>(this TryLazy<T> obj, TryLazy<T> other, Func<T, T, T> combiner)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (combiner == null) throw new ArgumentNullException(nameof(combiner));

            return () => obj().CombineWith(other(), combiner);
        }
    }
}