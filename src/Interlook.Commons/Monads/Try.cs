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
    public abstract class Try<TResult>
    {
        public abstract bool Failed { get; }

        protected internal abstract Try<T> BindInternal<T>(Func<TResult, Try<T>> func);
    }

    public sealed class Success<TResult> : Try<TResult>
    {
        public TResult Value { get; }

        private int _valueHash;

        public Success(TResult value)
        {
            Value = value;
            _valueHash = value == null ? 0 : value.GetHashCode();
        }

        public override bool Failed => false;

        public override int GetHashCode() => _valueHash;

        public override bool Equals(object obj) => obj is Success<TResult> succ ? succ.Value.Equals(Value) : false;

        protected internal override Try<T> BindInternal<T>(Func<TResult, Try<T>> func) => func(Value);
    }

    public sealed class Failure<TResult> : Try<TResult>
    {
        private int _errorHash;

        public override bool Failed => true;

        public object Error { get; }

        public Failure(object error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            Error = error;
            _errorHash = Error.GetHashCode();
        }

        public override int GetHashCode() => _errorHash;

        public override bool Equals(object obj) => obj is Failure<TResult> fail ? fail.Error.Equals(Error) : false;

        protected internal override Try<T> BindInternal<T>(Func<TResult, Try<T>> func) => new Failure<T>(Error);
    }

    /// <summary>
    /// Class with factory methods for <see cref="Try{T}"/>-Delegates
    /// </summary>
    public static class Try
    {
        /// <summary>
        /// <para>
        /// Creates a <see cref="Try{T}"/>-instance, encapsulating a method invocation,
        /// which either returns a value of <c>TResult</c> or
        /// throws an exception.
        /// </para>
        /// </summary>
        /// <typeparam name="TResult">Data type of the result value.</typeparam>
        /// <param name="func">The method call to execute.</param>
        /// <returns>A <see cref="Try{T}"/>-instance.</returns>
        public static Try<TResult> Invoke<TResult>(Func<TResult> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            try
            {
                var value = func();
                return new Success<TResult>(value);
            }
            catch (Exception ex)
            {
                return new Failure<TResult>(ex);
            }
        }

        /// <summary>
        /// For LINQ support
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">Ein <see cref="Try{T}"/>-Delegat.</param>
        /// <param name="selector">Selector function, that shall be applied to the return value of the method call.</param>
        /// <returns>A <see cref="Try{T}"/>-monad, encapsulating the chained (still unexecuted) calls of both,
        /// the original method call and the selector function.</returns>
        public static Try<TResult> Select<TSource, TResult>(this Try<TSource> obj, Func<TSource, TResult> selector)
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
        /// <param name="obj">A <see cref="Try{T}"/>-monad.</param>
        /// <param name="func">A function to be bound in an additional <see cref="Try{T}"/>-monad.</param>
        /// <param name="selector">A selector function for the result of both functions.</param>
        /// <returns></returns>
        public static Try<TResult> SelectMany<T1, T2, TResult>(this Try<T1> obj, Func<T1, Try<T2>> func, Func<T1, T2, TResult> selector)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            if (obj is Failure<T1> primaryFail)
            {
                return new Failure<TResult>(primaryFail.Error);
            }
            else
            {
                try
                {
                    var secondary = func(((Success<T1>)obj).Value);
                    if (secondary is Failure<T2> fail) return new Failure<TResult>(fail.Error);
                    else
                    {
                        try
                        {
                            return new Success<TResult>(selector(((Success<T1>)obj).Value, ((Success<T2>)secondary).Value));
                        }
                        catch (Exception ex)
                        {
                            return new Failure<TResult>(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new Failure<TResult>(ex);
                }
            }
        }

        /// <summary>
        /// Chains two method calls.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">A <see cref="Try{T}"/>-monad.</param>
        /// <param name="selector">Selector function, that shall be applied to
        /// the return value of the encapsulated method call.</param>
        /// <returns>
        /// A new <see cref="Try{T}"/>-monad, encapsulating the chained, but still
        /// not executed, calls of the monad's method and the selector function.
        /// </returns>
        public static Try<TResult> Then<TSource, TResult>(this Try<TSource> obj, Func<TSource, TResult> selector)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            if (obj is Failure<TSource> primaryFail)
            {
                return new Failure<TResult>(primaryFail.Error);
            }
            else
            {
                try
                {
                    var value = selector(((Success<TSource>)obj).Value);
                    return new Success<TResult>(value);
                }
                catch (Exception ex)
                {
                    return new Failure<TResult>(ex);
                }
            }
        }

        /// <summary>
        /// Similar to <see cref="SelectMany{T1, T2, TResult}(Try{T1}, Func{T1, Try{T2}}, Func{T1, T2, TResult})"/>.
        /// Safely invokes the method calls of the current and the provided monad (in that order)
        /// and provides the return values of both to a combiner function. The result of
        /// that function will be returned.
        /// </summary>
        /// <typeparam name="T">Data type of the return value of both monads.</typeparam>
        /// <param name="obj">A <see cref="Try{T}"/>-monad.</param>
        /// <param name="other">Another (the second) <see cref="Try{T}"/>-monad.</param>
        /// <param name="combiner">A function, combining the return values of the first and the second monad, if both succeed.</param>
        /// <returns></returns>
        public static Try<T> CombineWith<T>(this Try<T> obj, Try<T> other, Func<T, T, T> combiner)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (combiner == null) throw new ArgumentNullException(nameof(combiner));

            if (obj is Failure<T> firstFail)
            {
                return new Failure<T>(firstFail.Error);
            }
            else
            {
                try
                {
                    if (other is Failure<T> secondFail) return new Failure<T>(secondFail.Error);
                    else
                    {
                        try
                        {
                            return new Success<T>(combiner(((Success<T>)obj).Value, ((Success<T>)other).Value));
                        }
                        catch (Exception ex)
                        {
                            return new Failure<T>(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new Failure<T>(ex);
                }
            }
        }
    }
}