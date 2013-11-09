using System;

namespace Interlook.Eventing
{
	/// <summary>
	/// Generic Interface for event handlers.
	/// </summary>
	public interface IEventHandlerFor<TEvent> where TEvent : IEvent
	{
		void Handle(TEvent ev);

		bool CanHandle(TEvent ev);
	}
}