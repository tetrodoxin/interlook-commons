using System;

namespace Interlook.Eventing
{
	/// <summary>
	/// Interface, that combines the interface for event publishing and the extended event source, which supports
	/// in addition to subscribing to events the usage of weak delegates for subscriptions.
	/// </summary>
	public interface IEventBusEx : IEventBus, IEventSourceEx
	{
	}
}