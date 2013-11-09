using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Interlook.Collections
{
	[Serializable]
	public class ObservableHashSet<T> : HashSet<T>, ICollection<T>, INotifyCollectionChanged
	{
		#region Events

		private event NotifyCollectionChangedEventHandler collectionChanged;

		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add { collectionChanged += value; }
			remove { collectionChanged -= value; }
		}

		#endregion Events

		void ICollection<T>.Add(T item)
		{
			if (base.Add(item))
			{
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			}
		}

		void ICollection<T>.Clear()
		{
			base.Clear();
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		bool ICollection<T>.Contains(T item)
		{
			return base.Contains(item);
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			base.CopyTo(array, arrayIndex);
		}

		int ICollection<T>.Count
		{
			get { return base.Count; }
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}

		bool ICollection<T>.Remove(T item)
		{
			var r = base.Remove(item);
			if (r)
			{
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
			}

			return r;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return base.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return base.GetEnumerator();
		}

		protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (collectionChanged != null)
			{
				collectionChanged(this, args);
			}
		}
	}
}