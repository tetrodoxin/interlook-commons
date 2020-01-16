#region license

//MIT License

//Copyright(c) 2013-2020 Andreas Hübner

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
using System.Collections.Generic;
using System.Linq;

namespace Interlook.Eventing
{
	/// <summary>
	/// Default implementation of the <see cref="IEventBus"/> and <see cref="IEventBusEx"/> interfaces.
	/// </summary>
	public class EventBus : IEventBusEx
	{
		private Dictionary<IUnsubscribeToken, IEventSubscription> subscriptions = new Dictionary<IUnsubscribeToken, IEventSubscription>();

		public virtual void Publish<TEvent>(TEvent ev) where TEvent : IEvent
		{
			var currentSubscriptions = subscriptions.ToList();

			var handlersForThisRun = currentSubscriptions
				.Where(p => p.Value.CanHandle(ev))
				.ToList();

			foreach (var z in handlersForThisRun)
			{
				z.Value.Handle(ev);
			}
		}

		public virtual IUnsubscribeToken Subscribe<TEvent>(Action<TEvent> eventHandlerCallback) where TEvent : IEvent
		{
			return Subscribe<TEvent>(eventHandlerCallback, false);
		}

		public virtual IUnsubscribeToken Subscribe<TEvent>(Action<TEvent> eventHandlerCallback, Func<TEvent, bool> filterPredicate) where TEvent : IEvent
		{
			return Subscribe<TEvent>(eventHandlerCallback, filterPredicate, false);
		}

		public virtual IUnsubscribeToken RegisterHandlerFor<TEvent>(IEventHandlerFor<TEvent> handler) where TEvent : IEvent
		{
			return RegisterHandlerFor<TEvent>(handler, false);
		}

		public virtual void Unsubscribe<TEvent>(Action<TEvent> eventHandlerCallback) where TEvent : IEvent
		{
			var found = this.subscriptions
				.Where(p => p.Value is EventSubscription<TEvent>)
				.Where(p => ((EventSubscription<TEvent>)p.Value).ActionDelegate == eventHandlerCallback);

			if (found.Any())
			{
				this.subscriptions.Remove(found.First().Key);
			}
		}

		public virtual void Unsubscribe(IUnsubscribeToken token)
		{
			this.subscriptions.Remove(token);
		}

		public virtual void UnregisterHandlerFor<TEvent>(IEventHandlerFor<TEvent> handler) where TEvent : IEvent
		{
			var found = this.subscriptions
				.Where(p => p.Value is EventHandlerSubscription<TEvent>)
				.Where(p => ((EventHandlerSubscription<TEvent>)p.Value).GetHandler() == handler);

			if (found.Any())
			{
				this.subscriptions.Remove(found.First().Key);
			}
		}

		public virtual IUnsubscribeToken Subscribe<TEvent>(Action<TEvent> eventHandlerCallback, bool useWeakReference) where TEvent : IEvent
		{
			var subscription = new EventSubscription<TEvent>(eventHandlerCallback, useWeakReference, this.Unsubscribe);
			subscriptions.Add(subscription.UnsubscribeToken, subscription);
			return subscription.UnsubscribeToken;
		}

		public virtual IUnsubscribeToken Subscribe<TEvent>(Action<TEvent> eventHandlerCallback, Func<TEvent, bool> filterPredicate, bool useWeakReferences) where TEvent : IEvent
		{
			var subscription = new EventSubscription<TEvent>(eventHandlerCallback, filterPredicate, useWeakReferences, this.Unsubscribe);
			subscriptions.Add(subscription.UnsubscribeToken, subscription);
			return subscription.UnsubscribeToken;
		}

		public virtual IUnsubscribeToken RegisterHandlerFor<TEvent>(IEventHandlerFor<TEvent> handler, bool useWeakReference) where TEvent : IEvent
		{
			var subscription = new EventHandlerSubscription<TEvent>(handler, useWeakReference, this.Unsubscribe);
			subscriptions.Add(subscription.UnsubscribeToken, subscription);
			return subscription.UnsubscribeToken;
		}
	}
}