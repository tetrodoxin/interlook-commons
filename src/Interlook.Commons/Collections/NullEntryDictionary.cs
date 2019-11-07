#region license

//MIT License

//Copyright(c) 2013-2019 Andreas Hübner

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

namespace Interlook.Collections
{
    /// <summary>
    /// Dictionary, which always contains a value for <c>null</c> as key. 
    /// This <c>null</c>-value will always be iterated at first place.
    /// </summary>
    /// <typeparam name="TKey">Datatype of the key.</typeparam>
    /// <typeparam name="TValue">Datatype of the value.</typeparam>
    public class NullEntryDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        /// <summary>
        /// The Value for the <c>null</c>-key
        /// </summary>
        public TValue NullEntryValue { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TValue"/> with the specified key.
        /// </summary>
        /// <value>
        /// The value for the key (including <c>null</c>).
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The value for the key.</returns>
        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                if (key == null)
                {
                    return NullEntryValue;
                }
                else
                {
                    return base[key];
                }
            }

            set
            {
                if (key == null)
                {
                    NullEntryValue = value;
                }
                else
                {
                    base[key] = value;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => new NullEntryEnumerator<TKey, TValue>(this, NullEntryValue, () 
            => base.GetEnumerator());

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() 
            => new NullEntryEnumerator<TKey, TValue>(this, NullEntryValue, () => base.GetEnumerator());

        #region Constructors

        public NullEntryDictionary()
        { }

        public NullEntryDictionary(int capacity)
            : base(capacity)
        { }

        public NullEntryDictionary(IDictionary<TKey, TValue> dictionary)
        {
            foreach (var p in dictionary)
            {
                base.Add(p.Key, p.Value);
            }
        }

        #endregion Constructors

        /// <summary>
        /// Sets the <c>null</c>-value for this dictionary in fluent API.
        /// </summary>
        /// <param name="nullValue">The value for the <c>null</c>-key.</param>
        /// <returns>This instance.</returns>
        public NullEntryDictionary<TKey, TValue> WithNullEntryValue(TValue nullValue)
        {
            NullEntryValue = nullValue;
            return this;
        }
    }

    /// <summary>
    /// Helper enumerator, which ensures, that the <c>null</c>-value is always listed
    /// in first place.
    /// </summary>
    /// <typeparam name="TKey">Datatype of the key.</typeparam>
    /// <typeparam name="TValue">Datataype of the value.</typeparam>
    public struct NullEntryEnumerator<TKey, TValue> : IEnumerator, IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private bool _isReset;
        private bool _isFirst;
        private KeyValuePair<TKey, TValue> _nullValue;
        private IDictionary<TKey, TValue> _dict;
        private IEnumerator<KeyValuePair<TKey, TValue>> _dictGenericEnumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullEntryEnumerator{TKey, TValue}" /> struct.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="nullValue">The null value.</param>
        /// <param name="baseEnumeratorFactoryMethod">The base enumerator factory method.</param>
        public NullEntryEnumerator(IDictionary<TKey, TValue> dictionary,
                    TValue nullValue,
                    Func<IEnumerator<KeyValuePair<TKey, TValue>>> baseEnumeratorFactoryMethod)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));

            _isReset = true;
            _isFirst = false;
            _nullValue = new KeyValuePair<TKey, TValue>(default(TKey), nullValue);
            _dict = dictionary;
            _dictGenericEnumerator = baseEnumeratorFactoryMethod();
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>
        /// The element in the collection at the current position of the enumerator.
        /// </value>
        public KeyValuePair<TKey, TValue> Current
        {
            get
            {
                if (_isReset || _isFirst)
                {
                    return _nullValue;
                }
                else
                {
                    return _dictGenericEnumerator.Current;
                }
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        ///  <c>true</c> if the enumerator was successfully advanced to the next element; <c>false</c> if
        ///  the enumerator has passed the end of the collection.</returns>
        ///  <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        public bool MoveNext()
        {
            if (_isReset)
            {
                _isReset = false;
                _isFirst = true;
                return true;
            }
            else if (_isFirst)
            {
                _dictGenericEnumerator.Reset();
                _isFirst = false;
            }

            return moveEnumerator();
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the sequence.
        /// </summary>
        public void Reset()
        {
            _isReset = true;
        }

        private bool moveEnumerator()
        {
            var result = _dictGenericEnumerator.MoveNext();
            if (!result)
            {
                _isReset = true;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        { }

        object IEnumerator.Current => Current;
    }
}