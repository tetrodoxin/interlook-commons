using System;
using Interlook.Eventing;
using Interlook.Eventing.Tests;
using Xunit;
using Xunit.Extensions;

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
		Assert.ThrowsDelegate action = () => EventBus.Subscribe<TestEvent>(eventAction, true);
		Assert.DoesNotThrow(action);
	}

	[Fact]
	public void PublishedEventHandledByWeakDelegateTest()
	{
		var wasHandled = false;
		var eventAction = new Action<TestEvent>(p => wasHandled = true);
		var subscription = EventBus.Subscribe<TestEvent>(eventAction, true);

		EventBus.Publish<TestEvent>(new TestEvent(null));

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

		var subscription = EventBus.Subscribe<TestEvent>(action, filter, true);

		EventBus.Publish<TestEvent>(new TestEvent(eventPredicate));
		Assert.True(wasHandled, "Registered delegate not invoked by publish.");

		wasHandled = false;
		EventBus.Publish<TestEvent>(new TestEvent(definiteWrong));
		Assert.False(wasHandled, "Registered delegate was invoked although actually was expected to fail the filter.");
	}

	[Fact]
	public void PublishedEventHandledSpecificEventTest()
	{
		var result = String.Empty;
		var original = "Event";
		var delta = "_Raised";

		var eventAction = new Action<TestEvent>(p => result = p.Data + delta);
		var subscription = EventBus.Subscribe<TestEvent>(eventAction, true);

		EventBus.Publish<TestEvent>(new TestEvent(original));

		Assert.Equal(original + delta, result);
	}

	[Fact]
	public void UnSubscribeDelegateViaDisposeTest()
	{
		var wasHandled = false;
		var eventAction = new Action<TestEvent>(p => wasHandled = !wasHandled);
		using (var subscription = EventBus.Subscribe<TestEvent>(eventAction))
		{
			EventBus.Publish<TestEvent>(new TestEvent(null));
			Assert.True(wasHandled, "Registered delegate not invoked by publish.");
		}

		EventBus.Publish<TestEvent>(new TestEvent(null));
		Assert.True(wasHandled, "Registered delegate invoked although subscription was disposed.");
	}

	[Fact]
	public void UnSubscribeDelegateViaUnsubscriptionToken()
	{
		var wasHandled = false;
		var eventAction = new Action<TestEvent>(p => wasHandled = !wasHandled);
		var subscription = EventBus.Subscribe<TestEvent>(eventAction, true);
		EventBus.Publish<TestEvent>(new TestEvent(null));
		Assert.True(wasHandled, "Registered delegate not invoked by publish.");
		EventBus.Unsubscribe(subscription);

		EventBus.Publish<TestEvent>(new TestEvent(null));
		Assert.True(wasHandled, "Registered delegate invoked although unsubscription has been called.");
	}

	[Fact]
	public void UnSubscribeDelegateViaUnsubscribeDelegateMethodTest()
	{
		var wasHandled = false;
		var eventAction = new Action<TestEvent>(p => wasHandled = !wasHandled);
		Action<TestEvent> action = new Action<TestEvent>(eventAction);
		var subscription = EventBus.Subscribe<TestEvent>(action, true);
		EventBus.Publish<TestEvent>(new TestEvent(null));
		Assert.True(wasHandled, "Registered delegate not invoked by publish.");
		EventBus.Unsubscribe(action);

		EventBus.Publish<TestEvent>(new TestEvent(null));
		Assert.True(wasHandled, "Registered delegate invoked although unsubscription has been called.");
	}

	[Fact]
	public void CanRegisterHandlerTest()
	{
		var handler = new StringAppendingTestEventHandler("_tested");
		Assert.ThrowsDelegate action = () => EventBus.RegisterHandlerFor<TestEvent>(handler, true);
		Assert.DoesNotThrow(action);
	}

	[Fact]
	public void PublishedEventHandledByHandlerTest()
	{
		var original = "start";
		var delta = "_tested";
		var handler = new StringAppendingTestEventHandler(delta);
		EventBus.RegisterHandlerFor<TestEvent>(handler, true);

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
		using (var subscription = EventBus.RegisterHandlerFor<TestEvent>(handler, true))
		{
			EventBus.Publish<TestEvent>(ev);
			Assert.Equal(original + delta, ev.Data);
		}

		original = "disposed";
		ev = new TestEvent(original);
		EventBus.Publish<TestEvent>(ev);
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
		EventBus.Publish<TestEvent>(ev);
		Assert.Equal(original + delta, ev.Data);
		EventBus.Unsubscribe(subscription);

		original = "disposed";
		ev = new TestEvent(original);
		EventBus.Publish<TestEvent>(ev);
		Assert.Equal(original, ev.Data);
	}

	[Fact]
	public void UnregisterHandlerViaUnregisterHandlerMethodTest()
	{
		var original = "start";
		var delta = "_tested";
		var ev = new TestEvent(original);
		var handler = new StringAppendingTestEventHandler(delta);
		var subscription = EventBus.RegisterHandlerFor<TestEvent>(handler, true);
		EventBus.Publish<TestEvent>(ev);
		Assert.Equal(original + delta, ev.Data);

		EventBus.UnregisterHandlerFor(handler);

		original = "disposed";
		ev = new TestEvent(original);
		EventBus.Publish<TestEvent>(ev);
		Assert.Equal(original, ev.Data);
	}

	[Fact]
	public void IsWeakDelegateGettingGarbageCollectedTest()
	{
		var wasHandled = false;

		var subscriber = new TestEventWeakDelegateSubscriberObject(p => wasHandled = true);
		subscriber.Subscribe(EventBus);

		EventBus.Publish<TestEvent>(new TestEvent(null));
		Assert.True(wasHandled, "Registered delegate was not invoked.");

		wasHandled = false;
		subscriber = null;
		GC.Collect();		// force garbage collection

		EventBus.Publish<TestEvent>(new TestEvent(null));
		Assert.False(wasHandled, "Registered delegate was still invoked, although set to null with forced garbage collection.");
	}

	[Fact]
	public void IsWeakHandlerGettingGarbageCollectedTest()
	{
		var original = "start";
		var delta = "_tested";
		var handler = new StringAppendingTestEventHandler(delta);

		var weakDelegate = new WeakReference(handler);

		EventBus.RegisterHandlerFor<TestEvent>(handler, true);

		handler = null;
		GC.Collect();		// force garbage collection

		Assert.False(weakDelegate.IsAlive, "Delegate has not been garbage collected.");

		var ev = new TestEvent(original);
		EventBus.Publish(ev);

		Assert.Equal(original, ev.Data);
	}

	[Fact]
	public void IsWeakFilteredDelegateGettingGarbageCollectedTest()
	{
		var wasHandled = false;
		var correctData = "correct";

		var subscriber = new TestEventWeakDelegateSubscriberObject(p => wasHandled = true);
		subscriber.Subscribe(EventBus, p => correctData.Equals(p.Data));

		EventBus.Publish<TestEvent>(new TestEvent(correctData));
		Assert.True(wasHandled, "Registered delegate was not invoked.");

		wasHandled = false;
		subscriber = null;
		GC.Collect();		// force garbage collection

		EventBus.Publish<TestEvent>(new TestEvent(correctData));
		Assert.False(wasHandled, "Registered delegate was still invoked, although set to null with forced garbage collection.");
	}
}