using System;

namespace Interlook.Eventing
{
	/// <summary>
	/// Interface, that provides creation of buffers/transactions for events.
	/// </summary>
	public interface IEventTransactionSupport
	{
		/// <summary>
		/// Creates an object, implementing <see cref="IEventPublisherTransaction"/> for support of buffering events.
		/// </summary>
		/// <returns>A new transaction object implementing <see cref="IEventPublisherTransaction"/>.</returns>
		IEventPublisherTransaction CreateTransactionForLocalThread();
	}
}