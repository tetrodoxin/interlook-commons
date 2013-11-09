using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Interlook.Collections
{
	/// <summary>
	/// Class for a enumerator, which merges several enumerators by relaying their change events,
	/// as they implement <see cref=" System.Collections.Specialized.INotifyCollectionChanged"/>
	/// </summary>
	/// <typeparam name="T">The type of elements of all the contained enumerators.</typeparam>
	public class CompositeObservableEnumerable<T> : IEnumerable<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		#region Fields

		private List<IEnumerable<T>> enumerations;

		#endregion Fields

		#region Events

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { this.propertyChanged += value; }
			remove { this.propertyChanged -= value; }
		}

		private event PropertyChangedEventHandler propertyChanged;

		protected void RaisePropertyChanged(string propertyName)
		{
			if (this.propertyChanged != null)
			{
				this.propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private event NotifyCollectionChangedEventHandler collectionChanged;

		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add { this.collectionChanged += value; }
			remove { this.collectionChanged -= value; }
		}

		protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (this.collectionChanged != null)
			{
				this.collectionChanged(this, args);
			}
		}

		#endregion Events


		public virtual int Count
		{
			get { return this.enumerations.Sum(p => p.Count()); }
		}

		#region Constructors

		public CompositeObservableEnumerable()
		{
			this.enumerations = new List<IEnumerable<T>>();
		}

		public CompositeObservableEnumerable(IEnumerable<T> collectionToAdd)
		{
			this.enumerations = new List<IEnumerable<T>>();
			AddEnumerable(collectionToAdd);
		}

		#endregion Constructors

		#region Public Methods

		public virtual void AddEnumerable(IEnumerable<T> collectionToAdd)
		{
			this.enumerations.Add(collectionToAdd);

			var cn = collectionToAdd as INotifyCollectionChanged;
			if (cn != null)
			{
				cn.CollectionChanged += new NotifyCollectionChangedEventHandler(handleContainedCollectionChanged);
			}

			var cp = collectionToAdd as INotifyPropertyChanged;
			if (cp != null)
			{
				cp.PropertyChanged += new PropertyChangedEventHandler(handleContainedCollectionPropertyChanged);
			}

			raiseCountPropertyChanged();
		}

		public bool Contains(T item)
		{
			return this.enumerations.Any(p => p.Contains(item));
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException();
			}

			int availableLen = array.Length - arrayIndex;
			int neededLen = Count;

			if (neededLen > availableLen)
			{
				throw new IndexOutOfRangeException("Target array too small to copy content into from given index.");
			}

			int pos = arrayIndex;
			foreach (var en in this.enumerations)
			{
				foreach (var item in en)
				{
					array[pos] = item;
				}
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var c in this.enumerations)
			{
				foreach (var item in c)
				{
					yield return item;
				}
			}
		}

		#endregion Public Methods

		#region Explicit Interface Implementations

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion Explicit Interface Implementations

		#region Private Methods

		private void raiseCountPropertyChanged()
		{
			RaisePropertyChanged("Count");
		}

		private void handleContainedCollectionPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Count")
			{
				raiseCountPropertyChanged();
			}
		}

		private void handleContainedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RaiseCollectionChanged(e);
		}

		#endregion Private Methods
	}
}