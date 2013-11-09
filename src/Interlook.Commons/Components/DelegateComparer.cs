using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Interlook.Components
{
	/// <summary>
	/// Implementation of the <see cref="IEqualityComparer{T}"/> interface for comparers,
	/// that are defined via delegate functions.
	/// </summary>
	/// <typeparam name="T">Type of the objects, that are to be compared.</typeparam>
	public class DelegateComparer<T> : IEqualityComparer<T>
	{
		private Func<T, int> hashFunc;
		private Func<T, T, bool> equalsFunc;

		public DelegateComparer(Func<T, T, bool> equalsFunc, Func<T, int> hashFunc)
		{
			Contract.Requires<ArgumentNullException>(hashFunc != null, "hashFunc");
			Contract.Requires<ArgumentNullException>(equalsFunc != null, "equalsFunc");

			this.hashFunc = hashFunc;
			this.equalsFunc = equalsFunc;
		}

		public virtual bool Equals(T x, T y)
		{
			return equalsFunc(x, y);
		}

		public virtual int GetHashCode(T obj)
		{
			return hashFunc(obj);
		}
	}
}
