using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Interlook.Collections
{
	public class ObservableEnumerableAdapter<T> : IEnumerable<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private INotifyCollectionChanged collection;
		private INotifyPropertyChanged observable;
		private IEnumerable<T> source;

		public ObservableEnumerableAdapter(ObservableCollection<T> originalSource, IEnumerable<T> enumerable)
		{
			this.source = enumerable;
			this.collection = originalSource;
			this.observable = originalSource;

			this.collection.CollectionChanged += new NotifyCollectionChangedEventHandler(handleCollectionChanged);
		}

		private void handleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			ReRaiseCollectionChanged(e);
		}

		private event PropertyChangedEventHandler propertyChanged;

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { propertyChanged += value; }
			remove { propertyChanged -= value; }
		}

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
			add { collectionChanged += value; }
			remove { collectionChanged -= value; }
		}

		protected void ReRaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (this.collectionChanged != null)
			{
				this.collectionChanged(this, args);
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return source.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return source.GetEnumerator();
		}
	}
}