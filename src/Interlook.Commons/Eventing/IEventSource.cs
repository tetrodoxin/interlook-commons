using System;
using System.Diagnostics.Contracts;

namespace Interlook.Eventing
{
	/// <summary>
	/// Interface, that provides methods for subscribing to events on an event bus.
	/// </summary>
	[ContractClass(typeof(IEventSourceContract))]
	public interface IEventSource
	{
		IUnsubscribeToken Subscribe<TEvent>(Action<TEvent> eventHandlerCallback) where TEvent : IEvent;

		IUnsubscribeToken Subscribe<TEvent>(Action<TEvent> eventHandlerCallback, Func<TEvent, bool> filterPredicate) where TEvent : IEvent;

		IUnsubscribeToken RegisterHandlerFor<TEvent>(IEventHandlerFor<TEvent> handler) where TEvent : IEvent;

		void Unsubscribe<TEvent>(Action<TEvent> eventHandlerCallback) where TEvent : IEvent;

		void Unsubscribe(IUnsubscribeToken token);

		void UnregisterHandlerFor<TEvent>(IEventHandlerFor<TEvent> handler) where TEvent : IEvent;
	}

	[ContractClassFor(typeof(IEventSource))]
	internal abstract class IEventSourceContract : IEventSource
	{

		public IUnsubscribeToken Subscribe<TEvent>(Action<TEvent> eventHandlerCallback) where TEvent : IEvent
		{
			Contract.Requires<ArgumentNullException>(eventHandlerCallback != null, "eventHandlerCallback");

			throw new NotImplementedException();
		}

		public IUnsubscribeToken Subscribe<TEvent>(Action<TEvent> eventHandlerCallback, Func<TEvent, bool> filterPredicate) where TEvent : IEvent
		{
			Contract.Requires<ArgumentNullException>(eventHandlerCallback != null, "eventHandlerCallback");
			Contract.Requires<ArgumentNullException>(filterPredicate != null, "filterPredicate");

			throw new NotImplementedException();
		}

		public IUnsubscribeToken RegisterHandlerFor<TEvent>(IEventHandlerFor<TEvent> handler) where TEvent : IEvent
		{
			Contract.Requires<ArgumentNullException>(handler != null, "handler");

			throw new NotImplementedException();
		}

		public void Unsubscribe<TEvent>(Action<TEvent> eventHandlerCallback) where TEvent : IEvent
		{
			throw new NotImplementedException();
		}

		public void Unsubscribe(IUnsubscribeToken token)
		{
			throw new NotImplementedException();
		}

		public void UnregisterHandlerFor<TEvent>(IEventHandlerFor<TEvent> handler) where TEvent : IEvent
		{
			throw new NotImplementedException();
		}
	}
}