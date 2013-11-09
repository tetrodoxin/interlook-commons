using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interlook.Eventing
{
	/// <summary>
	/// Base Interface for event subscriptions.
	/// </summary>
	public interface IEventSubscription
	{
		void Handle(IEvent ev);

		bool CanHandle(IEvent ev);

		bool IsForExactType(Type eventType);
	}
}
