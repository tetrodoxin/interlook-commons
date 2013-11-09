using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Interlook.Components
{
	/// <summary>
	/// Variant of <see cref="DelegateComparer{T}"/>, which only uses the equality delegate,
	/// if both arguments are not null (otherwise <c>false</c> is returned) 
	/// and are no equal references (results in <c>true</c>).
	/// Furthermore the hashing delegate is only called for non null objects and
	/// zero returned instead.
	/// </summary>
	/// <typeparam name="T">Type of the objects, that are to be compared.</typeparam>
	public class ConcreteDelegateComparer<T> : DelegateComparer<T>
	{
		public ConcreteDelegateComparer(Func<T, T, bool> equalsFunc, Func<T, int> hashFunc)
			: base(equalsFunc, hashFunc)
		{
			Contract.Requires<ArgumentNullException>(hashFunc != null, "hashFunc");
			Contract.Requires<ArgumentNullException>(equalsFunc != null, "equalsFunc");
		}

		public override bool Equals(T x, T y)
		{
			if ((object)x == null || (object)y == null)
			{
				return false;
			}

			if (ReferenceEquals(x, y))
			{
				return true;
			}
			
			return base.Equals(x, y);
		}
	}
}