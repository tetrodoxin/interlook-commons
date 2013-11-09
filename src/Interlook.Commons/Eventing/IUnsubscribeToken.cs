using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interlook.Eventing
{
	/// <summary>
	/// Interface to support unsubscribing of events.
	/// </summary>
	public interface IUnsubscribeToken : IDisposable
	{
		bool Equals(IUnsubscribeToken token);
	}
}
