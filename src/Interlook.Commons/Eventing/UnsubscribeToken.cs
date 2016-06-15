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