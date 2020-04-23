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
using System.Collections;

namespace Interlook.Collections
{
    /// <summary>
    /// Dictionary, which always contains a value for <c>null</c> as key. 
    /// This <c>null</c>-value will always be iterated at first place.
    /// This dictionary derived from <see cref="ObservableDictionary{TKey, TValue}"/> and therefore
    /// implements the <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>-interface
    /// </summary>
    /// <typeparam name="TKey">Type of key element</typeparam>
    /// <typeparam name="TValue">Type of value element</typeparam>
    public class NullEntryObservableDictionary0<TKey, TValue> : ObservableDictionary<TKey, TValue>, IEnumerable
    {
        /// <summary>
        /// The Value for the <c>null</c>-key
        /// </summary>
        public TValue NullEntryValue { get; set; }

        /// <summary>
        /// Gets or sets the <typeparamref name="TValue"/> with the specified key.
        /// </summary>
        /// <value>
        /// The value for the key (including <c>null</c>).
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The value for the key.</returns>
        public new TValue this[TKey key]
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
                    this.NullEntryValue = value;
                }
                else
                {
                    base[key] = value;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => new NullEntryEnumerator<TKey, TValue>(this, NullEntryValue, () => base.GetEnumerator());
    }
}