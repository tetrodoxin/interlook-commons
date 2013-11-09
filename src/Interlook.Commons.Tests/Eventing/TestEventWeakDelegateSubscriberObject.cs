using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interlook.Eventing;

namespace Interlook.Eventing.Tests
{
	internal class TestEventWeakDelegateSubscriberObject
	{
		private Action<TestEvent> action;

		public TestEventWeakDelegateSubscriberObject(Action<TestEvent> action)
		{
			this.action = action;
		}

		public void Subscribe(IEventSourceEx bus)
		{
			bus.Subscribe<TestEvent>(handle, true);
		}

		public void Subscribe(IEventSourceEx bus, Func<TestEvent, bool> filter)
		{
			bus.Subscribe<TestEvent>(handle, filter, true);
		}

		private void handle(TestEvent ev)
		{
			action(ev);
		}
	}
}