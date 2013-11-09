using System;

namespace Interlook.Eventing
{
	/// <summary>
	/// Interface to support unsubscribing of events.
	/// </summary>
	internal class UnsubscribeToken : IUnsubscribeToken, IEquatable<UnsubscribeToken>
	{
		private Guid id;
		private Action<IUnsubscribeToken> unsubscribeAction;

		public UnsubscribeToken(Action<IUnsubscribeToken> unsubscribeAction)
		{
			this.unsubscribeAction = unsubscribeAction;
			id = Guid.NewGuid();
		}

		public bool Equals(UnsubscribeToken token)
		{
			return (object)token != null && (ReferenceEquals(token, this) || ReferenceEquals(token.id, this.id));
		}

		public bool Equals(IUnsubscribeToken token)
		{
			return this.Equals(token as UnsubscribeToken);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as UnsubscribeToken);
		}

		public override int GetHashCode()
		{
			return this.id.GetHashCode();
		}

		public void Dispose()
		{
			Unsubscribe();
			GC.SuppressFinalize(this);
		}

		internal void Unsubscribe()
		{
			if (this.unsubscribeAction != null)
			{
				this.unsubscribeAction(this);
				this.unsubscribeAction = null;
			}
		}
	}
}