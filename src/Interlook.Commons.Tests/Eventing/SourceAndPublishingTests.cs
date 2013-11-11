using System;
using Interlook.Eventing;
using Interlook.Eventing.Tests;
using Xunit;
using Xunit.Extensions;

public class SourceAndPublishingTests
{
	protected IEventBus EventBus { get; private set; }

	public SourceAndPublishingTests()
	{
		EventBus = new EventBus();
	}

	[Fact]
	public void CanSubscribeDelegateTest()
	{
		Assert.ThrowsDelegate action = () => EventBus.Subscribe<TestEvent>(p => { });
		Assert.DoesNotThrow(action);
	}

	[Fact]
	public void PublishedEventHandledByDelegateTest()
	{
		var wasHandled = false;
		var subscription = EventBus.Subscribe<TestEvent>(p => wasHandled = true);
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

		var subscription = EventBus.Subscribe<TestEvent>(action, filter);

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

		var subscription = EventBus.Subscribe<TestEvent>(p => result = p.Data + delta);
		EventBus.Publish<TestEvent>(new TestEvent(original));

		Assert.Equal(original + delta, result);
	}

	[Fact]
	public void UnSubscribeDelegateViaDisposeTest()
	{
		var wasHandled = false;
		using (var subscription = EventBus.Subscribe<TestEvent>(p => wasHandled = !wasHandled))
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
		var subscription = EventBus.Subscribe<TestEvent>(p => wasHandled = !wasHandled);
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
		Action<TestEvent> action = new Action<TestEvent>(p => wasHandled = !wasHandled);
		var subscription = EventBus.Subscribe<TestEvent>(action);
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
		Assert.ThrowsDelegate action = () => EventBus.RegisterHandlerFor<TestEvent>(handler);
		Assert.DoesNotThrow(action);
	}

	[Fact]
	public void PublishedEventHandledByHandlerTest()
	{
		var original = "start";
		var delta = "_tested";
		var handler = new StringAppendingTestEventHandler(delta);
		EventBus.RegisterHandlerFor<TestEvent>(handler);

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
		using (var subscription = EventBus.RegisterHandlerFor<TestEvent>(handler))
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
		var subscription = EventBus.RegisterHandlerFor<TestEvent>(handler);
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
		var subscription = EventBus.RegisterHandlerFor<TestEvent>(handler);
		EventBus.Publish<TestEvent>(ev);
		Assert.Equal(original + delta, ev.Data);

		EventBus.UnregisterHandlerFor(handler);

		original = "disposed";
		ev = new TestEvent(original);
		EventBus.Publish<TestEvent>(ev);
		Assert.Equal(original, ev.Data);
	}
}