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
using System.Reflection;

namespace Interlook.Eventing
{
    public class EventSubscription<TEvent> : EventSubscriptionBase<TEvent>
        where TEvent : IEvent
    {
        #region Fields

        private bool _filterSet = false;
        private readonly Action<TEvent> _directAction = null;
        private readonly Func<TEvent, bool> _directPredicate = null;

        private readonly MethodInfo _actionMethod;
        private readonly WeakReference _weakReferenceAction;
        private readonly MethodInfo _predicateMethod;
        private readonly WeakReference _weakReferenceFilter;

        #endregion Fields

        internal Action<TEvent> ActionDelegate
        {
            get { return tryGetAction(); }
        }

        #region Constructors

        public EventSubscription(Action<TEvent> action, Action<IUnsubscribeToken> unsubscribeAction)
            : this(action, null, false, unsubscribeAction)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (unsubscribeAction == null) throw new ArgumentNullException(nameof(unsubscribeAction));
        }

        public EventSubscription(Action<TEvent> action, bool useWeakReference, Action<IUnsubscribeToken> unsubscribeAction)
            : this(action, null, useWeakReference, unsubscribeAction)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (unsubscribeAction == null) throw new ArgumentNullException(nameof(unsubscribeAction));
        }

        public EventSubscription(Action<TEvent> action, Func<TEvent, bool> filterPredicate, bool useWeakReferences, Action<IUnsubscribeToken> unsubscribeAction)
            : base(unsubscribeAction)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (unsubscribeAction == null) throw new ArgumentNullException(nameof(unsubscribeAction));

            if (useWeakReferences)
            {
                this._weakReferenceAction = new WeakReference(action.Target);
                this._actionMethod = action.Method;
                if (filterPredicate != null)
                {
                    this._weakReferenceFilter = new WeakReference(filterPredicate.Target);
                    this._predicateMethod = filterPredicate.Method;
                    this._filterSet = true;
                }
            }
            else
            {
                this._directAction = action;
                if (filterPredicate != null)
                {
                    this._directPredicate = filterPredicate;
                    this._filterSet = true;
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
            if (_directAction != null)
            {
                return _directAction;
            }
            else
            {
                return tryGetDelegate(this._actionMethod, this._weakReferenceAction, typeof(Action<TEvent>)) as Action<TEvent>;
            }
        }

        private Func<TEvent, bool> tryGetPredicate()
        {
            if (_directPredicate != null)
            {
                return _directPredicate;
            }
            else
            {
                return tryGetDelegate(this._predicateMethod, this._weakReferenceFilter, typeof(Func<TEvent, bool>)) as Func<TEvent, bool>;
            }
        }

        private bool applyFilter(TEvent ev)
        {
            if (!_filterSet)
            {
                return true;
            }

            var pred = tryGetPredicate();
            if (pred == null)
            {
                this._filterSet = false;
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