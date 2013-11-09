using System;
using System.Diagnostics.Contracts;

namespace Interlook.Eventing
{
	/// <summary>
	/// Interface, that provides additional methods for subscribing to events on an event bus, that provide support for
	/// weak references, so registered handlers arent be blocked from garbage collection.
	/// </summary>
	[ContractClass(typeof(IEventSourceExContract))]
	public interface IEventSourceEx : IEventSource
	{
		IUnsubscribeToken Subscribe<TEvent>(Action<TEvent> eventHandlerCallback, bool useWeakReference) where TEvent : IEvent;

		IUnsubscribeToken Subscribe<TEvent>(Action<TEvent> eventHandlerCallback, Func<TEvent, bool> filterPredicate, bool useWeakReferences) where TEvent : IEvent;

		IUnsubscribeToken RegisterHandlerFor<TEvent>(IEventHandlerFor<TEvent> handler, bool useWeakReference) where TEvent : IEvent;
	}

	[ContractClassFor(typeof(IEventSourceEx))]
	internal abstract class IEventSourceExContract : IEventSourceEx
	{

		public IUnsubscribeToken Subscribe<TEvent>(Action<TEvent> eventHandlerCallback, bool useWeakReference) where TEvent : IEvent
		{
			Contract.Requires<ArgumentNullException>(eventHandlerCallback != null, "eventHandlerCallback");

			throw new NotImplementedException();
		}

		public IUnsubscribeToken Subscribe<TEvent>(Action<TEvent> eventHandlerCallback, Func<TEvent, bool> filterPredicate, bool useWeakReferences) where TEvent : IEvent
		{
			Contract.Requires<ArgumentNullException>(eventHandlerCallback != null, "eventHandlerCallback");
			Contract.Requires<ArgumentNullException>(filterPredicate != null, "filterPredicate");

			throw new NotImplementedException();
		}

		public IUnsubscribeToken RegisterHandlerFor<TEvent>(IEventHandlerFor<TEvent> handler, bool useWeakReference) where TEvent : IEvent
		{
			Contract.Requires<ArgumentNullException>(handler != null, "handler");

			throw new NotImplementedException();
		}

		public IUnsubscribeToken Subscribe<TEvent>(Action<TEvent> eventHandlerCallback) where TEvent : IEvent
		{
			throw new NotImplementedException();
		}

		public IUnsubscribeToken Subscribe<TEvent>(Action<TEvent> eventHandlerCallback, Func<TEvent, bool> filterPredicate) where TEvent : IEvent
		{
			throw new NotImplementedException();
		}

		public IUnsubscribeToken RegisterHandlerFor<TEvent>(IEventHandlerFor<TEvent> handler) where TEvent : IEvent
		{
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