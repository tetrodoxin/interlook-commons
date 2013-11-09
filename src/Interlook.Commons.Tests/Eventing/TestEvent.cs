using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interlook.Eventing;

namespace Interlook.Eventing.Tests
{
	internal class TestEvent : IEvent
	{
		public string Data { get; set; }

		public TestEvent(string payload)
		{
			this.Data = payload;
		}
	}
}
