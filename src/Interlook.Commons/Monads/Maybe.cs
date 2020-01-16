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
    /// Strict implementation of maybe, thus without lazy execution.
    /// Assignments and bindings are executed instantly.
    /// </summary>
    /// <typeparam name="T">Type of the encapsulated value.</typeparam>
    public abstract class Maybe<T>
    {
        /// <summary>
        /// Binds, in overriding classes, a function to the instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The function to bind.</param>
        protected internal abstract Maybe<TResult> BindInternal<TResult>(Func<T, Maybe<TResult>> func);
    }

    /// <summary>
    /// Implements an empty <see cref="Maybe{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the value of the implemented maybe.</typeparam>
    public sealed class Nothing<T> : Maybe<T>
    {
        private const string NothingText = "Nothing";

        private static readonly Lazy<Nothing<T>> _instance = new Lazy<Nothing<T>>(() => new Nothing<T>());

        /// <summary>
        /// A default empty maybe.
        /// </summary>
        public static Nothing<T> Instance => _instance.Value;

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() => $"{NothingText}.";

        /// <summary>
        /// Returns constant hash value 0 for <see cref="Nothing{T}"/>-instances.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => 0;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is Nothing<T>;

        /// <summary>
        /// Does not actually bind the function, since the maybe object is nothing.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The function to bind.</param>
        /// <returns></returns>
        protected internal override Maybe<TResult> BindInternal<TResult>(Func<T, Maybe<TResult>> func) => new Nothing<TResult>();
    }

    /// <summary>
    /// Implements a <see cref="Maybe{T}"/>-object with an existing value.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public class Just<T> : Maybe<T>
    {
        private const string NullAsString = "null";

        private bool _isNull;
        private int _hash;
        private bool _implementsEquatable;

        /// <summary>
        /// Gets the actual value.
        /// </summary>
        /// <remarks>
        /// The value may be <c>null</c> for reference types, since this is a possible value for objects.
        /// Do not confuse <c>null</c> and <see cref="Nothing{T}"/>!
        /// </remarks>
        public T Value { get; }

        /// <summary>
        /// Creates a maybe-type with a set value.
        /// </summary>
        /// <param name="value">The value.</param>
        public Just(T value)
        {
            Value = value;
            _isNull = (object.Equals(null, Value));
            _hash = _isNull ? 0 : value.GetHashCode();
            _implementsEquatable = typeof(IEquatable<T>).IsAssignableFrom(typeof(T));
        }

        /// <summary>
        /// Converts the encapsulated value to a string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() => _isNull ? NullAsString : Value.ToString();

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => _hash;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => Equals(this, obj as Just<T>);

        /// <summary>
        /// Determines, if two valued maybe-object are equal (not necessarily identical),
        /// based on the encapsulated value, if present.
        /// </summary>
        /// <param name="x">The first maybe-object to match.</param>
        /// <param name="y">The second maybe-object to match.</param>
        /// <returns><c>true</c>, if both arguments aren't <c>null</c> and wrap the same value.</returns>
        public static bool Equals(Just<T> x, Just<T> y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if ((object)x == null || (object)y == null)
            {
                return false;
            }

            if (x._isNull || y._isNull)
            {
                return x._isNull == y._isNull;
            }
            else
            {
                if (x._implementsEquatable)
                {
                    return ((IEquatable<T>)x.Value).Equals(y.Value);
                }
                else
                {
                    return x.Value.Equals(y.Value);
                }
            }
        }

        /// <summary>
        /// Binds a function to the maybe instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The function to bind.</param>
        /// <returns>The result of the <paramref name="func"/></returns>
        protected internal override Maybe<TResult> BindInternal<TResult>(Func<T, Maybe<TResult>> func) => func(Value);
    }

    /// <summary>
    /// Extension methods for <see cref="Maybe{T}"/> types.
    /// </summary>
    public static class MaybeExtensions
    {
        /// <summary>
        /// Puts a value into a strict maybe monad. (see <see cref="Maybe{T}"/>)
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="value">The object/value to be wrapped in a maybe-object.</param>
        /// <returns>A maybe-object wrapping the given object.</returns>
        public static Maybe<T> ToMaybe<T>(this T value)
        {
            return new Just<T>(value);
        }

        /// <summary>
        /// Puts a value into a strict maybe monad. (see <see cref="Maybe{T}" />)
        /// and uses a filter function for distiction (<see cref="Just{T}" /> vs. <see cref="Nothing{T}" /> ).
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="value">The object/value to be wrapped in a maybe-object.</param>
        /// <param name="predicate">The  (filter) function to decide, if the
        /// object is wrapped into <see cref="Nothing{T}" /> or <see cref="Just{T}" />.</param>
        /// <returns>
        /// <returns>A <see cref="Just{T}" /> instance, if the predicate function returned <c>true</c>,
        /// otherwise a <see cref="Nothing{T}" /> object.</returns>
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="predicate"/> was <c>null</c>.</exception>
        public static Maybe<T> ToMaybe<T>(this T value, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return predicate(value) ? new Just<T>(value) : (Maybe<T>)new Nothing<T>();
        }

        /// <summary>
        /// Puts a value into a strict maybe monad. (see <see cref="Maybe{T}"/>)
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The object to be wrapped in a maybe-object.</param>
        /// <returns>A <see cref="Just{T}"/> instance, if the object was not <c>null</c>, otherwise
        /// a <see cref="Nothing{T}"/> object.</returns>
        public static Maybe<T> ToMaybeNotNull<T>(this T obj) where T : class
        {
            return (object)obj == null ? new Nothing<T>() : (Maybe<T>)new Just<T>(obj);
        }

        /// <summary>
        /// Extension method to make the LINQ FROM-notation usable for Maybe[T] for multiple disjunct FROMs
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="obj">The maybe aggregate.</param>
        /// <param name="collector">The function to bind.</param>
        /// <param name="select">The result flattening selector.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="obj"/>, <paramref name="collector"/> or <paramref name="select"/> was <c>null</c>;
        /// </exception>
        /// <remarks>
        /// Multiple disjunct FROMs means:
        /// the notation
        /// <code>
        /// from a in s1
        /// from b in s2
        /// select a + b
        /// </code>
        /// is tanslated to:
        /// <code>
        /// s1.SelectMany(x =&gt; s2, (a, b) =&gt; a + b)
        /// </code>
        /// </remarks>
        public static Maybe<C> SelectMany<A, B, C>(this Maybe<A> obj, Func<A, Maybe<B>> collector, Func<A, B, C> select)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (select == null) throw new ArgumentNullException(nameof(select));

            return Bind(obj, aValue => Bind(collector(aValue), funcValue => select(aValue, funcValue).ToMaybe()));
        }

        /// <summary>
        /// Returns the aggregate if the query function succeeds.
        /// </summary>
        /// <typeparam name="T">Datatype of the maybe aggregate</typeparam>
        /// <param name="obj">The maybe aggregate.</param>
        /// <param name="query">The query (predicate) for keeping the maybe,
        /// rather than returning Nothing[T].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="query"/> was <c>null</c>.</exception>
        public static Maybe<T> Where<T>(this Maybe<T> obj, Func<T, bool> query)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (query == null) throw new ArgumentNullException(nameof(query));

            return obj is Just<T> justa && query(justa.Value) ? justa : (Maybe<T>)new Nothing<T>();
        }

        /// <summary>
        /// Selector function for LINQ support.
        /// Similiar to the <see cref="Bind{TSource, TResult}(Maybe{TSource}, Func{TSource, Maybe{TResult}})"/> method,
        /// with the difference, that the selector function here does not return a <see cref="Maybe{T}"/>-type,
        /// but a arbitrary datatype, that is encapsulated afterwards.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">Das Maybe-Objekt.</param>
        /// <param name="selector">Selector function, similiar to a bind-function.</param>
        /// <returns>Result of the selector function, that has been encapsulated into a <see cref="Just{T}"/>-
        /// or a <see cref="Nothing{T}"/>-object. The latter one applies, if the provided maybe monad
        /// was already Nothing.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="selector"/> was <c>null</c>.</exception>
        public static Maybe<TResult> Select<TSource, TResult>(this Maybe<TSource> obj, Func<TSource, TResult> selector)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return obj.BindInternal(p => new Just<TResult>(selector(p)));
        }

        /// <summary>
        /// Binds an aggregate function (strict = non lazy)
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">The maybe monad.</param>
        /// <param name="functionToBind">The function to bind.</param>
        /// <returns>The resulting aggregate (maybe object) after binding the function to the given aggregate.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="functionToBind"/> was <c>null</c>.</exception>
        public static Maybe<TResult> Bind<TSource, TResult>(this Maybe<TSource> obj, Func<TSource, Maybe<TResult>> functionToBind)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (functionToBind == null) throw new ArgumentNullException(nameof(functionToBind));

            return obj.BindInternal(functionToBind);
        }

        /// <summary>
        /// Applies an action on the specified aggregate.
        /// </summary>
        /// <typeparam name="T">Encapsulated data type</typeparam>
        /// <param name="obj">The maybe monad.</param>
        /// <param name="actionToApply">The action to apply.</param>
        /// <returns>The aggregate that was provided.</returns>
        /// <remarks>
        /// This method may cause side effects using the aggregate value and
        /// does not change the aggregate itself.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="actionToApply"/> was <c>null</c>.</exception>
        public static Maybe<T> Apply<T>(this Maybe<T> obj, Action<T> actionToApply)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (actionToApply == null)
                throw new ArgumentNullException(nameof(actionToApply));

            if (obj is Just<T> justa)
            {
                actionToApply(justa.Value);
            }

            return obj;
        }

        /// <summary>
        /// Applies an action on the specified aggregate, depending on a predicate function.
        /// </summary>
        /// <typeparam name="T">Encapsulated data type</typeparam>
        /// <param name="obj">The maybe monad.</param>
        /// <param name="actionToApply">The action to apply.</param>
        /// <param name="predicate">The predicate function to decide, whether to
        /// applay the action on the aggregate.</param>
        /// <returns>
        /// The aggregate that was provided.
        /// </returns>
        /// <remarks>
        /// This method may cause side effects using the aggregate value and
        /// does not change the aggregate itself.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/>,
        /// <paramref name="predicate"/> or <paramref name="actionToApply"/> was <c>null</c>.</exception>
        public static Maybe<T> Apply<T>(this Maybe<T> obj, Action<T> actionToApply, Func<T, bool> predicate)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            if (actionToApply == null)
                throw new ArgumentNullException(nameof(actionToApply));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (obj is Just<T> justa && predicate(justa.Value))
            {
                actionToApply(justa.Value);
            }

            return obj;
        }

        /// <summary>
        /// Determines whether the specified maybe-object has a value.
        /// </summary>
        /// <typeparam name="T">The type of the object-type</typeparam>
        /// <param name="obj">The maybe-object to check.</param>
        /// <returns><c>true</c>, if the maybe-object contains a value. <seealso cref="Just{T}"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> was <c>null</c>.</exception>
        public static bool HasValue<T>(this Maybe<T> obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return obj is Just<T>;
        }

        /// <summary>
        /// Determines whether the specified maybe-object has no value.
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The maybe-object to check.</param>
        /// <returns><c>true</c>, if the maybe-object contains no value. <seealso cref="Nothing{T}"/></returns>
        /// <remarks>
        /// Do not confuse <c>null</c> and <see cref="Nothing{T}"/>!
        /// This method also returns <c>true</c>, if a maybe's value is <c>null</c>,
        /// since this is a valid value for objects.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> was <c>null</c>.</exception>
        public static bool IsNothing<T>(this Maybe<T> obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return obj is Nothing<T>;
        }

        /// <summary>
        /// Gets the value of the maybe-object.
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The maybe-object.</param>
        /// <param name="defaultValue">The default value to return, if the maybe object contains no value.</param>
        /// <returns>The value of the maybe-object, if existing, or the given default value otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> was <c>null</c>.</exception>
        public static T GetValue<T>(this Maybe<T> obj, T defaultValue)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return obj is Just<T> just ? just.Value : defaultValue;
        }

        /// <summary>
        /// Returns another maybe-object, if the maybe-object is empty (<see cref="Nothing{T}"/>)
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The maybe-object.</param>
        /// <param name="defaultMaybe">The default maybe-object, if the given maybe-object is empty.</param>
        /// <returns>The original maybe-object, if it contains a value; otherwise the given default maybe-object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="defaultMaybe"/> was <c>null</c>.</exception>
        public static Maybe<T> Otherwise<T>(this Maybe<T> obj, Maybe<T> defaultMaybe)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (defaultMaybe == null) throw new ArgumentNullException(nameof(defaultMaybe));

            return obj is Just<T> just ? just : defaultMaybe;
        }

        /// <summary>
        /// Throws an exception, if the maybe-aggregate is empty (<see cref="Nothing{T}"/>)
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The maybe-object.</param>
        /// <param name="exceptionToThrow">The exception to throw.</param>
        /// <returns>The original maybe-aggregate, if it contains a value</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="exceptionToThrow"/> was <c>null</c>.</exception>
        public static Maybe<T> OtherwiseThrow<T>(this Maybe<T> obj, Exception exceptionToThrow)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (exceptionToThrow == null) throw new ArgumentNullException(nameof(exceptionToThrow));

            if (obj is Just<T> just)
            {
                return just;
            }
            else
            {
                throw exceptionToThrow;
            }
        }

        /// <summary>
        /// Executes a predicate query with the maybe-object's value.
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The maybe-object.</param>
        /// <param name="predicate">The predicate to execute with the maybe-object's value.</param>
        /// <returns>
        /// A maybe-object, containing the boolean result of the given predicate, if the maybe-object contains a value;
        /// otherwise an empty maybe-value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="predicate"/> was <c>null</c>.</exception>
        public static Maybe<bool> Satisfies<T>(this Maybe<T> obj, Func<T, bool> predicate)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return obj is Just<T> just ? new Just<bool>(predicate(just.Value)) : (Maybe<bool>)new Nothing<bool>();
        }

        /// <summary>
        /// Version of Map (LINQ Select), applying a mapping function, which returns a maybe monad,
        /// to an <see cref="IEnumerable{T}"/> and only returns values of <see cref="Just{T}"/>-instances,
        /// thus leaving out <see cref="Nothing{T}"/>-elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the objects within the source enumerator.</typeparam>
        /// <typeparam name="TResult">The type of the objects within the result enumerator.</typeparam>
        /// <param name="source">The source enumerator.</param>
        /// <param name="selector">The result selector function.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> was <c>null</c>.</exception>
        public static IEnumerable<TResult> MapMaybe<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Maybe<TResult>> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return source.Select(p => selector(p))
                .OfType<Just<TResult>>()
                .Select(p => p.Value);
        }

        /// <summary>
        /// Returns the value, encapsulated by a maybe-monad,
        /// as an enumerator, returning nothing for a <see cref="Nothing{T}"/>-instance
        /// and the encapsulated value otherwise.</summary>
        /// <typeparam name="T">The encapsulated data type.</typeparam>
        /// <param name="obj">The maybe instance.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> was <c>null</c>.</exception>
        public static IEnumerable<T> MaybeToList<T>(this Maybe<T> obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return !(obj is Just<T> just) ? new List<T>() : new List<T>() { just.Value };
        }

        /// <summary>
        /// Returns for an enumerator of maybe-instances the
        /// actual encapsulated values inside an enumerator,
        /// leaving out <see cref="Nothing{T}"/>-instances.
        /// </summary>
        /// <typeparam name="T">Data type, encapsulated by the maybe-monad in the source enumerator.</typeparam>
        /// <param name="source">The source enumerator.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <c>null</c>.</exception>
        public static IEnumerable<T> CatMaybes<T>(this IEnumerable<Maybe<T>> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.OfType<Just<T>>()
                .Select(p => p.Value);
        }

        /// <summary>
        /// Puts the first element returned by the enumerator into a maybe-monad,
        /// returning <see cref="Nothing{T}"/> for an empty enumerator.
        /// </summary>
        /// <typeparam name="T">Data type, encapsulated by the maybe-monad in the source enumerator.</typeparam>
        /// <param name="source">The source enumerator.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <c>null</c>.</exception>
        public static Maybe<T> FirstToMaybe<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Any() ? new Just<T>(source.First()) : (Maybe<T>)new Nothing<T>();
        }

        /// <summary>
        /// Puts the first element returned by the enumerator, that matches a given filter,
        /// into a maybe-monad, returning <see cref="Nothing{T}"/> for an enumerator with
        /// no matching elements.
        /// </summary>
        /// <typeparam name="T">Data type, encapsulated by the maybe-monad in the source enumerator.</typeparam>
        /// <param name="source">The source enumerator.</param>
        /// <param name="predicate">The filter criterion.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> was <c>null</c>.</exception>
        public static Maybe<T> FirstToMaybe<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var results = source.Where(predicate)
                .Take(1)
                .ToList();

            return results.Any() ? new Just<T>(results[0]) : (Maybe<T>)new Nothing<T>();
        }
    }
}