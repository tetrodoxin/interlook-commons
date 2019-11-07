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
    /// Either-monad with deferred execution.
    /// </summary>
    /// <typeparam name="TLeft">Data type of 'Left'</typeparam>
    /// <typeparam name="TRight">Data type of 'Right'</typeparam>
    /// <returns></returns>
    public delegate Either<TLeft, TRight> EitherLazy<TLeft, TRight>();

    public static class EitherLazy<TLeft>
    {
        public static EitherLazy<TLeft, TRight> Right<TRight>(Func<TRight> valueFactory) => EitherLazy.Right<TLeft, TRight>(valueFactory);

        public static EitherLazy<TLeft, TRight> Return<TRight>(Func<TRight> valueFactory) => Right(valueFactory);

        public static EitherLazy<TLeft, TRight> Left<TRight>(Func<TLeft> valueFactory) => EitherLazy.Left<TLeft, TRight>(valueFactory);
    }

    public static class EitherLazy
    {
        public static EitherLazy<TLeft, TRight> Left<TLeft, TRight>(Func<TLeft> valueFactory) => () => Either.Left<TLeft, TRight>(valueFactory());

        public static EitherLazy<TLeft, TRight> Left<TLeft, TRight>(TLeft value) => () => Either.Left<TLeft, TRight>(value);

        public static EitherLazy<TLeft, TRight> Right<TLeft, TRight>(Func<TRight> valueFactory) => () => Either.Right<TLeft, TRight>(valueFactory());

        public static EitherLazy<TLeft, TRight> Right<TLeft, TRight>(TRight value) => () => Either.Right<TLeft, TRight>(value);

        public static EitherLazy<TLeft, TRight> Create<TLeft, TRight>(Func<bool> predicate, Func<TRight> rightValueFactory, Func<TLeft> leftValueFactory)
            =>  () =>
                {
                    return predicate()
                    ? new Right<TLeft, TRight>(rightValueFactory())
                    : (Either<TLeft, TRight>)new Left<TLeft, TRight>(leftValueFactory());
                };

        /// <summary>
        /// Evaluates the either monad and returns if it
        /// is in the left-state
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        public static bool IsLeft<L, R>(this EitherLazy<L, R> either)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));

            return either().IsLeft;
        }

        /// <summary>
        /// Evaluates the either monad and returns if it
        /// is in the right-state
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
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
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        public static R GetRight<L, R>(this EitherLazy<L, R> either)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));

            return either().GetRight();
        }

        public static EitherLazy<L, R2> Bind<L, R1, R2>(this EitherLazy<L, R1> either, Func<R1, Either<L, R2>> functionToBind)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (functionToBind == null) throw new ArgumentNullException(nameof(functionToBind));

            return () => either().IsLeft ? new Left<L, R2>(either().GetLeft()) : functionToBind(either().GetRight());
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        public static EitherLazy<TLeft, TRight> Memoize<TLeft, TRight>(this EitherLazy<TLeft, TRight> either)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));

            var result = new Lazy<Either<TLeft, TRight>>(() => either());

            return () => result.Value;
        }
    }
}