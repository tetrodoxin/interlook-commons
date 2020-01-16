#region license

//MIT License

//Copyright(c) 2013-2020 Andreas Hübner

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
    /// as they implement <see cref=" INotifyCollectionChanged"/>
    /// </summary>
    /// <typeparam name="T">The type of elements of all the contained enumerators.</typeparam>
    public class CompositeObservableEnumerable<T> : IEnumerable<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Fields

        private List<IEnumerable<T>> _enumerations;

        #endregion Fields

        #region Events

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        private event PropertyChangedEventHandler _propertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private event NotifyCollectionChangedEventHandler _collectionChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _collectionChanged += value; }
            remove { _collectionChanged -= value; }
        }

        protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            _collectionChanged?.Invoke(this, args);
        }

        #endregion Events


        /// <summary>
        /// The total number of elements in all included enumerators.
        /// </summary>
        public virtual int Count => _enumerations.Sum(p => p.Count());

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeObservableEnumerable{T}" /> class.
        /// </summary>
        public CompositeObservableEnumerable()
        {
            _enumerations = new List<IEnumerable<T>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeObservableEnumerable{T}" /> class
        /// with a given sequence to add.
        /// </summary>
        /// <param name="sequenceToAdd">The sequence to add.</param>
        public CompositeObservableEnumerable(IEnumerable<T> sequenceToAdd)
        {
            _enumerations = new List<IEnumerable<T>>();
            AddEnumerable(sequenceToAdd);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Adds an enumerator.
        /// </summary>
        /// <param name="sequenceToAdd">The sequence to add.</param>
        public virtual void AddEnumerable(IEnumerable<T> sequenceToAdd)
        {
            _enumerations.Add(sequenceToAdd);

            var cn = sequenceToAdd as INotifyCollectionChanged;
            if (cn != null)
            {
                cn.CollectionChanged += new NotifyCollectionChangedEventHandler(handleContainedCollectionChanged);
            }

            var cp = sequenceToAdd as INotifyPropertyChanged;
            if (cp != null)
            {
                cp.PropertyChanged += new PropertyChangedEventHandler(handleContainedCollectionPropertyChanged);
            }

            raiseCountPropertyChanged();
        }

        /// <summary>
        /// Determines whether any of the underlying enumerators contains an specified element.
        /// </summary>
        /// <param name="item">The element to check for.</param>
        /// <returns><c>true</c>, if an underlying enumerator contains the element.</returns>
        public bool Contains(T item) => _enumerations.Any(p => p.Contains(item));

        /// <summary>
        /// Copies the total content of all underlying enumerators to an array.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="arrayIndex">Index in <c>array</c> at which the copying begins.</param>
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
            foreach (var en in _enumerations)
            {
                foreach (var item in en)
                {
                    array[pos] = item;
                }
            }
        }

        /// <summary>
        /// Returns the enumerator over all underlying enumerators.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var c in _enumerations)
            {
                foreach (var item in c)
                {
                    yield return item;
                }
            }
        }

        #endregion Public Methods

        #region Explicit Interface Implementations

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

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