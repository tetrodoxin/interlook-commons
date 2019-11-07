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
    /// <seealso cref="AbstractMonad{TRight}" />
    public abstract class Either<TLeft, TRight>
    {
        public abstract bool IsLeft { get; }

        public bool IsRight => !IsLeft;

        public TLeft GetLeft()
        {
            if (!IsLeft) throw new InvalidOperationException("GetLeft() can only be invoked in 'left' state.");
            return GetLeftInternal();
        }

        public TRight GetRight()
        {
            if (IsLeft) throw new InvalidOperationException("GetRight() can only be invoked in 'right' state.");
            return GetRightInternal();
        }

        protected abstract TLeft GetLeftInternal();

        protected abstract TRight GetRightInternal();

        protected internal abstract Either<TLeft, TRightResult> BindInternal<TRightResult>(Func<TRight, Either<TLeft, TRightResult>> mapper);
    }

    public sealed class Right<TLeft, TRight> : Either<TLeft, TRight>
    {
        private TRight _value;
        private int _valueHash;

        public override bool IsLeft => false;

        public static Either<TLeft, TRight> Create(TRight right) => new Right<TLeft, TRight>(right);

        protected override TLeft GetLeftInternal() => throw new InvalidOperationException();

        protected override TRight GetRightInternal() => _value;

        public Right(TRight value)
        {
            _value = value;
            _valueHash = value == null ? 0 : value.GetHashCode();
        }

        public override int GetHashCode() => _valueHash;

        public override bool Equals(object obj) => (obj is Right<TLeft, TRight> rg) ? rg._value.Equals(_value) : false;

        protected internal override Either<TLeft, TRightResult> BindInternal<TRightResult>(Func<TRight, Either<TLeft, TRightResult>> func) 
            => func(_value);
    }

    public sealed class Left<TLeft, TRight> : Either<TLeft, TRight>
    {
        private TLeft _value;
        private int _valueHash;

        public override bool IsLeft => true;

        public static Either<TLeft, TRight> Create(TLeft left) => new Left<TLeft, TRight>(left);

        protected override TLeft GetLeftInternal() => _value;

        protected override TRight GetRightInternal() => throw new InvalidOperationException();

        public Left(TLeft value)
        {
            _value = value;
            _valueHash = value == null ? 0 : value.GetHashCode();
        }

        public override int GetHashCode() => _valueHash;

        public override bool Equals(object obj) => (obj is Left<TLeft, TRight> rg) ? rg._value.Equals(_value) : false;

        protected internal override Either<TLeft, TRightResult> BindInternal<TRightResult>(Func<TRight, Either<TLeft, TRightResult>> func) 
            => new Left<TLeft, TRightResult>(_value);
    }

    public static class Either
    {
        public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft value) => new Left<TLeft, TRight>(value);

        public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight value) => new Right<TLeft, TRight>(value);

        public static Either<TLeft, TRightResult> Bind<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either, Func<TRight, Either<TLeft, TRightResult>> functionToBind)
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
        /// <returns>Either-instance in the possibly changed state.</returns>
        public static Either<TLeft, TRight> FailIf<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TRight, bool> errorCondition, TLeft errorValue)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (errorCondition == null) throw new ArgumentNullException(nameof(errorCondition));

            if (either.IsRight && errorCondition(either.GetRight())) return Left<TLeft, TRight>(errorValue);
            else return either;
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
        /// <returns></returns>
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
        public static TResult MapEither<TLeft, TRight, TResult>(this Either<TLeft, TRight> either, Func<TLeft, TResult> leftFunction, Func<TRight, TResult> rightFunction)
        {
            if (either == null) throw new ArgumentNullException(nameof(either));
            if (leftFunction == null) throw new ArgumentNullException(nameof(leftFunction));
            if (rightFunction == null) throw new ArgumentNullException(nameof(rightFunction));

            return either.IsLeft ? leftFunction(either.GetLeft()) : rightFunction(either.GetRight());
        }
    }
}