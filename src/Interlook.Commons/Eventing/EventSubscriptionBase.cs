﻿#region license

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
	public abstract class EventSubscriptionBase<TEvent> : IEventSubscription, IEventHandlerFor<TEvent>
		where TEvent : IEvent
	{
		#region Fields

		#endregion Fields

		#region Properties

		internal UnsubscribeToken UnsubscribeTokenInternal { get; private set; }

		public IUnsubscribeToken UnsubscribeToken
		{
			get { return UnsubscribeTokenInternal; }
		}

		#endregion Properties

		#region Constructors

		protected EventSubscriptionBase(Action<IUnsubscribeToken> unsubscribeAction)
		{
			this.UnsubscribeTokenInternal = new UnsubscribeToken(unsubscribeAction);
		}

		#endregion Constructors

		#region Public Methods

		public abstract void Handle(TEvent ev);

		public abstract bool CanHandle(TEvent ev);

		public virtual bool IsForExactType(Type eventType)
		{
			return eventType != null && typeof(TEvent).Equals(eventType);
		}

		#endregion Public Methods

		#region Protected Methods

		protected void Unsubscribe()
		{
			this.UnsubscribeTokenInternal.Unsubscribe();
		}

		#endregion Protected Methods

		#region IEventSubscription Methods

		bool IEventSubscription.CanHandle(IEvent ev)
		{
			if (ev is TEvent)
			{
				return this.CanHandle((TEvent)ev);
			}
			else
			{
				return false;
			}
		}

		void IEventSubscription.Handle(IEvent ev)
		{
			if (ev is TEvent)
			{
				this.Handle((TEvent)ev);
			}
		}

		#endregion IEventSubscription Methods
	}
}