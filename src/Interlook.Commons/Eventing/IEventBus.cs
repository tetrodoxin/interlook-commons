using System;

namespace Interlook.Eventing
{
	/// <summary>
	/// Interface, that provides methods for publishing to events on an event bus.
	/// </summary>
	public interface IEventBus : IEventSource
	{
		/// <summary>
		/// Publishes an event on the event bus.
		/// </summary>
		/// <typeparam name="TEvent">Type of the event, that is to be published.</typeparam>
		/// <param name="ev">The actual event object to publish.</param>
		void Publish<TEvent>(TEvent ev) where TEvent : IEvent; 
	}
}