using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interlook.Components.Tests
{
	internal class MethodResultTestObject
	{
		public string Payload { get; private set; }

		public MethodResultTestObject(string payload)
		{
			this.Payload = payload;
		}
	}
}
