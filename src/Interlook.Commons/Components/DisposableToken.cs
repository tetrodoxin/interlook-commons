using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Interlook.Components
{
	public abstract class DisposableToken : IDisposable, IEquatable<DisposableToken>, IEqualityComparer<DisposableToken>
	{
		private readonly Guid id;

		public DisposableToken()
		{
			this.id = Guid.NewGuid();
		}

		public virtual void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		public static bool Equals(DisposableToken x, DisposableToken y)
		{
			if ((object)x == null || (object)y == null)
			{
				return false;
			}

			if (ReferenceEquals(x, y))
			{
				return true;
			}

			return Guid.Equals(x.id, y.id);
		}

		public static int GetHashCode(DisposableToken obj)
		{
			if ((object)obj == null)
			{
				return 0;
			}
			else
			{
				return obj.id.GetHashCode();
			}
		}

		public override bool Equals(object obj)
		{
			return Equals(this, obj as DisposableToken);
		}

		public override int GetHashCode()
		{
			return GetHashCode(this);
		}

		public bool Equals(DisposableToken other)
		{
			return Equals(this, other);
		}

		bool IEqualityComparer<DisposableToken>.Equals(DisposableToken x, DisposableToken y)
		{
			return Equals(x, y);
		}

		int IEqualityComparer<DisposableToken>.GetHashCode(DisposableToken obj)
		{
			return GetHashCode(this);
		}
	}
}