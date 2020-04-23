using System;
using Xunit;

namespace Interlook.Eventing.Tests
{
    public class ExtendedSourceAndPublishingTests
    {
        protected IEventBusEx EventBus { get; private set; }

        public ExtendedSourceAndPublishingTests()
        {
            EventBus = new EventBus();
        }

        [Fact]
        public void CanSubscribeWeakDelegateTest()
        {
            var eventAction = new Action<TestEvent>(p => { });
            EventBus.Subscribe(eventAction, true);
        }

        [Fact]
        public void PublishedEventHandledByWeakDelegateTest()
        {
            var wasHandled = false;
            var eventAction = new Action<TestEvent>(p => wasHandled = true);
            var subscription = EventBus.Subscribe(eventAction, true);

            EventBus.Publish(new TestEvent(null));

            Assert.True(wasHandled, "Registered delegate not invoked by publish.");
        }

        [Fact]
        public void PublishedEventHandledFilteredDelegateTest()
        {
            var wasHandled = false;
            var eventPredicate = "correct";
            var definiteWrong = "not correct";

            Action<TestEvent> action = p => wasHandled = true;
            Func<TestEvent, bool> filter = p => eventPredicate.Equals(p.Data);

            var subscription = EventBus.Subscribe(action, filter, true);

            EventBus.Publish(new TestEvent(eventPredicate));
            Assert.True(wasHandled, "Registered delegate not invoked by publish.");

            wasHandled = false;
            EventBus.Publish(new TestEvent(definiteWrong));
            Assert.False(wasHandled, "Registered delegate was invoked although actually was expected to fail the filter.");
        }

        [Fact]
        public void PublishedEventHandledSpecificEventTest()
        {
            var result = string.Empty;
            var original = "Event";
            var delta = "_Raised";

            var eventAction = new Action<TestEvent>(p => result = p.Data + delta);
            var subscription = EventBus.Subscribe(eventAction, true);

            EventBus.Publish(new TestEvent(original));

            Assert.Equal(original + delta, result);
        }

        [Fact]
        public void UnSubscribeDelegateViaDisposeTest()
        {
            var wasHandled = false;
            var eventAction = new Action<TestEvent>(p => wasHandled = !wasHandled);
            using (var subscription = EventBus.Subscribe<TestEvent>(eventAction))
            {
                EventBus.Publish(new TestEvent(null));
                Assert.True(wasHandled, "Registered delegate not invoked by publish.");
            }

            EventBus.Publish(new TestEvent(null));
            Assert.True(wasHandled, "Registered delegate invoked although subscription was disposed.");
        }

        [Fact]
        public void UnSubscribeDelegateViaUnsubscriptionToken()
        {
            var wasHandled = false;
            var eventAction = new Action<TestEvent>(p => wasHandled = !wasHandled);
            var subscription = EventBus.Subscribe(eventAction, true);
            EventBus.Publish(new TestEvent(null));
            Assert.True(wasHandled, "Registered delegate not invoked by publish.");
            EventBus.Unsubscribe(subscription);

            EventBus.Publish(new TestEvent(null));
            Assert.True(wasHandled, "Registered delegate invoked although unsubscription has been called.");
        }

        [Fact]
        public void UnSubscribeDelegateViaUnsubscribeDelegateMethodTest()
        {
            var wasHandled = false;
            var eventAction = new Action<TestEvent>(p => wasHandled = !wasHandled);
            Action<TestEvent> action = new Action<TestEvent>(eventAction);
            var subscription = EventBus.Subscribe(action, true);
            EventBus.Publish(new TestEvent(null));
            Assert.True(wasHandled, "Registered delegate not invoked by publish.");
            EventBus.Unsubscribe(action);

            EventBus.Publish(new TestEvent(null));
            Assert.True(wasHandled, "Registered delegate invoked although unsubscription has been called.");
        }

        [Fact]
        public void CanRegisterHandlerTest()
        {
            var handler = new StringAppendingTestEventHandler("_tested");
            EventBus.RegisterHandlerFor(handler, true);
        }

        [Fact]
        public void PublishedEventHandledByHandlerTest()
        {
            var original = "start";
            var delta = "_tested";
            var handler = new StringAppendingTestEventHandler(delta);
            EventBus.RegisterHandlerFor(handler, true);

            var ev = new TestEvent(original);
            EventBus.Publish(ev);

            Assert.Equal(original + delta, ev.Data);
        }

        [Fact]
        public void UnregisterHandlerViaDisposeTest()
        {
            var original = "active";
            var delta = "_tested";
            var ev = new TestEvent(original);
            var handler = new StringAppendingTestEventHandler(delta);
            using (var subscription = EventBus.RegisterHandlerFor(handler, true))
            {
                EventBus.Publish(ev);
                Assert.Equal(original + delta, ev.Data);
            }

            original = "disposed";
            ev = new TestEvent(original);
            EventBus.Publish(ev);
            Assert.Equal(original, ev.Data);
        }

        [Fact]
        public void UnregisterHandlerViaUnsubscriptionToken()
        {
            var original = "start";
            var delta = "_tested";
            var ev = new TestEvent(original);
            var handler = new StringAppendingTestEventHandler(delta);
            var subscription = EventBus.RegisterHandlerFor<TestEvent>(handler, true);
            EventBus.Publish(ev);
            Assert.Equal(original + delta, ev.Data);
            EventBus.Unsubscribe(subscription);

            original = "disposed";
            ev = new TestEvent(original);
            EventBus.Publish(ev);
            Assert.Equal(original, ev.Data);
        }

        [Fact]
        public void UnregisterHandlerViaUnregisterHandlerMethodTest()
        {
            var original = "start";
            var delta = "_tested";
            var ev = new TestEvent(original);
            var handler = new StringAppendingTestEventHandler(delta);
            var subscription = EventBus.RegisterHandlerFor(handler, true);
            EventBus.Publish(ev);
            Assert.Equal(original + delta, ev.Data);

            EventBus.UnregisterHandlerFor(handler);

            original = "disposed";
            ev = new TestEvent(original);
            EventBus.Publish(ev);
            Assert.Equal(original, ev.Data);
        }
    }
}