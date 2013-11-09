using System;

namespace Interlook.Eventing
{
	/// <summary>
	/// Simple class for event args containing a typed payload.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EventArgs<T> : EventArgs
	{
		/// <summary>
		/// Actual payload of the event args.
		/// </summary>
		public T Data { get; private set; }

		/// <summary>
		/// Creates an generic event args object.
		/// </summary>
		/// <param name="data">Payload of the event args.</param>
		public EventArgs(T data)
		{
			this.Data = data;
		}
	}
}