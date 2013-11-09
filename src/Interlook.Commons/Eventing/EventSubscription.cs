using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Interlook.Eventing
{
	public class EventSubscription<TEvent> : EventSubscriptionBase<TEvent>
		where TEvent : IEvent
	{
		#region Fields

		private bool filterSet = false;
		private readonly Action<TEvent> directAction = null;
		private readonly Func<TEvent, bool> directPredicate = null;

		private readonly MethodInfo actionMethod;
		private readonly WeakReference weakReferenceAction;
		private readonly MethodInfo predicateMethod;
		private readonly WeakReference weakReferenceFilter;

		#endregion Fields

		internal Action<TEvent> ActionDelegate
		{
			get { return tryGetAction(); }
		}

		#region Constructors

		public EventSubscription(Action<TEvent> action, Action<IUnsubscribeToken> unsubscribeAction)
			: this(action, null, false, unsubscribeAction)
		{
			Contract.Requires<ArgumentNullException>(action != null, "action");
			Contract.Requires<ArgumentNullException>(unsubscribeAction != null, "unsubsribeAction");
		}

		public EventSubscription(Action<TEvent> action, bool useWeakReference, Action<IUnsubscribeToken> unsubscribeAction)
			: this(action, null, useWeakReference, unsubscribeAction)
		{
			Contract.Requires<ArgumentNullException>(action != null, "action");
			Contract.Requires<ArgumentNullException>(unsubscribeAction != null, "unsubsribeAction");
		}

		public EventSubscription(Action<TEvent> action, Func<TEvent, bool> filterPredicate, bool useWeakReferences, Action<IUnsubscribeToken> unsubscribeAction)
			: base(unsubscribeAction)
		{
			Contract.Requires<ArgumentNullException>(action != null, "action");
			Contract.Requires<ArgumentNullException>(unsubscribeAction != null, "unsubsribeAction");

			if (useWeakReferences)
			{
				this.weakReferenceAction = new WeakReference(action.Target);
				this.actionMethod = action.Method;
				if (filterPredicate != null)
				{
					this.weakReferenceFilter = new WeakReference(filterPredicate.Target);
					this.predicateMethod = filterPredicate.Method;
					this.filterSet = true;
				}
			}
			else
			{
				this.directAction = action;
				if (filterPredicate != null)
				{
					this.directPredicate = filterPredicate;
					this.filterSet = true;
				}
			}
		}

		#endregion Constructors

		#region Public Methods

		public override void Handle(TEvent ev)
		{
			if (ev != null)
			{
				var action = tryGetAction();
				if (action == null)
				{
					Unsubscribe();
				}
				else
				{
					action(ev);
				}
			}
		}

		public override bool CanHandle(TEvent ev)
		{
			if (ev != null)
			{
				return applyFilter(ev);
			}
			else
			{
				return false;
			}
		}

		#endregion Public Methods

		#region Private Methods

		private Action<TEvent> tryGetAction()
		{
			if (directAction != null)
			{
				return directAction;
			}
			else
			{
				return tryGetDelegate(this.actionMethod, this.weakReferenceAction, typeof(Action<TEvent>)) as Action<TEvent>;
			}
		}

		private Func<TEvent, bool> tryGetPredicate()
		{
			if (directPredicate != null)
			{
				return directPredicate;
			}
			else
			{
				return tryGetDelegate(this.predicateMethod, this.weakReferenceFilter, typeof(Func<TEvent, bool>)) as Func<TEvent, bool>;
			}
		}

		private bool applyFilter(TEvent ev)
		{
			if (!filterSet)
			{
				return true;
			}

			var pred = tryGetPredicate();
			if (pred == null)
			{
				this.filterSet = false;
				return true;
			}
			else
			{
				return pred(ev);
			}
		}

		private static Delegate tryGetDelegate(MethodInfo method, WeakReference reference, Type typeOfDelegate)
		{
			if (method.IsStatic)
			{
				return Delegate.CreateDelegate(typeOfDelegate, null, method);
			}

			object target = reference.Target;
			if (target != null && reference.IsAlive)
			{
				return Delegate.CreateDelegate(typeOfDelegate, target, method);
			}

			return null;
		}

		#endregion Private Methods
	}
}