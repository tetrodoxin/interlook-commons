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