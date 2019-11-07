using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interlook.Monads
{
    /// <summary>
    /// Lazy implementation of maybe, thus with deferred execution.
    /// Assignments and bindings are executed not before
    /// the monad is evaluated (e.g. by <c>GetValue, HasValue, IsNothing</c> etc.)
    /// </summary>
    /// <typeparam name="T">Type of the encapsulated value.</typeparam>
    public delegate Maybe<T> MaybeLazy<T>();

    /// <summary>
    /// Maybe-Monad with deferred execution.
    /// Increases reusability of maybe instances.
    /// </summary>
    /// <remarks>
    /// When using the lazy version of maybe (corresponding to the native Maybe in Haskell) one has to consider,
    /// that some methods like <see cref="MaybeLazyExtensions.GetValue{T}(MaybeLazy{T}, T)"/>
    /// or <see cref="MaybeLazyExtensions.IsNothing{T}(MaybeLazy{T})"/> evaluate the monad, so multiple executions
    /// of the same actions are possible in inept code.
    /// </remarks>
    /// <example>
    /// Following code is ineffective:
    ///
    /// <code>
    /// <![CDATA[LazyMaybe<int>]]> m = getSomeLazyMaybe();
    /// // DONT DO THAT
    /// if(m.HasValue())        // Maybe is evaluated
    /// {
    ///     doSomething(m.GetValue(0));     // Maybe is evaluated again
    /// }
    /// </code>
    ///
    /// Better:
    ///
    /// <code>
    /// <![CDATA[LazyMaybe<int>]]> m = getSomeLazyMaybe();
    /// var mb = m();           // returning a strict maybe, that is already evaluated
    /// if(mb.HasValue())
    /// {
    ///     doSomething(mb.GetValue(0));        // no re-evaluation
    /// }
    /// </code>
    ///
    /// This code is only for illustration. If your code uses HasValue() with following GetValue often,
    /// you should review the correct usage of lazy maybes.
    /// </example>
    public static class MaybeLazy
    {
        /// <summary>
        /// Returns a <see cref="MaybeLazy{T}"/> instance, that will
        /// later result in <see cref="Components.Nothing{T}"/>.
        /// </summary>
        /// <typeparam name="T">Encapsulated data type.</typeparam>
        /// <returns>An instance of <see cref="MaybeLazy{T}"/>,
        /// that encapsulates no value (see <see cref="Components.Nothing{T}"/></returns>
        public static MaybeLazy<T> Nothing<T>() => () => new Nothing<T>();

        /// <summary>
        /// Returns a <see cref="MaybeLazy{T}"/> instance, that will
        /// later result in <see cref="Components.Nothing{T}"/>.
        /// This overload is syntactical only sugar.
        /// </summary>
        /// <typeparam name="T">Encapsulated data type.</typeparam>
        /// <param name="dummy">Unused dummy value, that is only used for type inference.</param>
        /// <returns>An instance of <see cref="MaybeLazy{T}"/>,
        /// that encapsulates no value (see <see cref="Components.Nothing{T}"/></returns>
        /// <example>
        /// <code>
        ///     var m = MaybeLazy.NothingLike(1);   // gets us an <![CDATA[Nothing<int>]]>
        ///     var n = MaybeLazy.<![CDATA[Nothing<int>]]>();   // same result as above
        /// </code>
        /// </example>
        public static MaybeLazy<T> NothingLike<T>(T dummy) => () => new Nothing<T>();

        /// <summary>
        /// Returns an instance of <see cref="MaybeLazy{T}"/>, that will
        /// result in <see cref="Components.Just{T}"/> later.
        /// </summary>
        /// <typeparam name="T">Encapsulated data type.</typeparam>
        /// <returns>An instance of <see cref="MaybeLazy{T}"/>,
        /// encapsulating the given value (see <see cref="Components.Just{T}"/></returns>
        public static MaybeLazy<T> Just<T>(T value) => () => new Just<T>(value);

        /// <summary>
        /// Returns an instance of <see cref="MaybeLazy{T}"/>, that will
        /// result in <see cref="Components.Just{T}"/> later, encapsulating
        /// the return value of a function .
        /// </summary>
        /// <typeparam name="T">Encapsulated data type.</typeparam>
        /// <param name="selector">Function, which returns the value to be encapsulated.</param>
        /// <returns></returns>
        public static MaybeLazy<T> Return<T>(Func<T> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return () => new Just<T>(selector());
        }

        /// <summary>
        /// Returns an instance of <see cref="MaybeLazy{T}"/>, that will
        /// result in <see cref="Components.Just{T}" /> or <see cref="Components.Nothing{T}" />,
        /// depending on whether the return value of the given function matches
        /// the filter criterion, defined by the provided predicate function.
        /// </summary>
        /// <typeparam name="T">Encapsulated data type.</typeparam>
        /// <param name="selector">Function, which returns the value to be encapsulated.</param>
        /// <param name="predicate">Filter predicate function.</param>
        /// <returns></returns>
        public static MaybeLazy<T> ReturnIf<T>(Func<T> selector, Func<T, bool> predicate)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return () =>
            {
                var v = selector();
                if (predicate(v))
                {
                    return new Just<T>(selector());
                }
                else
                {
                    return new Nothing<T>();
                }
            };
        }
    }

    /// <summary>
    /// Extension methods for <see cref="MaybeLazy{T}"/> types.
    /// </summary>
    public static class MaybeLazyExtensions
    {
        /// <summary>
        /// Puts an object into a lazy maybe monad (see <see cref="MaybeLazy{T}"/>)
        /// resulting in a <see cref="Just{T}"/> object.
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="value">The object/value to be wrapped in a maybe-object.</param>
        /// <returns>A maybe-object wrapping the given object.</returns>
        public static MaybeLazy<T> ToMaybeLazy<T>(this T value) => () => new Just<T>(value);

        /// <summary>
        /// Puts an object into a lazy maybe monad (see <see cref="MaybeLazy{T}"/>)
        /// by using a filter function for distinguising between
        /// <see cref="Just{T}"/> and <see cref="Nothing{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="value">The object/value to be wrapped in a maybe-object.</param>
        /// <param name="predicate">The  (filter) function to decide, if the
        /// object is wrapped into <see cref="Nothing{T}"/> or <see cref="Just{T}"/>.</param>
        /// <returns>
        ///   <returns>A <see cref="Just{T}"/> instance, if the predicate function returned <c>true</c>,
        /// otherwise a <see cref="Nothing{T}"/> object.</returns>
        /// </returns>
        public static MaybeLazy<T> ToMaybeLazy<T>(this T value, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            if (predicate(value))
            {
                return () => new Just<T>(value);
            }
            else
            {
                return () => new Nothing<T>();
            }
        }

        /// <summary>
        /// Puts an object into a lazy maybe monad (see <see cref="MaybeLazy{T}"/>)
        /// resulting in a <see cref="Just{T}"/> or <see cref="Just{T}"/> object,
        /// depending on the object being <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="value">The object to be wrapped in a maybe-object.</param>
        /// <returns>A <see cref="Just{T}"/> instance, if the object was not <c>null</c>, otherwise
        /// a <see cref="Nothing{T}"/> object.</returns>
        public static MaybeLazy<T> ToMaybeLazyNotNull<T>(this T value) where T : class
        {
            if ((object)value == null)
            {
                return () => new Nothing<T>();
            }
            else
            {
                return () => new Just<T>(value);
            }
        }

        /// <summary>
        /// Extension method to make the LINQ FROM-notation usable for Maybe[T] for multiple disjunct FROMs
        /// </summary>
        /// <typeparam name="T1">Gekapselter Datentyp des 1. Maybe-Aggregats</typeparam>
        /// <typeparam name="T2">Gekapselter Datentyp des 2. Maybe-Aggregats</typeparam>
        /// <typeparam name="TResult">Gekapselter Datentyp des Ergebnis-Maybe-Aggregats</typeparam>
        /// <param name="a">The maybe aggregate.</param>
        /// <param name="func">The function to bind.</param>
        /// <param name="select">The result flattening selector.</param>
        /// <returns></returns>
        /// <remarks>
        /// Multiple disjunct FROMs means:
        ///		the notation
        ///		<code>
        ///			from a in s1
        ///			from b in s2
        ///			select a + b
        ///		</code>
        ///
        ///		is tanslated to:
        ///		<code>
        ///			s1.SelectMany(a => s2, (s1, b) => a + b)
        ///		</code>
        /// </remarks>
        public static MaybeLazy<TResult> SelectMany<T1, T2, TResult>(this MaybeLazy<T1> a, Func<T1, MaybeLazy<T2>> func, Func<T1, T2, TResult> select)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (select == null) throw new ArgumentNullException(nameof(select));

            return () => MaybeExtensions.Bind(a(), av => MaybeExtensions.Bind(func(av)(), fv => select(av, fv).ToMaybe()));
        }

        /// <summary>
        /// Returns the aggregate if the query function succeeds.
        /// </summary>
        /// <typeparam name="T">Datatype of the maybe aggregate</typeparam>
        /// <param name="obj">The maybe aggregate.</param>
        /// <param name="query">The query (predicate) for keeping the maybe,
        /// rather than returning Nothing[T].</param>
        /// <returns></returns>
        public static MaybeLazy<T> Where<T>(this MaybeLazy<T> obj, Func<T, bool> query)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (query == null) throw new ArgumentNullException(nameof(query));

            return () =>
            {
                if (obj() is Just<T> justa && query(justa.Value))
                {
                    return justa;
                }
                else
                {
                    return new Nothing<T>();
                }
            };
        }

        /// <summary>
        /// Selector function for LINQ support.
        /// Similiar to <see cref="Bind{TSource, TResult}(MaybeLazy{TSource}, Func{TSource, Maybe{TResult}})" /> method,
        /// but here, the selector function does not return an <see cref="MaybeLazy{T}" />-type,
        /// but an arbitrary data type.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">The maybe instance.</param>
        /// <param name="selector">The selector function, returning a value to be encapsluated.</param>
        /// <returns>
        /// The result of the selector function encapsulated in an <see cref="Just{T}" />-instance
        /// or <see cref="Nothing{T}" />, if the given maybe monad was <c>Nothing</c> already.
        /// </returns>
        public static MaybeLazy<TResult> Select<TSource, TResult>(this MaybeLazy<TSource> obj, Func<TSource, TResult> selector)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return () =>
            {
                if (obj() is Just<TSource> justa)
                {
                    return selector(justa.Value).ToMaybe();
                }
                else
                {
                    return new Nothing<TResult>();
                }
            };
        }

        /// <summary>
        /// Binds an aggregate function to a lazy maybe monad.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">The maybe aggregate.</param>
        /// <param name="functionToBind">The function to bind.</param>
        /// <returns>The resulting aggregate (maybe object) after binding the function to the given aggregate.</returns>
        public static MaybeLazy<TResult> Bind<TSource, TResult>(this MaybeLazy<TSource> obj, Func<TSource, Maybe<TResult>> functionToBind)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (functionToBind == null) throw new ArgumentNullException(nameof(functionToBind));

            return () => obj().Bind(functionToBind);
        }

        /// <summary>
        /// Evaluates the lazy monad and executes an action instantly.
        /// </summary>
        /// <typeparam name="T">Type of the aggregate (maybe object)</typeparam>
        /// <param name="obj">The lazy maybe monad.</param>
        /// <param name="actionToApply">The action to apply.</param>
        /// <returns>A strict <see cref="Maybe{T}"/>-object, encapsulating the same value
        /// as the provided lazy one.</returns>
        /// <remarks>
        /// This method may cause side effects with the aggregate value and
        /// does not change the aggregate itself.
        /// </remarks>
        public static Maybe<T> Apply<T>(this MaybeLazy<T> obj, Action<T> actionToApply)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (actionToApply == null) throw new ArgumentNullException(nameof(actionToApply));

            if (obj() is Just<T> justa)
            {
                actionToApply(justa.Value);
            }

            return obj();
        }

        /// <summary>
        /// Creates an <see cref="MaybeLazy{T}"/>-instance, encapsulating
        /// all underlying evaluations, thus ensuring, they are
        /// executed only one time.
        /// </summary>
        /// <typeparam name="T">The encapsulated data typ</typeparam>
        /// <param name="obj">The lazy maybe monad</param>
        /// <returns></returns>
        public static MaybeLazy<T> Memoize<T>(this MaybeLazy<T> obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return () => new Lazy<Maybe<T>>(() => obj()).Value;
        }

        /// <summary>
        /// Evaluates the lazy Maybe monad instantly and performs an action accordingly,
        /// if the evaluated value matches a specified filter criterion.
        /// Then the delayed Maybe will not be returned to re-run
        /// to avoid the chain of recombination.
        /// </summary>
        /// <typeparam name="T">Type of the aggregate (maybe object)</typeparam>
        /// <param name="obj">The aggregate.</param>
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
        public static Maybe<T> Apply<T>(this MaybeLazy<T> obj, Action<T> actionToApply, Func<T, bool> predicate)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (actionToApply == null) throw new ArgumentNullException(nameof(actionToApply));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            if (obj() is Just<T> justa && predicate(justa.Value))
            {
                actionToApply(justa.Value);
            }

            return obj();
        }

        /// <summary>
        /// Evaluates the lazy maybe monad and returns, if it actually contains a value.
        /// </summary>
        /// <typeparam name="T">The type of the object-type</typeparam>
        /// <param name="obj">The maybe-object to check.</param>
        /// <returns><c>true</c>, if the maybe-object contains a value. <seealso cref="Just{T}"/></returns>
        public static bool HasValue<T>(this MaybeLazy<T> obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return obj() as Just<T> != null;
        }

        /// <summary>
        /// Evaluates the lazy maybe monad and returns, if it does not contain a value.
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The maybe-object to check.</param>
        /// <returns><c>true</c>, if the maybe-object contains no value. <seealso cref="Nothing{T}"/></returns>
        /// <remarks>
        /// Do not confuse <c>null</c> and <see cref="Nothing{T}"/>!
        /// This method also returns <c>true</c>, if a maybe's value is <c>null</c>,
        /// since this is a valid value for objects.
        /// </remarks>
        public static bool IsNothing<T>(this MaybeLazy<T> obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return obj() as Nothing<T> != null;
        }

        /// <summary>
        /// Evaluates the lazy maybe monad and returns its value or
        /// and defaqult value, depending on it encapsulating a
        /// <see cref="Just{T}"/>- or <see cref="Nothing{T}"/>-object.
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The maybe-object.</param>
        /// <param name="defaultValue">The default value to return, if the maybe object contains no value.</param>
        /// <returns>The value of the maybe-object, if existing, or the given default value otherwise.</returns>
        public static T GetValue<T>(this MaybeLazy<T> obj, T defaultValue)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return obj() is Just<T> just ? just.Value : defaultValue;
        }

        /// <summary>
        /// Returns an new lazy maybe monad, resulting in an alternative maybe monad,
        /// if the current would evaluate to <see cref="Nothing{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The maybe-object.</param>
        /// <param name="defaultMaybe">The default maybe-object, if the given maybe-object is empty.</param>
        /// <returns>The original maybe-object, if it contains a value; otherwise the given default maybe-object.</returns>
        public static MaybeLazy<T> Otherwise<T>(this MaybeLazy<T> obj, MaybeLazy<T> defaultMaybe)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (defaultMaybe == null) throw new ArgumentNullException(nameof(defaultMaybe));

            return () => obj() is Just<T> just ? just : defaultMaybe();
        }

        /// <summary>
        /// Returns a lazy maybe monad, that will throw an exception,
        /// if it results in <see cref="Nothing{T}"/> when evaluated.
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The maybe-object.</param>
        /// <param name="exceptionToThrow">The exception to throw.</param>
        /// <returns>The original maybe-aggregate, if it contains a value</returns>
        public static MaybeLazy<T> OtherwiseThrow<T>(this MaybeLazy<T> obj, Exception exceptionToThrow)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (exceptionToThrow == null) throw new ArgumentNullException(nameof(exceptionToThrow));

            return () =>
            {
                if (obj() is Just<T> just)
                {
                    return just;
                }

                throw exceptionToThrow;
            };
        }

        /// <summary>
        /// Returns a new lazy maybe monad, encapsulating a <see cref="bool"/>-value,
        /// that reflects, if the monad results in <see cref="Just{T}"/>, when evaluated.
        /// </summary>
        /// <typeparam name="T">The type of the maybe-object.</typeparam>
        /// <param name="obj">The maybe-object.</param>
        /// <param name="predicate">The predicate to execute with the maybe-object's value.</param>
        /// <returns>
        /// A maybe-object, containing the boolean result of the given predicate, if the maybe-object contains a value;
        /// otherwise an empty maybe-value.
        /// </returns>
        public static MaybeLazy<bool> Satisfies<T>(this MaybeLazy<T> obj, Func<T, bool> predicate)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return () => obj() is Just<T> just ? new Just<bool>(predicate(just.Value)) : (Maybe<bool>)new Nothing<bool>();
        }

        /// <summary>
        /// Version of Map (LINQ Select), applying a mapping function, which returns a maybe monad,
        /// to an <see cref="IEnumerable{T}"/> and only returns values of <see cref="Just{T}"/>-instances,
        /// thus leaving out <see cref="Nothing{T}"/>-elements.
        /// </summary>
        /// </summary>
        /// <typeparam name="TSource">The type of the objects within the source enumerator.</typeparam>
        /// <typeparam name="TResult">The type of the objects within the result enumerator.</typeparam>
        /// <param name="source">The source enumerator.</param>
        /// <param name="selector">The result selector function.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> MapMaybeLazy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, MaybeLazy<TResult>> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return source.Select(p => selector(p)())
                .OfType<Just<TResult>>()
                .Select(p => p.Value);
        }

        /// <summary>
        /// Evaluates the monad and returns the encapsulated value,
        /// as an enumerator, returning nothing for a <see cref="Nothing{T}"/>-instance
        /// and the encapsulated value otherwise.</summary>
        /// <typeparam name="T">The encapsulated data type.</typeparam>
        /// <param name="obj">The maybe instance.</param>
        /// <returns></returns>
        public static IEnumerable<T> MaybeToList<T>(this MaybeLazy<T> obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            if (obj() is Just<T> just)
            {
                yield return just.Value;
            }
            else
            {
                yield break;
            }
        }

        /// <summary>
        /// Returns for an enumerator of lazy maybe instances the
        /// actual encapsulated values inside an enumerator,
        /// leaving out <see cref="Nothing{T}"/>-instances.
        /// </summary>
        /// <remarks>
        /// All lazy maybes in the enumerator will be evaluated.
        /// </remarks>
        /// <typeparam name="T">Data type, encapsulated by the maybe-monad in the source enumerator.</typeparam>
        /// <param name="source">The source enumerator.</param>
        /// <returns></returns>
        public static IEnumerable<T> CatMaybes<T>(this IEnumerable<MaybeLazy<T>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.Select(p => p())
                .OfType<Just<T>>()
                .Select(p => p.Value);
        }

        /// <summary>
        /// Returns a new lazy maybe monad encapsulating the first element returned by the enumerator
        /// or <see cref="Nothing{T}"/> for an empty enumerator.
        /// <typeparam name="T">Der Elemententyp</typeparam>
        /// <param name="source">Die Quellauflistung.</param>
        /// <returns></returns>
        public static MaybeLazy<T> FirstToMaybeLazy<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return () => source.Any() ? (Maybe<T>)new Just<T>(source.First()) : new Nothing<T>();
        }

        /// <summary>
        /// Returns a new lazy maybe monad encapsulating the first element of the enumerator
        /// that matches a given filter or <see cref="Nothing{T}"/> for an enumerator with
        /// no matching elements.
        /// </summary>
        /// <typeparam name="T">Der Elemententyp</typeparam>
        /// <param name="source">Die Quellauflistung.</param>
        /// <returns></returns>
        public static MaybeLazy<T> FirstToMaybeLazy<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return () =>
            {
                var results = source.Where(predicate)
                    .Take(1)
                    .ToList();

                return results.Any() ? (Maybe<T>)new Just<T>(results.First()) : new Nothing<T>();
            };
        }
    }
}