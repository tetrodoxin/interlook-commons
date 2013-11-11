using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Interlook.Components
{
	/// <summary>
	/// Type that encapsulates an optional value.
	/// </summary>
	/// <typeparam name="T">Type of the encapsulated value.</typeparam>
	public abstract class Maybe<T> : IEquatable<Maybe<T>>
	{
		protected const string NOTHING_CAP1 = "Nothing";
		protected const string NOTHING_LOWER = "nothing";
		protected const string NULL_AS_STRING = "null";

		protected abstract int CalculateHashCode();

		protected abstract bool IsEqualToInstance(Maybe<T> obj);

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
	}

	/// <summary>
	/// Implements an empty <see cref="Maybe{T}"/>.
	/// </summary>
	/// <typeparam name="T">Type of the value of the implemented maybe.</typeparam>
	public sealed class Nothing<T> : Maybe<T>
	{
		private static readonly Lazy<Nothing<T>> _instance = new Lazy<Nothing<T>>(() => new Nothing<T>());

		/// <summary>
		/// A default empty maybe.
		/// </summary>
		public static Nothing<T> Instance
		{
			get { return _instance.Value; }
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return NOTHING_CAP1 + ".";
		}

		protected override int CalculateHashCode()
		{
			return 0;
		}

		protected override bool IsEqualToInstance(Maybe<T> obj)
		{
			return obj is Nothing<T>;
		}
	}

	/// <summary>
	/// Implements a <see cref="Maybe{T}"/>-object with an existing value.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	public class Just<T> : Maybe<T>
	{
		private bool _isNull;
		private bool _implementsEquatable;

		/// <summary>
		/// Gets the actual value.
		/// </summary>
		/// <remarks>
		/// The value may be <c>null</c> for reference types, since this is a possible value for objects.
		/// Do not confuse <c>null</c> and <see cref="Nothing{T}"/>!
		/// </remarks>
		public T Value { get; private set; }

		/// <summary>
		/// Creates a maybe-type with a set value.
		/// </summary>
		/// <param name="value">The value.</param>
		public Just(T value)
		{
			this.Value = value;
			_isNull = (object.Equals(null, Value));
			_implementsEquatable = typeof(IEquatable<T>).IsAssignableFrom(typeof(T));
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
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

		protected override bool IsEqualToInstance(Maybe<T> obj)
		{
			return Equals(this, obj as Just<T>);
		}

		/// <summary>
		/// Determines, if two valued maybe-object are equal.
		/// </summary>
		/// <param name="x">The first maybe-object to match.</param>
		/// <param name="y">The second maybe-object to match.</param>
		/// <returns><c>true</c>, if both arguments aren't <c>null</c> and wrap the same value.</returns>
		public static bool Equals(Just<T> x, Just<T> y)
		{
			if ((object)x == null || (object)y == null)
			{
				return object.Equals(x, y);
			}

			if (ReferenceEquals(x, y))
			{
				return true;
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
	}

	/// <summary>
	/// Extension methods for <see cref="Maybe{T}"/> types.
	/// </summary>
	public static class MaybeExtensions
	{
		/// <summary>
		/// Wraps the object/value into a maybe-object.
		/// </summary>
		/// <typeparam name="T">The type of the maybe-object.</typeparam>
		/// <param name="value">The object/value to be wrapped in a maybe-object.</param>
		/// <returns>A maybe-object wrapping the given object.</returns>
		public static Maybe<T> ToMaybe<T>(this T value)
		{
			return new Just<T>(value);
		}

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
		/// Returns another maybe-object, if the maybe-object is empty (<see cref="Nothing{T}"/>)
		/// </summary>
		/// <typeparam name="T">The type of the maybe-object.</typeparam>
		/// <param name="obj">The maybe-object.</param>
		/// <param name="defaultMaybe">The default maybe-object, if the given maybe-object is empty.</param>
		/// <returns>The original maybe-object, if it contains a value; otherwise the given default maybe-object.</returns>
		public static Maybe<T> Otherwise<T>(this Maybe<T> obj, Maybe<T> defaultMaybe)
		{
			Contract.Requires<ArgumentNullException>(defaultMaybe != null, "maybeDefaultValue");

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
	}
}