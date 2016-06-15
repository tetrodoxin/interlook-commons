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