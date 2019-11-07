#region license

//MIT License

//Copyright(c) 2013-2019 Andreas Hübner

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
    public class EventHandlerSubscription<TEvent> : EventSubscriptionBase<TEvent>
        where TEvent : IEvent
    {
        #region Fields

        private readonly WeakReference _weakReference;
        private readonly IEventHandlerFor<TEvent> _directHandler = null;
        private bool _wasLost = false;

        #endregion Fields

        #region Constructors

        public EventHandlerSubscription(IEventHandlerFor<TEvent> eventHandler, bool useWeakReference, Action<IUnsubscribeToken> unsubscribeAction)
            : base(unsubscribeAction)
        {
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));
            if (unsubscribeAction == null) throw new ArgumentNullException(nameof(unsubscribeAction));

            if (useWeakReference)
            {
                this._weakReference = new WeakReference(eventHandler);
            }
            else
            {
                this._directHandler = eventHandler;
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
            if (_wasLost)
            {
                return null;
            }
            if (_directHandler != null)
            {
                return _directHandler;
            }
            else
            {
                var r = this._weakReference.Target as IEventHandlerFor<TEvent>;
                if (r == null || !this._weakReference.IsAlive)
                {
                    _wasLost = true;
                    this.Unsubscribe();
                }

                return r;
            }
        }

        #endregion Private Methods
    }
}