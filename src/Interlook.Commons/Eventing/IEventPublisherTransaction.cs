using System;

namespace Interlook.Eventing
{
	/// <summary>
	/// Interface, that provides methods for buffering events to finally commit or discard them. Makes it thus possible,
	/// to collect a bunch of events first an publish them later as a one-block.
	/// </summary>
	public interface IEventPublisherTransaction : IEventBus, IDisposable
	{
		/// <summary>
		/// Publishes all events, that has been published into buffer so far.
		/// </summary>
		void PublishAllEvents();

		/// <summary>
		/// Discards all bufferend events.
		/// </summary>
		void DiscardAllEvents();
	}
}