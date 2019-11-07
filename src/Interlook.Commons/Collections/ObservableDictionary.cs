#region license

//MIT License

//Copyright(c) 2016 Andreas Huebner

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

#endregion 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Interlook.Collections
{
	/// <summary>
	/// A dictionary implementing <see cref="INotifyCollectionChanged"/> and <see cref="INotifyPropertyChanged"/>.
	/// </summary>
	/// <typeparam name="TKey">Type of key element</typeparam>
	/// <typeparam name="TValue">Type of value element</typeparam>
	public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		#region Consts

		private const string STRING_PROPERTY_COUNT = "Count";
		private const string STRING_PROPERTY_INDEXER = "Item[]";
		private const string STRING_EXCEPTION_ADD_EXIST = "Key already exists in dictionary.";
		private const string STRING_EXCEPTION_NULL = "The parameter must not be null.";

		#endregion Consts

		#region Fields

		private IDictionary<TKey, TValue> _internalDictionary;

		#endregion Fields

		#region Properties

		protected IDictionary<TKey, TValue> Dictionary
		{
			get { return _internalDictionary; }
		}

		#endregion Properties

		#region Constructors

		public ObservableDictionary()
		{
			_internalDictionary = new Dictionary<TKey, TValue>();
		}

		public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
		{
			dictionary = new Dictionary<TKey, TValue>(dictionary);
		}

		public ObservableDictionary(IEqualityComparer<TKey> comparer)
		{
			_internalDictionary = new Dictionary<TKey, TValue>(comparer);
		}

		public ObservableDictionary(int capacity)
		{
			_internalDictionary = new Dictionary<TKey, TValue>(capacity);
		}

		public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
		{
			dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
		}

		public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			_internalDictionary = new Dictionary<TKey, TValue>(capacity, comparer);
		}

		#endregion Constructors

		#region IDictionary<TKey,TValue> Members

		/// <summary>
		/// Adds an element with the provided key and value to the dictionary.
		/// </summary>
		/// <param name="key">The object to use as the key of the element to add.</param>
		/// <param name="value">The object to use as the value of the element to add.</param>
		public void Add(TKey key, TValue value)
		{
			setItem(key, value, true);
		}

		/// <summary>
		/// Determines whether the dictionary  contains an element with the specified key.
		/// </summary>
		/// <param name="key">The key to locate in the dictionary.</param>
		/// <returns>
		/// 	<c>true</c>, if the dictionary contains an element with that key; otherwise <c>false</c>.
		/// </returns>
		public bool ContainsKey(TKey key)
		{
			return Dictionary.ContainsKey(key);
		}

		/// <summary>
		///  Gets an <see cref="System.Collections.Generic.ICollection<T>"/> containing the keys of the dictionary.
		/// </summary>
		public ICollection<TKey> Keys
		{
			get { return Dictionary.Keys; }
		}

		/// <summary>
		/// Removes the element with the specified key from the dictionary.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <returns><c>true</c> if the element is successfully removed; otherwise <c>false</c>.</returns>
		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key), STRING_EXCEPTION_NULL);
			}

			TValue value;
			Dictionary.TryGetValue(key, out value);
			var removed = Dictionary.Remove(key);
			if (removed)
			{
				onCollectionItemChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));
			}

			return removed;
		}

		/// <summary>
		/// Gets the value associated with the specified key.
		/// No Exception is thrown if no element exists for the given key.
		/// </summary>
		/// <param name="key">The key whose value to get.</param>
		/// <param name="value">
		/// When this method returns, the value associated with the specified key, if
		/// the key is found; otherwise, the default value for the type of the value
		/// parameter. This parameter is passed uninitialized.
		/// </param>
		/// <returns><c>true</c> if an element for this key was found in the dictionary and its value stored in <c>value</c>.</returns>
		public bool TryGetValue(TKey key, out TValue value)
		{
			return Dictionary.TryGetValue(key, out value);
		}

		/// <summary>
		///  Gets an <see cref="System.Collections.Generic.ICollection<T>"/> containing the values int the dictionary.
		/// </summary>
		public ICollection<TValue> Values
		{
			get { return Dictionary.Values; }
		}

		/// <summary>
		/// Gets or sets the element with the specified key.
		/// </summary
		/// <param name="key">The key of the element to get or set.</param>
		/// <value>The element with the specified key.</value>
		public TValue this[TKey key]
		{
			get
			{
				return Dictionary[key];
			}

			set
			{
				setItem(key, value, false);
			}
		}

		#endregion IDictionary<TKey,TValue> Members

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		/// <summary>
		/// Adds an item.
		/// </summary>
		/// <param name="item">The new item.</param>
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			setItem(item.Key, item.Value, true);
		}

		/// <summary>
		/// Removes all items of the dictionary.
		/// </summary>
		public void Clear()
		{
			if (Dictionary.Count > 0)
			{
				Dictionary.Clear();
				onCollectionReset();
			}
		}

		/// <summary>
		///  Determines whether the System.Collections.Generic.ICollection<T> contains a specific item.
		/// </summary>
		/// <param name="item">The object to locate in the dictionary</param>
		/// <returns>
		/// 	<c>true</c> if item is found in the dictionary; otherwise <c>false</c>.
		/// </returns>
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return Dictionary.Contains(item);
		}

		/// <summary>
		/// Copies the elements of the dictionary into an array, 
		/// starting at an particular index
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination 
		/// of the elements from this dictionary.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			Dictionary.CopyTo(array, arrayIndex);
		}

		/// <summary>
		///  Gets the number of elements contained in the dictionary.
		/// </summary>
		public int Count
		{
			get { return Dictionary.Count; }
		}

		/// <summary>
		/// Returns whether the dictionary is readonly or can be manipulated.
		/// </summary>
		public bool IsReadOnly
		{
			get { return Dictionary.IsReadOnly; }
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the dictionary.
		/// </summary>
		/// <param name="item">The object to remove.</param>
		/// <returns><c>true</c> if item was successfully removed; otherwise <c>false</c></returns>
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item.Key);
		}

		#endregion ICollection<KeyValuePair<TKey,TValue>> Members

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		/// <summary>
		/// Returns an enumerator for iterating through the dictionary.
		/// </summary>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return Dictionary.GetEnumerator();
		}

		#endregion IEnumerable<KeyValuePair<TKey,TValue>> Members

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that iterates through the dictionary.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the dictionary
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)Dictionary).GetEnumerator();
		}

		#endregion IEnumerable Members

		#region INotifyCollectionChanged Members

		private event NotifyCollectionChangedEventHandler collectionChanged;

		/// <summary>
		/// Occurs when the collection changes.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add { collectionChanged += value; }
			remove { collectionChanged -= value; }
		}

		/// <summary>
		/// Raises the <see cref="E:collectionChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (collectionChanged != null)
			{
				collectionChanged(this, e);
			}
		}

		#endregion INotifyCollectionChanged Members

		#region INotifyPropertyChanged Members

		private event PropertyChangedEventHandler propertyChanged;

		/// <summary>
		///  Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add { propertyChanged += value; }
			remove { propertyChanged -= value; }
		}

		/// <summary>
		/// Raising the <c>propertyChanged</c> event.
		/// </summary>
		/// <param name="propertyName">Name of the changed property.</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion INotifyPropertyChanged Members

		/// <summary>
		/// Adds several items at once.
		/// </summary>
		/// <param name="items">Dictionary, containing the items to add.</param>
		public void AddRange(IDictionary<TKey, TValue> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException(nameof(items), STRING_EXCEPTION_NULL);
			}

			if (items.Count > 0)
			{
				if (Dictionary.Count > 0)
				{
					if (items.Keys.Any((k) => Dictionary.ContainsKey(k)))
					{
						throw new ArgumentException(STRING_EXCEPTION_ADD_EXIST);
					}
					else
					{
						foreach (var item in items)
						{
							Dictionary.Add(item);
						}
					}
				}
				else
				{
					_internalDictionary = new Dictionary<TKey, TValue>(items);
				}

				onCollectionAdd(NotifyCollectionChangedAction.Add, items.ToArray());
			}
		}

		private void onCollectionReset()
		{
			onPropertiesChanged();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		private void onCollectionItemChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
		{
			onPropertiesChanged();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItem));
		}

		private void onCollectionItemReplaced(KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
		{
			onPropertiesChanged();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem));
		}

		private void onCollectionAdd(NotifyCollectionChangedAction action, IList newItems)
		{
			onPropertiesChanged();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItems));
		}

		private void setItem(TKey key, TValue value, bool add)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key), STRING_EXCEPTION_NULL);
			}

			TValue item;
			if (Dictionary.TryGetValue(key, out item))
			{
				if (add)
				{
					throw new ArgumentException(STRING_EXCEPTION_ADD_EXIST);
				}

				if (!Equals(item, value))
				{
					Dictionary[key] = value;
					onCollectionItemReplaced(new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
				}
			}
			else
			{
				Dictionary[key] = value;
				onCollectionItemChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
			}
		}

		private void onPropertiesChanged()
		{
			OnPropertyChanged(STRING_PROPERTY_COUNT);
			OnPropertyChanged(STRING_PROPERTY_INDEXER);
		}
	}
}