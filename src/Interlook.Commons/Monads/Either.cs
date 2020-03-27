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
        /// Returns, in overriding classes, the left value. 
        /// Throws an exception, if the instance is not in left state.
        /// </summary>
        protected abstract TLeft GetLeftInternal();

        /// <summary>
        /// Returns, in overriding classes, the right value.
        /// Throws an exception, if the instance is not in right state.
        /// </summary>
        protected abstract TRight GetRightInternal();

        /// <summary>
        /// Binds a function, in overriding classes.
        /// </summary>
        /// <typeparam name="TRightResult">The right type of the result.</typeparam>
        /// <param name="func">The function to bind.</param>
        /// <returns></returns>
        protected internal abstract Either<TLeft, TRightResult> BindInternal<TRightResult>(Func<TRight, Either<TLeft, TRightResult>> func);
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
        /// Static factory method.
        /// </summary>
        /// <param name="right">The right value.</param>
        /// <returns></returns>
        public static Either<TLeft, TRight> Create(TRight right) => new Right<TLeft, TRight>(right);

        /// <summary>
        /// Not supported for instances in right state.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        protected override TLeft GetLeftInternal() => throw new InvalidOperationException();

        /// <summary>
        /// Gets the right value.
        /// </summary>
        protected override TRight GetRightInternal() => _value;

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
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => _valueHash;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => (obj is Right<TLeft, TRight> rg) ? rg._value.Equals(_value) : false;

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
        /// Static factory method.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <returns></returns>
        public static Either<TLeft, TRight> Create(TLeft left) => new Left<TLeft, TRight>(left);

        /// <summary>
        /// Returns the left value.
        /// </summary>
        protected override TLeft GetLeftInternal() => _value;

        /// <summary>
        /// Not valid for instances in left state.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        protected override TRight GetRightInternal() => throw new InvalidOperationException();

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
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => _valueHash;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => (obj is Left<TLeft, TRight> rg) ? rg._value.Equals(_value) : false;

        /// <summary>
        /// Does not actually bind the function, since the instance is in left state.
        /// Hence, a new <see cref="Left{TLeft, TRight}"/> instance is returned.
        /// </summary>
        /// <typeparam name="TRightResult">The right type of the result.</typeparam>
        /// <param name="func">The function to bind.</param>
        /// <returns>A new <see cref="Left{TLeft, TRight}"/> with the original left value.</returns>
        protected internal override Either<TLeft, TRightResult> BindInternal<TRightResult>(Func<TRight, Either<TLeft, TRightResult>> func) 
            => new Left<TLeft, TRightResult>(_value);
    }

    /// <summary>
    /// Contains helper/extension methods for the either monad.
    /// </summary>
    public static class Either
    {
        /// <summary>
        /// Creates a new <see cref="Left{TLeft, TRight}"/> instance
        /// </summary>
        /// <typeparam name="TLeft">The left type.</typeparam>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="value">The left value.</param>
        public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft value) => new Left<TLeft, TRight>(value);

        /// <summary>
        /// Creates a new <see cref="Right{TLeft, TRight}"/> instance
        /// </summary>
        /// <typeparam name="TLeft">The left type.</typeparam>
        /// <typeparam name="TRight">The right type.</typeparam>
        /// <param name="value">The right value.</param>
        public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight value) => new Right<TLeft, TRight>(value);

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

        /// <summary>
        /// Evaluates the either with one of two given mapping functions.
        /// </summary>
        /// <typeparam name="TLeft">Left data type.</typeparam>
        /// <typeparam name="TRight">Right data type.</typeparam>
        /// <typeparam name="TResult">Right .</typeparam>
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
    }
}