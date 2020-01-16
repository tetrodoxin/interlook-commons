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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Interlook.Monads
{
    /// <summary>
    /// Either-monad with deferred execution.
    /// </summary>
    /// <typeparam name="TLeft">Data type of 'Left'</typeparam>
    /// <typeparam name="TRight">Data type of 'Right'</typeparam>
    /// <returns></returns>
    public delegate Either<TLeft, TRight> EitherLazy<TLeft, TRight>();

    /// <summary>
    /// Class with static helper/factory methods
    /// </summary>
    /// <typeparam name="TLeft">The left type.</typeparam>
    public static class EitherLazy<TLeft>
    {
        /// <summary>
        /// Factory method for a lazy either instance with a right value
        /// </summary>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="valueFactory">Factory method for the right value.</param>
        public static EitherLazy<TLeft, TRight> Right<TRight>(Func<TRight> valueFactory) => EitherLazy.Right<TLeft, TRight>(valueFactory);

        /// <summary>
        /// Creates a lazy either instance with a right value.
        /// </summary>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="valueFactory">Factory method for the right value.</param>
        public static EitherLazy<TLeft, TRight> Return<TRight>(Func<TRight> valueFactory) => Right(valueFactory);

        /// <summary>
        /// Factory method for a lazy either instance with a left value
        /// </summary>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="valueFactory">Factory method for the left value.</param>
        public static EitherLazy<TLeft, TRight> Left<TRight>(Func<TLeft> valueFactory) => EitherLazy.Left<TLeft, TRight>(valueFactory);
    }

    /// <summary>
    /// Class with static extension and factory methods
    /// </summary>
    public static class EitherLazy
    {
        /// <summary>
        /// Factory method for a lazy either instance with a left value
        /// </summary>
        /// <typeparam name="TLeft">The left type.</typeparam>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="valueFactory">Factory method for the left value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="valueFactory"/> was <c>null</c>.</exception>
        public static EitherLazy<TLeft, TRight> Left<TLeft, TRight>(Func<TLeft> valueFactory)
        {
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            return () => Either.Left<TLeft, TRight>(valueFactory());
        }

        /// <summary>
        /// Factory method for a lazy either instance with a left value
        /// </summary>
        /// <typeparam name="TLeft">The left type.</typeparam>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="value">The left value</param>
        public static EitherLazy<TLeft, TRight> Left<TLeft, TRight>(TLeft value) => () => Either.Left<TLeft, TRight>(value);

        /// <summary>
        /// Factory method for a lazy either instance with a right value
        /// </summary>
        /// <typeparam name="TLeft">The left type.</typeparam>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="valueFactory">Factory method for the right value.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="valueFactory"/> was <c>null</c>.</exception>
        public static EitherLazy<TLeft, TRight> Right<TLeft, TRight>(Func<TRight> valueFactory)
        {
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            return () => Either.Right<TLeft, TRight>(valueFactory());
        }

        /// <summary>
        /// Factory method for a lazy either instance with a right value
        /// </summary>
        /// <typeparam name="TLeft">The left type.</typeparam>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="value">The right value</param>
        public static EitherLazy<TLeft, TRight> Right<TLeft, TRight>(TRight value) => () => Either.Right<TLeft, TRight>(value);

        /// <summary>
        /// Factory method for a lazy either instance, whose state is
        /// determined by a predicate function
        /// </summary>
        /// <typeparam name="TLeft">The left type.</typeparam>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="predicate">The predicate function, which determines,
        /// whether the instance will be in left or right state and thus
        /// which factory method will be used.</param>
        /// <param name="rightValueFactory">Factory method for the right value.</param>
        /// <param name="leftValueFactory">Factory method for the left value.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="predicate"/>, <paramref name="rightValueFactory"/> 
        /// or <paramref name="leftValueFactory"/> was <c>null</c>
        /// </exception>
        public static EitherLazy<TLeft, TRight> Create<TLeft, TRight>(Func<bool> predicate, Func<TRight> rightValueFactory, Func<TLeft> leftValueFactory)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (rightValueFactory == null) throw new ArgumentNullException(nameof(rightValueFactory));
            if (leftValueFactory == null) throw new ArgumentNullException(nameof(leftValueFactory));

            return () =>
                            {
                                return predicate()
                                ? new Right<TLeft, TRight>(rightValueFactory())
                                : (Either<TLeft, TRight>)new Left<TLeft, TRight>(leftValueFactory());
                            };
        }

        /// <summary>
        /// Evaluates the either monad and returns if it
        /// is in the left-state
        /// </summary>
        /// <typeparam name="L">The left data type</typeparam>
        /// <typeparam name="R">The right data type</typeparam>
        /// <param name="either">A lazy either instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified either is left; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">either was <c>null</c></exception>
        public static bool IsLeft<L, R>(this EitherLazy<L, R> either)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));

            return either().IsLeft;
        }

        /// <summary>
        /// Evaluates the either monad and returns if it
        /// is in the right-state
        /// </summary>
        /// <typeparam name="L">The left data type</typeparam>
        /// <typeparam name="R">The right data type</typeparam>
        /// <param name="either">A lazy either instance.</param>
        public static bool IsRight<L, R>(this EitherLazy<L, R> either)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));

            return either().IsRight;
        }

        /// <summary>
        /// Evaluates the either monad and returns the left value.
        /// Will throw an exception, if the encapsulated either\
        /// is not in left-state.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        public static TLeft GetLeft<TLeft, TRight>(this EitherLazy<TLeft, TRight> either)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));

            return either().GetLeft();
        }

        /// <summary>
        /// Evaluates the either monad and returns the right value.
        /// Will throw an exception, if the encapsulated either\
        /// is not in right-state.
        /// </summary>
        /// <typeparam name="TLeft">The type of the left.</typeparam>
        /// <typeparam name="TRight">The type of the right.</typeparam>
        /// <param name="either">The either.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="either"/> was <c>null</c>.</exception>
        public static TRight GetRight<TLeft, TRight>(this EitherLazy<TLeft, TRight> either)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));

            return either().GetRight();
        }

        /// <summary>
        /// Binds the specified function.
        /// </summary>
        /// <typeparam name="TLeft">The left data type</typeparam>
        /// <typeparam name="TRightSource">The right type of the specified either.</typeparam>
        /// <typeparam name="TRightResult">The right type of the resulting either instance.</typeparam>
        /// <param name="either">First either instance.</param>
        /// <param name="functionToBind">The function to bind.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="either"/> or <paramref name="functionToBind"/> was <c>null</c>.
        /// </exception>
        public static EitherLazy<TLeft, TRightResult> Bind<TLeft, TRightSource, TRightResult>(this EitherLazy<TLeft, TRightSource> either, Func<TRightSource, Either<TLeft, TRightResult>> functionToBind)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (functionToBind == null) throw new ArgumentNullException(nameof(functionToBind));

            return () => either().IsLeft ? new Left<TLeft, TRightResult>(either().GetLeft()) : functionToBind(either().GetRight());
        }

        /// <summary>
        /// Binds the specified function.
        /// </summary>
        /// <typeparam name="TLeft">The left data type</typeparam>
        /// <typeparam name="TRightSource">The right type of the specified either.</typeparam>
        /// <typeparam name="TRightResult">The right type of the resulting either instance.</typeparam>
        /// <param name="either">First either instance.</param>
        /// <param name="functionToBind">The function to bind.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="either"/> or <paramref name="functionToBind"/> was <c>null</c>.
        /// </exception>
        public static EitherLazy<TLeft, TRightResult> Bind<TLeft, TRightSource, TRightResult>(this EitherLazy<TLeft, TRightSource> either, Func<TRightSource, EitherLazy<TLeft, TRightResult>> functionToBind)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (functionToBind == null) throw new ArgumentNullException(nameof(functionToBind));

            return () => either().IsLeft ? new Left<TLeft, TRightResult>(either().GetLeft()) : functionToBind(either().GetRight())();
        }

        /// <summary>
        /// Switches to the left (failed) state under conditions, defined by a function.
        /// If this function results in <c>false</c>, nothing is altered.
        /// </summary>
        /// <typeparam name="TLeft">Left data type (Error)</typeparam>
        /// <typeparam name="TRight">Right data type (Result)</typeparam>
        /// <param name="either">Either-object</param>
        /// <param name="errorCondition">Error condition.</param>
        /// <param name="errorValue">Error value, assigned to left state if function fails.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="either"/>, <paramref name="errorCondition"/> or <paramref name="errorValue"/> was <c>null</c>.
        /// </exception>
        public static EitherLazy<TLeft, TRight> FailIf<TLeft, TRight>(this EitherLazy<TLeft, TRight> either, Func<TRight, bool> errorCondition, TLeft errorValue)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (errorCondition == null) throw new ArgumentNullException(nameof(errorCondition));

            return () => either().IsRight && errorCondition(either.GetRight()) ? new Left<TLeft, TRight>(errorValue) : either();
        }

        /// <summary>
        /// Just for LINQ-Query-Support
        /// </summary>
        /// <typeparam name="L"></typeparam>
        /// <typeparam name="R1">The type of the 1.</typeparam>
        /// <typeparam name="R2">The type of the 2.</typeparam>
        /// <typeparam name="TR">The type of the r.</typeparam>
        /// <param name="either">a.</param>
        /// <param name="selector">The function.</param>
        /// <param name="mapper">The select.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="either"/>, <paramref name="selector"/> or <paramref name="mapper"/> was <c>null</c>.
        /// </exception>
        public static EitherLazy<L, TR> SelectMany<L, R1, R2, TR>(this EitherLazy<L, R1> either, Func<R1, EitherLazy<L, R2>> selector, Func<R1, R2, TR> mapper)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            return () => Either.Bind(either(), aValue => Either.Bind(selector(aValue)(), bValue => Either.Right<L, TR>(mapper(aValue, bValue))));
        }

        /// <summary>
        /// Just for LINQ-Query-Support
        /// </summary>
        /// <typeparam name="L">The common left data type</typeparam>
        /// <typeparam name="R1">The type of the 1.</typeparam>
        /// <typeparam name="R2">The type of the 2.</typeparam>
        /// <param name="either">The either.</param>
        /// <param name="selector">The selector.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="either"/> or <paramref name="selector"/> was <c>null</c>.
        /// </exception>
        public static EitherLazy<L, R2> Select<L, R1, R2>(this EitherLazy<L, R1> either, Func<R1, R2> selector)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return () => either().IsLeft ? new Left<L, R2>(either().GetLeft()) : (Either<L, R2>)new Right<L, R2>(selector(either().GetRight()));
        }

        /// <summary>
        /// Returns an enumerator that eventually will
        /// evaluate an enumerator of lazy either monads
        /// and only containing those, encapsulating
        /// either monads in the left-state.
        /// </summary>
        /// <typeparam name="TLeft">Left data type.</typeparam>
        /// <typeparam name="TRight">Right data type.</typeparam>
        /// <param name="source">Source enumerator of lazy either monads.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/>was <c>null</c>.</exception>
        public static IEnumerable<TLeft> Lefts<TLeft, TRight>(this IEnumerable<EitherLazy<TLeft, TRight>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source
                           .Select(p => p())
                           .Where(p => p.IsLeft)
                           .Select(p => p.GetLeft());
        }

        /// <summary>
        /// Returns an enumerator that eventually will
        /// evaluate an enumerator of lazy either monads
        /// and only containing those, encapsulating
        /// either monads in the right-state.
        /// </summary>
        /// <typeparam name="TLeft">Left data type.</typeparam>
        /// <typeparam name="TRight">Right data type.</typeparam>
        /// <param name="source">Source enumerator of lazy either monads.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/>was <c>null</c>.</exception>
        public static IEnumerable<TRight> Rights<TLeft, TRight>(this IEnumerable<EitherLazy<TLeft, TRight>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source
                           .Select(p => p())
                           .Where(p => p.IsRight)
                           .Select(p => p.GetRight());
        }

        /// <summary>
        /// Returns two enumerators, which eventually will
        /// evaluate an enumerator of lazy either monads,
        /// one only containing those, encapsulating
        /// either monads in the left-state and the other
        /// one thos in the right-state.
        /// </summary>
        /// <typeparam name="TLeft">Left data type.</typeparam>
        /// <typeparam name="TRight">Right data type.</typeparam>
        /// <param name="source">Source enumerator of lazy either monads.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/>was <c>null</c>.</exception>
        public static (IEnumerable<TLeft> Left, IEnumerable<TRight> Right) PartitionEithers<TLeft, TRight>(this IEnumerable<EitherLazy<TLeft, TRight>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var src = source.Select(p => p());

            return (
                src.Where(p => p.IsLeft).Select(p => p.GetLeft()),
                src.Where(p => p.IsRight).Select(p => p.GetRight()));
        }

        /// <summary>
        /// Instantly evaluates the lazy either with one of two given mapping functions.
        /// </summary>
        /// <typeparam name="TLeft">Left data type.</typeparam>
        /// <typeparam name="TRight">Right data type.</typeparam>
        /// <typeparam name="TResult">Right .</typeparam>
        /// <param name="either">Lazy either monad.</param>
        /// <param name="leftFunction">Function to be used for left state.</param>
        /// <param name="rightFunction">Function to be used for right state.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="either"/>, 
        /// <paramref name="leftFunction"/> or <paramref name="rightFunction"/> was <c>null</c>.</exception>
        public static TResult MapEither<TLeft, TRight, TResult>(this EitherLazy<TLeft, TRight> either, Func<TLeft, TResult> leftFunction, Func<TRight, TResult> rightFunction)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (leftFunction == null) throw new ArgumentNullException(nameof(leftFunction));
            if (rightFunction == null) throw new ArgumentNullException(nameof(rightFunction));

            var result = either();
            return result.IsLeft ? leftFunction(result.GetLeft()) : rightFunction(result.GetRight());
        }

        /// <summary>
        /// Returns an <see cref="EitherLazy{L, R}" />-instance,
        /// encapsulating all underlying evaluations, thus ensuring
        /// they are executed only once.
        /// </summary>
        /// <typeparam name="TLeft">Left data type.</typeparam>
        /// <typeparam name="TRight">Right data type.</typeparam>
        /// <param name="either">Lazy either monad.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="either"/>was <c>null</c>.</exception>
        public static EitherLazy<TLeft, TRight> Memoize<TLeft, TRight>(this EitherLazy<TLeft, TRight> either)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));

            // not inside lambda-expression, otherwise every invocation
            // would create a new lazy instance
            var result = new Lazy<Either<TLeft, TRight>>(() => either());

            return () => result.Value;
        }
    }
}