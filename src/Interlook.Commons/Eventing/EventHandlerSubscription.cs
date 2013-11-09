using System;
using System.Diagnostics.Contracts;

namespace Interlook.Eventing
{
	public class EventHandlerSubscription<TEvent> : EventSubscriptionBase<TEvent>
		where TEvent : IEvent
	{
		#region Fields

		private readonly WeakReference weakReference;
		private readonly IEventHandlerFor<TEvent> directHandler = null;
		private bool wasLost = false;

		#endregion Fields

		#region Constructors

		public EventHandlerSubscription(IEventHandlerFor<TEvent> eventHandler, bool useWeakReference, Action<IUnsubscribeToken> unsubscribeAction)
			: base(unsubscribeAction)
		{
			Contract.Requires<ArgumentNullException>(eventHandler != null, "eventHandler");
			Contract.Requires<ArgumentNullException>(unsubscribeAction != null, "unsubscribeAction");

			if (useWeakReference)
			{
				this.weakReference = new WeakReference(eventHandler);
			}
			else
			{
				this.directHandler = eventHandler;
			}
		}

		#endregion Constructors

		#region Public Methods

		public override bool CanHandle(TEvent ev)
		{
			var handler = GetHandler();
			if (handler != null)
			{
				return handler.CanHandle(ev);
			}
			else
			{
				return false;
			}
		}

		public override void Handle(TEvent ev)
		{
			var handler = GetHandler();
			if (handler != null)
			{
				handler.Handle(ev);
			}
		}

		#endregion Public Methods

		#region Private Methods

		internal IEventHandlerFor<TEvent> GetHandler()
		{
			if (wasLost)
			{
				return null;
			}
			if (directHandler != null)
			{
				return directHandler;
			}
			else
			{
				var r = this.weakReference.Target as IEventHandlerFor<TEvent>;
				if (r == null || !this.weakReference.IsAlive)
				{
					wasLost = true;
					this.Unsubscribe();
				}

				return r;
			}
		}

		#endregion Private Methods
	}
}