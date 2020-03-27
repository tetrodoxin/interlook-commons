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

namespace Interlook.Monads
{
    /// <summary>
    /// Class with factory methods for <see cref="Try{T}"/>-Delegates
    /// </summary>
    public static class Try
    {
        /// <summary>
        /// Binds the specified function to the try monad.
        /// The resulting instance depends to the return value
        /// of <paramref name="func"/> and if the function
        /// threw an exception, which would result in an <see cref="Failure{TResult}"/> instance.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">A <see cref="Try{TSource}"/> monad.</param>
        /// <param name="func">The function to bind.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj"/> or <paramref name="func"/> was null.
        /// </exception>
        public static Try<TResult> Bind<TSource, TResult>(this Try<TSource> obj, Func<TSource, Try<TResult>> func)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (func == null) throw new ArgumentNullException(nameof(func));

            return obj.BindInternal(func);
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
                return firstFail;
            }
            else
            {
                if (other is Failure<T> secondFail)
                {
                    return secondFail;
                }
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
        }

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
        /// Evaluates the try monad with one of two given mapping functions
        /// depending on the state of the try.
        /// </summary>
        /// <typeparam name="TSource">Type of the result of <paramref name="obj"/>.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="obj">A try monad.</param>
        /// <param name="successFunc">Function to be used for <see cref="Success{TResult}"/> instances.</param>
        /// <param name="failureFunc">Function to be used for <see cref="Failure{TResult}"/> instances.</param>
        /// <returns>The result of one of the specified functions, that was accordingly applied.</returns>
        public static TResult MapTry<TSource, TResult>(this Try<TSource> obj, Func<TSource, TResult> successFunc, Func<object, TResult> failureFunc)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (successFunc == null) throw new ArgumentNullException(nameof(successFunc));
            if (failureFunc == null) throw new ArgumentNullException(nameof(failureFunc));

            return (obj is Success<TSource> succ) ? successFunc(succ.Value) : failureFunc(((Failure<TSource>)obj).Error);
        }

        /// <summary>
        /// For LINQ support. Equal to <see cref="Then{TSource, TResult}(Try{TSource}, Func{TSource, TResult})"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">A <see cref="Try{T}"/> monad.</param>
        /// <param name="selector">Selector function, that shall be applied to the return value of the method call.</param>
        /// <returns>A <see cref="Try{T}"/>-monad, encapsulating the chained (still unexecuted) calls of both,
        /// the original method call and the selector function.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj"/> or <paramref name="selector"/> was null.
        /// </exception>
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj"/> or <paramref name="selector"/> was null.
        /// </exception>
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
    }

    /// <summary>
    /// A concrete <see cref="Try{TResult}"/> implementation
    /// for failed method invocations without valid
    /// results, but an error object.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <seealso cref="Interlook.Monads.Try{TResult}" />
    public sealed class Failure<TResult> : Try<TResult>
    {
        private readonly int _errorHash;

        /// <summary>
        /// Returns the error object
        /// </summary>
        public object Error { get; }

        /// <summary>
        /// Returns, if the try was a failure
        /// </summary>
        /// <value>
        /// <c>true</c>, since this implementation represents failures.
        /// </value>
        public override bool Failed => true;

        /// <summary>
        /// Creates a new <see cref="Failure{TResult}"/> instance
        /// </summary>
        /// <param name="error">The error object to assign to the instance. Must not be <c>null</c></param>
        public Failure(object error)
        {
            Error = error ?? throw new ArgumentNullException(nameof(error));
            _errorHash = Error.GetHashCode();
        }

        /// <summary>Determines whether the specified <see cref="System.Object"/>, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => obj is Failure<TResult> fail ? fail.Error.Equals(Error) : false;

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() => _errorHash;

        /// <summary>
        /// Does not bind a function (since it's in failure state already)
        /// but returns another <see cref="Failure{T}"/> instance
        /// with the original error object.
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        /// <param name="func">Function to bind.</param>
        /// <returns></returns>
        protected internal override Try<T> BindInternal<T>(Func<TResult, Try<T>> func) => new Failure<T>(Error);
    }

    /// <summary>
    /// A concrete <see cref="Try{TResult}"/> implementation
    /// for successfull method results.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <seealso cref="Interlook.Monads.Try{TResult}" />
    public sealed class Success<TResult> : Try<TResult>
    {
        private readonly int _valueHash;

        /// <summary>
        /// Gets a value indicating whether the represented method
        /// call has failed.
        /// </summary>
        /// <value>
        /// <c>true</c> for the <see cref="Success{TResult}"/> implementation
        /// </value>
        public override bool Failed => false;

        /// <summary>
        /// Gets the actual result value.
        /// </summary>
        public TResult Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Success{TResult}"/> class,
        /// representing a successful method invocation
        /// </summary>
        /// <param name="value">The result value.</param>
        public Success(TResult value)
        {
            Value = value;
            _valueHash = value == null ? 0 : value.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is Success<TResult> succ ? succ.Value.Equals(Value) : false;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() => _valueHash;

        /// <summary>
        /// Binds a function to the internal state/value
        /// and returns a new <see cref="Try{T}"/> instance,\
        /// depending on the return value of <paramref name="func"/>
        /// and if it threw an exception.
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        /// <param name="func">Function to bind.</param>
        /// <returns>A new <see cref="Success{T}"/> instance for
        /// succeeded method calls; otherwise <see cref="Failure{T}"/>,
        /// if the function <paramref name="func"/> returned one or threw an exception</returns>
        protected internal override Try<T> BindInternal<T>(Func<TResult, Try<T>> func)
        {
            try
            {
                return func(Value);
            }
            catch (Exception ex)
            {
                return new Failure<T>(ex);
            }
        }
    }

    /// <summary>
    /// Abstract type for the try monad.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public abstract class Try<TResult>
    {
        /// <summary>
        /// Gets a value indicating whether the represented method
        /// call has failed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if invocation failed; otherwise, <c>false</c>.
        /// </value>
        public abstract bool Failed { get; }

        /// <summary>
        /// Binds in overriding classes a function to the internal state/value
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        /// <param name="func">Function to bind.</param>
        /// <returns></returns>
        protected internal abstract Try<T> BindInternal<T>(Func<TResult, Try<T>> func);
    }
}