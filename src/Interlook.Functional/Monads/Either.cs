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

#endregion license

using System;
using System.Collections.Generic;
using System.Linq;

namespace Interlook.Monads
{
    /// <summary>
    /// Contains helper/extension methods for the either monad.
    /// </summary>
    public static class Either
    {
        /// <summary>
        /// Wraps an exception around an contained exception of
        /// an either-object, if in left state.
        /// </summary>
        /// <typeparam name="TRight">Right data type</typeparam>
        /// <param name="either">The either object</param>
        /// <param name="wrappingExceptionFactory">A function that, if the either is in left state,
        /// receives the contained exception and shall create a new exception around it,
        /// by using it as inner exception.</param>
        /// <returns>A <see cref="Left{Exception, TRight}"/> containing the new exception created by <paramref name="wrappingExceptionFactory"/>,
        /// if the object in <paramref name="either"/> was in left state; otherwise the original <see cref="Right{Exception, TRight}"/> of <paramref name="wrappingExceptionFactory"/></returns>
        public static Either<Exception, TRight> AddOuterException<TRight>(this Either<Exception, TRight> either, Func<Exception, Exception> wrappingExceptionFactory)
            => either.MapEither(
                ex => Left<Exception, TRight>(wrappingExceptionFactory(ex)),
                right => either);

        /// <summary>
        /// Applies an action on an <see cref="Right{Exception, TRight}"/>-instance.
        /// Nothing is done with an <see cref="Left{Exception, TRight}"/>-instance accordingly.
        /// </summary>
        /// <typeparam name="TRight">The right-datatype</typeparam>
        /// <param name="either">The either object.</param>
        /// <param name="action">The action to invoke with the value of the right-value of <paramref name="either"/>,
        /// if that is in right-state.</param>
        /// <returns>The original object provided in <paramref name="either"/> or a new instance of
        /// <see cref="Left{Exception, TRight}"/>, if an exception occured while executing <paramref name="action"/></returns>
        public static Either<Exception, TRight> ApplySafe<TRight>(this Either<Exception, TRight> either, Action<TRight> action)
            => either.Bind(right =>
            {
                try
                {
                    action(right);
                    return either;
                }
                catch (Exception ex)
                {
                    return Left<Exception, TRight>(ex);
                }
            });

        /// <summary>
        /// Binds the specified function.
        /// </summary>
        /// <typeparam name="TLeft">The left type.</typeparam>
        /// <typeparam name="TRightSource">The right type of the specified either.</typeparam>
        /// <typeparam name="TRightResult">The righttype of the result.</typeparam>
        /// <param name="either">The either.</param>
        /// <param name="functionToBind">The function to bind.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="either"/> or <paramref name="functionToBind"/> was <c>null</c>.
        /// </exception>
        public static Either<TLeft, TRightResult> Bind<TLeft, TRightSource, TRightResult>(this Either<TLeft, TRightSource> either, Func<TRightSource, Either<TLeft, TRightResult>> functionToBind)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (functionToBind == null) throw new ArgumentNullException(nameof(functionToBind));

            return either.BindInternal(functionToBind);
        }

        /// <summary>
        /// Creates an "exception Either" object
        /// containing a new <see cref="Exception"/> with a specified text and an inner exception.
        /// </summary>
        /// <typeparam name="TRight">The type of the right.</typeparam>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <returns>A new instance of <see cref="Left{Exception, TRight}"/> containing
        /// the newly created exception.</returns>
        public static Either<Exception, TRight> CreateFailed<TRight>(string exceptionMessage, Exception innerException)
            => Left<Exception, TRight>(new Exception(exceptionMessage, innerException));

        /// <summary>
        /// Creates an "exception Either" object
        /// containing a new <see cref="Exception"/> with a specified text.
        /// </summary>
        /// <typeparam name="TRight">The type of the right.</typeparam>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <returns>A new instance of <see cref="Left{Exception, TRight}"/> containing
        /// the newly created exception.</returns>
        public static Either<Exception, TRight> CreateFailed<TRight>(string exceptionMessage)
            => Left<Exception, TRight>(new Exception(exceptionMessage));

        /// <summary>
        /// Switches to the left (failed) state under conditions, defined by a function.
        /// If that function results in <c>false</c>, nothing is altered.
        /// </summary>
        /// <typeparam name="TLeft">Left data type (Error)</typeparam>
        /// <typeparam name="TRight">Right data type (Result)</typeparam>
        /// <param name="either">Either-object</param>
        /// <param name="errorCondition">Error condition.</param>
        /// <param name="errorValue">Error value, assigned to left state if function fails.</param>
        /// <returns>
        /// Possibly a new Either-instance with changed state.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="either"/> or <paramref name="errorCondition"/> was <c>null</c>.
        /// </exception>
        public static Either<TLeft, TRight> FailIf<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TRight, bool> errorCondition, TLeft errorValue)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (errorCondition == null) throw new ArgumentNullException(nameof(errorCondition));

            return either.Bind(value => errorCondition(value) ? Left<TLeft, TRight>(errorValue) : either);
        }

        /// <summary>
        /// Switches to the left (failed) state under conditions, defined by a function.
        /// If that function results in <c>false</c>, nothing is altered.
        /// </summary>
        /// <typeparam name="TLeft">Left data type (Error)</typeparam>
        /// <typeparam name="TRight">Right data type (Result)</typeparam>
        /// <param name="either">Either-object</param>
        /// <param name="errorCondition">Error condition.</param>
        /// <param name="errorValueFactory">A function, that returns the left (error) value.</param>
        /// <returns>
        /// Possibly a new Either-instance with changed state.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="either"/> or <paramref name="errorCondition"/> was <c>null</c>.
        /// </exception>
        public static Either<TLeft, TRight> FailIf<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TRight, bool> errorCondition, Func<TRight, TLeft> errorValueFactory)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (errorCondition == null) throw new ArgumentNullException(nameof(errorCondition));
            errorValueFactory ??= (p => default);

            return either.Bind(value => errorCondition(value) ? Left<TLeft, TRight>(errorValueFactory(value)) : either);
        }

        /// <summary>
        /// Gets the left-value of an either object in left-state.
        /// </summary>
        /// <typeparam name="TLeft">Left data type.</typeparam>
        /// <typeparam name="TRight">Right data type.</typeparam>
        /// <param name="either">The either object</param>
        /// <param name="defaultLeftValue">A default value that is used, if <paramref name="either"/> was not in left-state.</param>
        /// <returns>The left-value of <paramref name="either"/>, if it was in left-state; otherwiese <paramref name="defaultLeftValue"/></returns>
        public static TLeft GetLeft<TLeft, TRight>(this Either<TLeft, TRight> either, TLeft defaultLeftValue)
            => either.MapEither(left => left, _ => defaultLeftValue);

        /// <summary>
        /// Gets the right-value of an either object in right state.
        /// </summary>
        /// <typeparam name="TLeft">Left data type.</typeparam>
        /// <typeparam name="TRight">Right data type.</typeparam>
        /// <param name="either">The either object</param>
        /// <param name="defaultRightValue">A default value that is used, if <paramref name="either"/> was not in right state.</param>
        /// <returns>The right-value of <paramref name="either"/>, if it was in right-state; otherwiese <paramref name="defaultRightValue"/></returns>
        public static TRight GetRight<TLeft, TRight>(this Either<TLeft, TRight> either, TRight defaultRightValue)
            => either.MapEither(_ => defaultRightValue, right => right);

        /// <summary>
        /// Creates a new <see cref="Left{TLeft, TRight}"/> instance
        /// </summary>
        /// <typeparam name="TLeft">The left type.</typeparam>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="value">The left value.</param>
        public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft value) => new Left<TLeft, TRight>(value);

        /// <summary>
        /// Evaluates an enumerator of either monads and returns the
        /// error values of those in a left state.
        /// </summary>
        /// <typeparam name="TLeft">Left data type.</typeparam>
        /// <typeparam name="TRight">Right data type.</typeparam>
        /// <param name="source">Source enumerator of either monads.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="source"/> was <c>null</c>.
        /// </exception>
        public static IEnumerable<TLeft> Lefts<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source
                .Where(p => p.IsLeft)
                .Select(p => p.GetLeft());
        }

        /// <summary>
        /// Evaluates the either with one of two given mapping functions.
        /// </summary>
        /// <typeparam name="TLeft">Left data type.</typeparam>
        /// <typeparam name="TRight">Right data type.</typeparam>
        /// <typeparam name="TResult">Right data type of result.</typeparam>
        /// <param name="either">Either monad.</param>
        /// <param name="leftFunction">Function to be used for left state.</param>
        /// <param name="rightFunction">Function to be used for right state.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="either"/>, <paramref name="leftFunction"/> or <paramref name="rightFunction"/> was <c>null</c>.
        /// </exception>
        public static TResult MapEither<TLeft, TRight, TResult>(this Either<TLeft, TRight> either, Func<TLeft, TResult> leftFunction, Func<TRight, TResult> rightFunction)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (leftFunction == null) throw new ArgumentNullException(nameof(leftFunction));
            if (rightFunction == null) throw new ArgumentNullException(nameof(rightFunction));

            return either.IsLeft ? leftFunction(either.GetLeft()) : rightFunction(either.GetRight());
        }

        /// <summary>
        /// Evaluates an enumerator of either monads and returns
        /// the encapsulated values in one of the corresponding
        /// result enumerators, left or right.
        /// </summary>
        /// <typeparam name="TLeft">Left data type.</typeparam>
        /// <typeparam name="TRight">Right data type.</typeparam>
        /// <param name="source">Source enumerator of either monads.</param>
        /// <returns>
        /// A tuple containing an enumerator of all left-values an an enumerator
        /// with all right values.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="source"/> was <c>null</c>.
        /// </exception>
        public static (IEnumerable<TLeft> Left, IEnumerable<TRight> Right) PartitionEithers<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return (
                source.Where(p => p.IsLeft).Select(p => p.GetLeft()),
                source.Where(p => p.IsRight).Select(p => p.GetRight()));
        }


#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
#pragma warning disable CS1658 // Warning is overriding an error
        /// <summary>
        /// Maps a sequence of <see cref="Either{Exception, TRight}"/> to an instance of
        /// <see cref="Right{Exception, IEnumerable{TRight}}"/>, if and only if ALL
        /// elements of that sequence are <see cref="Right{Exception, TRight}"/> instances;
        /// otherwise an aggregate <see cref="Left{Exception, IEnumerable{TRight}}"/> is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eithers">The sequence of either objects.</param>
        /// <param name="errorFunc">A function to aggregate all errors of left-state either objects in <paramref name="eithers"/>.</param>
        /// <returns>A <see cref="Right{Exception, IEnumerable{TRight}}"/> instance containing a sequence of <typeparamref name="T"/>-objects,
        /// if all objects in <paramref name="eithers"/> were in right state; otherwise a <see cref="Left{Exception, IEnumerable{TRight}}"/>
        /// isntance wrapping an aggregated exception.</returns>
#if !NETCORE
#pragma warning restore CS1658 // Warning is overriding an error
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute
#endif
        public static Either<Exception, IEnumerable<T>> MapAllRightEithers<T>(this IEnumerable<Either<Exception, T>> eithers, Func<IEnumerable<Exception>, Exception> errorFunc)
        {
            var lefts = eithers.Lefts().ToList();
            return lefts.Count > 0
                ? Left<Exception, IEnumerable<T>>(errorFunc(lefts))
                : eithers.Rights().ToExceptionEither();
        }

#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
#pragma warning disable CS1658 // Warning is overriding an error
        /// <summary>
        /// Maps a sequence of <see cref="Either{Exception, TRight}"/> to an instance of
        /// <see cref="Right{Exception, IEnumerable{TRight}}"/>, if and only if ALL
        /// elements of that sequence are <see cref="Right{Exception, TRight}"/> instances;
        /// otherwise an aggregate <see cref="Left{Exception, IEnumerable{TRight}}"/> is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eithers">The sequence of either objects.</param>
        /// <returns>A <see cref="Right{Exception, IEnumerable{TRight}}"/> instance containing a sequence of <typeparamref name="T"/>-objects,
        /// if all objects in <paramref name="eithers"/> were in right state; otherwise a <see cref="Left{Exception, IEnumerable{TRight}}"/>
        /// isntance wrapping an aggregated exception.</returns>
#pragma warning restore CS1658 // Warning is overriding an error
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute
        public static Either<Exception, IEnumerable<T>> MapAllRightEithers<T>(this IEnumerable<Either<Exception, T>> eithers)
            => MapAllRightEithers(eithers, ex => new AggregateException(ex.ToArray()));

        /// <summary>
        /// Creates a new <see cref="Right{TLeft, TRight}"/> instance
        /// </summary>
        /// <typeparam name="TLeft">The left type.</typeparam>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="value">The right value.</param>
        public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight value) => new Right<TLeft, TRight>(value);

        /// <summary>
        /// Evaluates an enumerator of either monads and returns the
        /// right values of those in a right state.
        /// </summary>
        /// <typeparam name="TLeft">Left data type.</typeparam>
        /// <typeparam name="TRight">Right data type.</typeparam>
        /// <param name="source">Source enumerator of either monads.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="source"/> was <c>null</c>.
        /// </exception>
        public static IEnumerable<TRight> Rights<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source
                .Where(p => p.IsRight)
                .Select(p => p.GetRight());
        }

        /// <summary>
        /// For LINQ-Query support
        /// </summary>
        /// <typeparam name="TLeft">Left data type (Error)</typeparam>
        /// <typeparam name="TRight">Right data type (Result)</typeparam>
        /// <typeparam name="TResult">Right data type (Result) of the result either monad.</typeparam>
        /// <param name="either">Either-object</param>
        /// <param name="selector">The selector function.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="either"/> or <paramref name="selector"/> was <c>null</c>.
        /// </exception>
        public static Either<TLeft, TResult> Select<TLeft, TRight, TResult>(this Either<TLeft, TRight> either, Func<TRight, TResult> selector)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return either.BindInternal(p => new Right<TLeft, TResult>(selector(p)));
        }

        /// <summary>
        /// For LINQ-Query-Support
        /// </summary>
        /// <typeparam name="TLeft">Left data type (Error)</typeparam>
        /// <typeparam name="TRight">Right data type (Result)</typeparam>
        /// <typeparam name="TSecond">The type of the second mapping function</typeparam>
        /// <typeparam name="TResult">Right data type (Result) of the result either monad.</typeparam>
        /// <param name="either">Either-object</param>
        /// <param name="func">The function.</param>
        /// <param name="select">The select function.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="either"/>, <paramref name="func"/> or <paramref name="select"/> was <c>null</c>.
        /// </exception>
        public static Either<TLeft, TResult> SelectMany<TLeft, TRight, TSecond, TResult>(this Either<TLeft, TRight> either, Func<TRight, Either<TLeft, TSecond>> func, Func<TRight, TSecond, TResult> select)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (select == null) throw new ArgumentNullException(nameof(select));
            if (func == null) throw new ArgumentNullException(nameof(func));

            return Bind(either, aValue => Bind(func(aValue), funcValue => Right<TLeft, TResult>(select(aValue, funcValue))));
        }

        /// <summary>
        /// Creates a new <see cref="Left{TLeft, TRight}"/> instance
        /// </summary>
        /// <typeparam name="TLeft">The left type.</typeparam>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="value">The left value.</param>
        /// <param name="dummyRight">Unused parameter just for type inference</param>
        public static Either<TLeft, TRight> ToEitherLeft<TLeft, TRight>(this TLeft value, TRight dummyRight) => new Left<TLeft, TRight>(value);

        /// <summary>
        /// Creates a new <see cref="Left{TLeft, TLeft}"/> instance
        /// </summary>
        /// <typeparam name="TLeft">The left type.</typeparam>
        /// <param name="value">The left value.</param>
        public static Either<TLeft, TLeft> ToEitherLeft<TLeft>(this TLeft value) => new Left<TLeft, TLeft>(value);

        /// <summary>
        /// Creates a new <see cref="Right{T, T}"/> instance
        /// </summary>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="value">The right value.</param>
        public static Either<TRight, TRight> ToEitherRight<TRight>(this TRight value)
            => new Right<TRight, TRight>(value);

        /// <summary>
        /// Creates a new <see cref="Right{TLeft, TRight}"/> instance
        /// </summary>
        /// <typeparam name="TLeft">The left type.</typeparam>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="value">The right value.</param>
        /// <param name="dummyLeft">Unused parameter just for type inference</param>
        public static Either<TLeft, TRight> ToEitherRight<TLeft, TRight>(this TRight value, TLeft dummyLeft) => new Right<TLeft, TRight>(value);

        /// <summary>
        /// Creates a new <see cref="Right{Exception, TRight}"/> instance
        /// </summary>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="value">The right value.</param>
        public static Either<Exception, TRight> ToExceptionEither<TRight>(this TRight value)
            => new Right<Exception, TRight>(value);

        /// <summary>
        /// Creates a new <see cref="Left{Exception, TRight}" /> instance
        /// </summary>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="ex">The exception to be contained in an left state "exception either"<see cref="Left"/></param>
        /// <returns>A <see cref="Left{Exception, TRight}"/> instance containing the given exception.</returns>
        public static Either<Exception, TRight> ToExceptionEitherLeft<TRight>(this Exception ex)
            => new Left<Exception, TRight>(ex);

        /// <summary>
        /// Creates a new <see cref="Right{String, TRight}"/> instance
        /// </summary>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="value">The right value.</param>
        public static Either<string, TRight> ToStringEither<TRight>(this TRight value)
            => new Right<string, TRight>(value);
    }

    /// <summary>
    /// Strict either monad, with instant execution, no deferred evaluation.
    /// </summary>
    /// <typeparam name="TLeft">Data type of 'Left' (Error)</typeparam>
    /// <typeparam name="TRight">Data type of 'Right' (Result)</typeparam>
    public abstract class Either<TLeft, TRight>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is in left state.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is in left state; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsLeft { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is in right state.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is in right state; otherwise, <c>false</c>.
        /// </value>
        public bool IsRight => !IsLeft;

        /// <summary>
        /// Enforces the right value to be of a certain subtype of <typeparamref name="TRight"/>
        /// </summary>
        /// <typeparam name="TSubtype">Datatype, being a subtype of <typeparamref name="TRight"/>,
        /// that the right value of a <see cref="Right{TLeft, TRight}"/> instance is excpected to be.</typeparam>
        /// <param name="leftValue">Value, that will be used for a <see cref="Left{TLeft, TSubtype}"/> instance,
        /// if the typecheck fails.</param>
        /// <returns></returns>
        public abstract Either<TLeft, TSubtype> FailIfNotSubtype<TSubtype>(TLeft leftValue);

        /// <summary>
        /// Enforces the right value to be of a certain subtype of <typeparamref name="TRight"/>
        /// </summary>
        /// <typeparam name="TSubtype">Datatype, being a subtype of <typeparamref name="TRight"/>,
        /// that the right value of a <see cref="Right{TLeft, TRight}"/> instance is excpected to be.</typeparam>
        /// <param name="leftValueFactory">Function creating a value, that will be used to create a
        /// <see cref="Left{TLeft, TSubtype}"/> instance, if the typecheck fails.</param>
        /// <returns></returns>
        public abstract Either<TLeft, TSubtype> FailIfNotSubtype<TSubtype>(Func<TRight, TLeft> leftValueFactory);

        /// <summary>
        /// Gets the left value.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If instance was not in left state.</exception>
        public TLeft GetLeft()
        {
            if (!IsLeft) throw new InvalidOperationException("GetLeft() can only be invoked in 'left' state.");
            return GetLeftInternal();
        }

        /// <summary>
        /// Gets the left value or a default value,
        /// if the object is not <see cref="Left{TLeft, TRight}" /></summary>
        /// <param name="defaultValue">The default value to return, if the object is in right state.</param>
        /// <returns></returns>
        public TLeft GetLeft(TLeft defaultValue) => !IsLeft ? defaultValue : GetLeftInternal();

        /// <summary>
        /// Gets the right value.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If instance was not in right state.</exception>
        public TRight GetRight()
        {
            if (IsLeft) throw new InvalidOperationException("GetRight() can only be invoked in 'right' state.");
            return GetRightInternal();
        }

        /// <summary>
        /// Gets the right value or a default value,
        /// if the object is not <see cref="Right{TLeft, TRight}" /></summary>
        /// <param name="defaultValue">The default value to return, if the object is in left state.</param>
        /// <returns></returns>
        public TRight GetRight(TRight defaultValue) => IsLeft ? defaultValue : GetRightInternal();

        /// <summary>
        /// Binds a function, in overriding classes.
        /// </summary>
        /// <typeparam name="TRightResult">The right type of the result.</typeparam>
        /// <param name="func">The function to bind.</param>
        /// <returns></returns>
        protected internal abstract Either<TLeft, TRightResult> BindInternal<TRightResult>(Func<TRight, Either<TLeft, TRightResult>> func);

        /// <summary>
        /// Returns, in overriding classes, the left value.
        /// Throws an exception, if the instance is not in left state.
        /// </summary>
        protected abstract TLeft GetLeftInternal();

        /// <summary>
        /// Returns, in overriding classes, the right value.
        /// Throws an exception, if the instance is not in right state.
        /// </summary>
        protected abstract TRight GetRightInternal();
    }

    /// <summary>
    /// Implements an either monad in left state (generally: the failed state)
    /// </summary>
    /// <typeparam name="TLeft">The left data type.</typeparam>
    /// <typeparam name="TRight">The right data type.</typeparam>
    /// <seealso cref="Interlook.Monads.Either{TLeft, TRight}" />
    public sealed class Left<TLeft, TRight> : Either<TLeft, TRight>
    {
        private TLeft _value;
        private int _valueHash;

        /// <summary>
        /// Gets a value indicating whether this instance is in left state.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in left state; otherwise, <c>false</c>.
        /// </value>
        public override bool IsLeft => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Left{TLeft, TRight}"/> class.
        /// </summary>
        /// <param name="value">The value left.</param>
        public Left(TLeft value)
        {
            _value = value;
            _valueHash = value == null ? 0 : value.GetHashCode();
        }

        /// <summary>
        /// Static factory method.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <returns></returns>
        public static Either<TLeft, TRight> Create(TLeft left) => new Left<TLeft, TRight>(left);

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => (obj is Left<TLeft, TRight> rg) ? rg._value.Equals(_value) : false;

        public override Either<TLeft, TSubtype> FailIfNotSubtype<TSubtype>(TLeft leftValue)
                    => new Left<TLeft, TSubtype>(_value);

        public override Either<TLeft, TSubtype> FailIfNotSubtype<TSubtype>(Func<TRight, TLeft> leftValueFactory)
                    => new Left<TLeft, TSubtype>(_value);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() => _valueHash;

        /// <summary>
        /// Does not actually bind the function, since the instance is in left state.
        /// Hence, a new <see cref="Left{TLeft, TRight}"/> instance is returned.
        /// </summary>
        /// <typeparam name="TRightResult">The right type of the result.</typeparam>
        /// <param name="func">The function to bind.</param>
        /// <returns>A new <see cref="Left{TLeft, TRight}"/> with the original left value.</returns>
        protected internal override Either<TLeft, TRightResult> BindInternal<TRightResult>(Func<TRight, Either<TLeft, TRightResult>> func)
            => new Left<TLeft, TRightResult>(_value);

        /// <summary>
        /// Returns the left value.
        /// </summary>
        protected override TLeft GetLeftInternal() => _value;

        /// <summary>
        /// Not valid for instances in left state.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        protected override TRight GetRightInternal() => throw new InvalidOperationException();
    }

    /// <summary>
    /// Implements an either monad in right state
    /// </summary>
    /// <typeparam name="TLeft">The left data type.</typeparam>
    /// <typeparam name="TRight">The right data type.</typeparam>
    /// <seealso cref="Interlook.Monads.Either{TLeft, TRight}" />
    public sealed class Right<TLeft, TRight> : Either<TLeft, TRight>
    {
        private TRight _value;
        private int _valueHash;

        /// <summary>
        /// Gets a value indicating whether this instance is in left state.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in left state; otherwise, <c>false</c>.
        /// </value>
        public override bool IsLeft => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Right{TLeft, TRight}"/> class.
        /// </summary>
        /// <param name="value">The right value.</param>
        public Right(TRight value)
        {
            _value = value;
            _valueHash = value == null ? 0 : value.GetHashCode();
        }

        /// <summary>
        /// Static factory method.
        /// </summary>
        /// <param name="right">The right value.</param>
        /// <returns></returns>
        public static Either<TLeft, TRight> Create(TRight right) => new Right<TLeft, TRight>(right);

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => (obj is Right<TLeft, TRight> rg) ? rg._value.Equals(_value) : false;

        public override Either<TLeft, TSubtype> FailIfNotSubtype<TSubtype>(TLeft leftValue)
                    => _value is TSubtype sub
                        ? Either.Right<TLeft, TSubtype>(sub)
                        : new Left<TLeft, TSubtype>(leftValue);

        public override Either<TLeft, TSubtype> FailIfNotSubtype<TSubtype>(Func<TRight, TLeft> leftValueFactory)
                    => _value is TSubtype sub
                        ? Either.Right<TLeft, TSubtype>(sub)
                        : new Left<TLeft, TSubtype>(leftValueFactory(_value));

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() => _valueHash;

        /// <summary>
        /// Implementation for binding a function to the instance.
        /// </summary>
        /// <typeparam name="TRightResult">The right type of the result</typeparam>
        /// <param name="func">The function to bind.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="func"/> was <c>null</c>.</exception>
        protected internal override Either<TLeft, TRightResult> BindInternal<TRightResult>(Func<TRight, Either<TLeft, TRightResult>> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            return func(_value);
        }

        /// <summary>
        /// Not supported for instances in right state.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        protected override TLeft GetLeftInternal() => throw new InvalidOperationException();

        /// <summary>
        /// Gets the right value.
        /// </summary>
        protected override TRight GetRightInternal() => _value;
    }
}