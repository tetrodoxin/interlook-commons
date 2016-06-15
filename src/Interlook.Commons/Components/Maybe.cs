#region license

//MIT License

//Copyright(c) 2016 Andreas Huebner

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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Interlook.Components
{
    /// <summary>
    /// Extension methods for <see cref="Maybe{T}"/> types.
    /// </summary>
    public static class MaybeExtensions
    {
        /// <summary>
        /// Applies an action on the specified aggregate.
        /// </summary>
        /// <typeparam name="T">Type of the aggregate (maybe object)</typeparam>
        /// <param name="a">The aggregate.</param>
        /// <param name="actionToApply">The action to apply.</param>
        /// <returns>The aggregate that was provided.</returns>
        /// <remarks>
        /// This method just causes side effects on the aggregate value and
        /// does not change the aggregate itself.
        /// </remarks>
        public static Maybe<T> Apply<T>(this Maybe<T> a, Action<T> actionToApply)
        {
            Contract.Requires<ArgumentNullException>(actionToApply != null, "actionToApply");

            var justa = a as Just<T>;
            if (justa != null)
            {
                actionToApply(justa.Value);
            }

            return a;
        }

        /// <summary>
        /// Applies an action on the specified aggregate, depending on a predicate function.
        /// </summary>
        /// <typeparam name="T">Type of the aggregate (maybe object)</typeparam>
        /// <param name="a">The aggregate.</param>
        /// <param name="actionToApply">The action to apply.</param>
        /// <param name="predicate">The predicate function to decide, whether to
        /// applay the action on the aggregate.</param>
        /// <returns>
        /// The aggregate that was provided.
        /// </returns>
        /// <remarks>
        /// This method just causes side effects on the aggregate value and
        /// does not change the aggregate itself.
        /// </remarks>
        public static Maybe<T> Apply<T>(this Maybe<T> a, Action<T> actionToApply, Func<T, bool> predicate)
        {
            Contract.Requires<ArgumentNullException>(actionToApply != null, "actionToApply");
            Contract.Requires<ArgumentNullException>(predicate != null, "predicate");

            var justa = a as Just<T>;
            if (justa != null && predicate(justa.Value))
            {
                actionToApply(justa.Value);
            }

            return a;
        }

        /// <summary>
        /// Binds an aggregate function, that is executed immediately (strict => not deferred)
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="a">The maybe aggregate.</param>
        /// <param name="functionToBind">The function to bind.</param>
        /// <returns>The resulting aggregate (maybe object) after binding the function to the given aggregate.</returns>
        public static Maybe<TResult> Bind<TSource, TResult>(this Maybe<TSource> a, Func<TSource, Maybe<TResult>> functionToBind)
        {
            var justa = a as Just<TSource>;
            if (justa == null)
            {
                return new Nothing<TResult>();
            }
            else
            {
                return functionToBind(justa.Value);
            }
        }

        /// <summary>
        /// Gets the value of the maybe-object.
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The maybe-object.</param>
        /// <param name="defaultValue">The default value to return, if the maybe object contains no value.</param>
        /// <returns>The value of the maybe-object, if existing, or the given default value otherwise.</returns>
        public static T GetValue<T>(this Maybe<T> obj, T defaultValue)
        {
            var just = obj as Just<T>;
            if (just != null)
            {
                return just.Value;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Determines whether the specified maybe-object has a value.
        /// </summary>
        /// <typeparam name="T">The type of the object-type</typeparam>
        /// <param name="obj">The maybe-object to check.</param>
        /// <returns><c>true</c>, if the maybe-object contains a value. <seealso cref="Just{T}"/></returns>
        public static bool HasValue<T>(this Maybe<T> obj)
        {
            var just = obj as Just<T>;
            return just != null;
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
        public static bool IsNothing<T>(this Maybe<T> obj)
        {
            var nothing = obj as Nothing<T>;
            return nothing != null;
        }

        /// <summary>
        /// Returns another maybe-object, if the maybe-object is empty (<see cref="Nothing{T}"/>)
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The maybe-object.</param>
        /// <param name="defaultMaybe">The default maybe-object, if the given maybe-object is empty.</param>
        /// <returns>The original maybe-object, if it contains a value; otherwise the given default maybe-object.</returns>
        public static Maybe<T> Otherwise<T>(this Maybe<T> obj, Maybe<T> defaultMaybe)
        {
            Contract.Requires<ArgumentNullException>(defaultMaybe != null, "defaultMaybe");

            var just = obj as Just<T>;
            if (just != null)
            {
                return just;
            }
            else
            {
                return defaultMaybe;
            }
        }

        /// <summary>
        /// Throws an exception, if the maybe-aggregate is empty (<see cref="Nothing{T}"/>)
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The maybe-object.</param>
        /// <param name="exceptionToThrow">The exception to throw.</param>
        /// <returns>The original maybe-aggregate, if it contains a value</returns>
        public static Maybe<T> OtherwiseThrow<T>(this Maybe<T> obj, Exception exceptionToThrow)
        {
            var just = obj as Just<T>;
            if (just != null)
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
        public static Maybe<bool> Satisfies<T>(this Maybe<T> obj, Func<T, bool> predicate)
        {
            Contract.Requires<ArgumentNullException>(predicate != null, "predicate");

            var just = obj as Just<T>;
            if (just != null)
            {
                return new Just<bool>(predicate(just.Value));
            }
            else
            {
                return new Nothing<bool>();
            }
        }

        /// <summary>
        /// Selector function for LINQ support.
        /// Is similiar to the <see cref="Bind{TSource, TResult}(Maybe{TSource}, Func{TSource, Maybe{TResult}})"/> method,
        /// with the difference, that it does not return a <see cref="Maybe{T}"/>-type,
        /// but any type, that is wrapped afterwards.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">Das Maybe-Objekt.</param>
        /// <param name="selector">Selector function, projecting the result from the wrapped value.</param>
        /// <returns>The result of the selector function wrapped in a <see cref="Just{T}"/>-object
        /// or respectively a <see cref="Nothing{T}"/>-object, if the given Maybe was already <c>Nothing{T}</c>.</returns>
        public static Maybe<TResult> Select<TSource, TResult>(this Maybe<TSource> obj, Func<TSource, TResult> selector)
        {
            var justa = obj as Just<TSource>;
            if (justa == null)
            {
                return new Nothing<TResult>();
            }
            else
            {
                return selector(justa.Value).ToMaybe();
            }
        }

        /// <summary>
        /// Extension method to make the LINQ FROM-notation usable for Maybe[T] for multiple disjunct FROMs
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="a">The maybe aggregate.</param>
        /// <param name="func">The function to bind.</param>
        /// <param name="select">The result flattening selector.</param>
        /// <returns></returns>
        /// <remarks>
        /// Multiple disjunct FROMs means:
        ///		query notation
        ///		<code>
        ///			from a in monad1
        ///			from b in monad2
        ///			select a + b
        ///		</code>
        ///
        ///		is tanslated to:
        ///		<code>
        ///			s1.SelectMany(a => monad2, (monad1, b) => a + b)
        ///		</code>
        /// </remarks>
        public static Maybe<C> SelectMany<A, B, C>(this Maybe<A> a, Func<A, Maybe<B>> func, Func<A, B, C> select)
            => a.Bind(aValue => func(aValue).Bind(funcValue => select(aValue, funcValue).ToMaybe()));

        /// <summary>
        /// Wraps the object in a strict Maybe instance. (see <see cref="Maybe{T}"/>)
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="value">The object/value to be wrapped in a maybe-object.</param>
        /// <returns>A maybe-object wrapping the given object.</returns>
        public static Maybe<T> ToMaybe<T>(this T value) => new Just<T>(value);

        /// <summary>
        /// Wraps the object in a strict Maybe instance. (see <see cref="Maybe{T}"/>)
        /// and uses a filter function to distinguish <see cref="Just{T}"/> from <see cref="Nothing{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="value">The object/value to be wrapped in a maybe-object.</param>
        /// <param name="predicate">The  (filter) function to decide, if the
        /// object is wrapped into <see cref="Nothing{T}"/> or <see cref="Just{T}"/>.</param>
        /// <returns>
        ///   <returns>A <see cref="Just{T}"/> instance, if the predicate function returned <c>true</c>,
        /// otherwise a <see cref="Nothing{T}"/> object.</returns>
        /// </returns>
        public static Maybe<T> ToMaybe<T>(this T value, Func<T, bool> predicate)
        {
            Contract.Requires<ArgumentNullException>(predicate != null, "predicate");

            if (predicate(value))
            {
                return new Just<T>(value);
            }
            else
            {
                return new Nothing<T>();
            }
        }

        /// <summary>
        /// Wraps the object in a strict Maybe instance (see <see cref="Maybe{T}"/>),
        /// if it is not <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The object to be wrapped in a maybe-object.</param>
        /// <returns>A <see cref="Just{T}"/> instance, if the object was not <c>null</c>, otherwise
        /// a <see cref="Nothing{T}"/> object.</returns>
        public static Maybe<T> ToMaybeNotNull<T>(this T obj) where T : class
        {
            if ((object)obj == null)
            {
                return new Nothing<T>();
            }
            else
            {
                return new Just<T>(obj);
            }
        }

        /// <summary>
        /// Returns the aggregate if the query function succeeds.
        /// </summary>
        /// <typeparam name="T">Datatype of the maybe aggregate</typeparam>
        /// <param name="obj">The maybe aggregate.</param>
        /// <param name="query">The query (predicate) for keeping the maybe,
        /// rather than returning Nothing[T].</param>
        /// <returns></returns>
        public static Maybe<T> Where<T>(this Maybe<T> obj, Func<T, bool> query)
        {
            Contract.Requires<ArgumentNullException>(query != null, "query");

            var justa = obj as Just<T>;
            if (justa != null && query(justa.Value))
            {
                return justa;
            }
            else
            {
                return new Nothing<T>();
            }
        }
    }

    /// <summary>
    /// Implements a <see cref="Maybe{T}"/>-object with an existing value.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public class Just<T> : Maybe<T>
    {
        protected const string NULL_AS_STRING = "null";

        private bool _implementsEquatable;
        private bool _isNull;

        /// <summary>
        /// Gets the actual value.
        /// </summary>
        /// <value>
        /// The wrapped value, that may also be <c>null</c> for reference types.
        /// </value>
        /// <remarks>
        /// The value may be <c>null</c> for reference types, since this is a possible value for objects.
        /// Do not confuse <c>null</c> and <see cref="Nothing{T}" />!
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
            _implementsEquatable = typeof(IEquatable<T>).IsAssignableFrom(typeof(T));
        }

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
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (_isNull)
            {
                return NULL_AS_STRING;
            }
            else
            {
                return Value.ToString();
            }
        }

        /// <summary>
        /// Returns the hashcode of the wrapped value, which may be <c>0</c>
        /// for <c>null</c> values.
        /// </summary>
        /// <returns></returns>
        protected override int CalculateHashCode()
        {
            if (_isNull)
            {
                return 0;
            }
            else
            {
                return Value.GetHashCode();
            }
        }

        /// <summary>
        /// Checks, if this instance is equals to a given instance, see <see cref="Equals(Just{T}, Just{T})"/>
        /// </summary>
        /// <param name="obj">Instance to check for equality.</param>
        /// <returns></returns>
        protected override bool IsEqualToInstance(Maybe<T> obj) => Equals(this, obj as Just<T>);
    }

    /// <summary>
    /// Strict Maybe-type, withour deferred execution.
    /// Assignments and bound functions are executed immediately.
    /// </summary>
    /// <typeparam name="T">Type of the encapsulated value.</typeparam>
    public abstract class Maybe<T> : IEquatable<Maybe<T>>
    {
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" }, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var maybe = obj as Maybe<T>;
            return Equals(maybe);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Maybe<T> other)
        {
            if (other != null)
            {
                return IsEqualToInstance(other);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return CalculateHashCode();
        }

        /// <summary>
        /// Calculates the hash code in overriding classes.
        /// </summary>
        /// <returns></returns>
        protected abstract int CalculateHashCode();

        /// <summary>
        /// Checks in overriding classes, if this instance is equal to another instance.
        /// </summary>
        /// <param name="obj">Die zu vergleichende Instanz.</param>
        /// <returns></returns>
        protected abstract bool IsEqualToInstance(Maybe<T> obj);
    }

    /// <summary>
    /// Implements an empty <see cref="Maybe{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the value of the implemented maybe.</typeparam>
    public sealed class Nothing<T> : Maybe<T>
    {
        private const string NOTHING_CAP1 = "Nothing";
        private const string NOTHING_LOWER = "nothing";

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
        public override string ToString() => NOTHING_CAP1 + ".";

        /// <summary>
        /// Returns <c>0</c> as hashcode for <see cref="Nothing{T}"/>-instances.
        /// </summary>
        /// <returns></returns>
        protected override int CalculateHashCode() => 0;

        /// <summary>
        /// Checks, if a given instance is also a <see cref="Nothing{T}"/>-object.
        /// </summary>
        /// <param name="obj">Instance to check.</param>
        /// <returns></returns>
        protected override bool IsEqualToInstance(Maybe<T> obj) => obj is Nothing<T>;
    }
}