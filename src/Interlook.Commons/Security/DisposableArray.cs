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
using System.Linq;

namespace Interlook.Security
{
    /// <summary>
    /// Encapsulates an array into an <see cref="IDisposable"/> object.
    /// The content will be destroyed when the object is disposed.
    /// <para>
    /// This class can be casted to an array of the base type.
    /// </para>
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public abstract class DisposableArray<T> : IDisposable
    {
        private readonly List<IDisposable> _additionalDisposables = new List<IDisposable>();

        private readonly int _hash;

        private bool _disposedValue = false;

        /// <summary>
        /// Gets the length of the original array.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets the original array,
        /// </summary>
        protected T[] OriginalArray { get; private set; }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        public T this[int index]
        {
            get
            {
                return OriginalArray[index];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableArray{T}"/> class
        /// with an existing array, which is NOT copied, but just referenced,
        /// thus external changes will affect this instance too.
        /// </summary>
        /// <param name="content">The content to reference.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="content"/> was <c>null</c>.</exception>
        protected DisposableArray(T[] content)
        {
            OriginalArray = content ?? throw new ArgumentNullException(nameof(content));
            _hash = content.GetHashCode();
            Length = content.Length;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="DisposableBytes"/> to <see cref="byte"/>[].
        /// </summary>
        /// <param name="array">The <see cref="DisposableBytes"/> object to convert.</param>
        public static implicit operator T[](DisposableArray<T> array)
        {
            if (array == null) return new T[0];

            return array.GetContent();
        }

        public override bool Equals(object obj)
        {
            if (obj is DisposableBytes dis)
                return Equals(dis);
            else if (obj is byte[] arr)
                return Equals(arr);
            else
                return false;
        }

        public bool Equals(DisposableArray<T> obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if ((object)obj == null) return false;

            return arraysEqual(OriginalArray, obj.OriginalArray);
        }

        public bool Equals(T[] array)
        {
            if (ReferenceEquals(OriginalArray, array))
                return true;

            if ((object)array == null) return false;

            return arraysEqual(OriginalArray, array);
        }

        /// <summary>
        /// Returns the contained array, directly. No copy is created.
        /// </summary>
        public T[] GetContent() => OriginalArray;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => _hash;

        /// <summary>
        /// Returns a string representation of the array.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{typeof(T).Name} array of length {OriginalArray.Length}";
        }

        private static bool arraysEqual(T[] x, T[] y)
        {
            return x.Length == y.Length
                            && Enumerable.Range(0, y.Length)
                                .All(i => x[i].Equals(y[i]));
        }

        #region IDisposable Support

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            doDispose(true);
        }

        protected void AddAdditionalDisposable(IDisposable d) => _additionalDisposables.Add(d);

        /// <summary>
        /// Performs in overriding classes additional disposing actions.
        /// </summary>
        protected virtual void AdditionalDisposing()
        { }

        private void doDispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    for (int i = 0; i < OriginalArray.Length; i++)
                    {
                        OriginalArray[i] = default;
                    }

                    OriginalArray = null;

                    foreach (var d in _additionalDisposables)
                    {
                        d?.Dispose();
                    }

                    AdditionalDisposing();
                }

                _disposedValue = true;
            }
        }

        #endregion IDisposable Support
    }
}