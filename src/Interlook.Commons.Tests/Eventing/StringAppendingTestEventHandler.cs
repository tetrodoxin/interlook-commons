using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interlook.Eventing;

namespace Interlook.Eventing.Tests
{
	internal class StringAppendingTestEventHandler : IEventHandlerFor<TestEvent>
	{
		private const string DEFAULT = "_handled";

		private string appendix;

		public StringAppendingTestEventHandler(string appendix)
		{
			this.appendix = String.IsNullOrEmpty(appendix) ? DEFAULT : appendix;
		}

		public void Handle(TestEvent ev)
		{
			ev.Data = ev.Data + appendix;
		}

		public bool CanHandle(TestEvent ev)
		{
			return ev != null;
		}
	}
}
