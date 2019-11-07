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
using System.Collections.Generic;

namespace Interlook.Components
{
    /// <summary>
    /// Implementation of the <see cref="IEqualityComparer{T}"/> interface for comparers,
    /// that are defined via delegate functions.
    /// </summary>
    /// <typeparam name="T">Type of the objects, that are to be compared.</typeparam>
    public class DelegateComparer<T> : IEqualityComparer<T>
    {
        private Func<T, int> _hashFunc;
        private Func<T, T, bool> _equalsFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateComparer{T}"/> class.
        /// </summary>
        /// <param name="equalsFunc">function, that determines the equality of two <c>T</c> objects.</param>
        /// <param name="hashFunc">Function, that calculates the hashcode for a <c>T</c> object.</param>
        public DelegateComparer(Func<T, T, bool> equalsFunc, Func<T, int> hashFunc)
        {
            if (equalsFunc == null) throw new ArgumentNullException(nameof(equalsFunc));
            if (hashFunc == null) throw new ArgumentNullException(nameof(hashFunc));

            _hashFunc = hashFunc;
            _equalsFunc = equalsFunc;
        }

        /// <summary>
        /// Determines, if two given objects of type <c>T</c> are equal.
        /// </summary>
        /// <param name="x">The first object.</param>
        /// <param name="y">The object to compare.</param>
        /// <returns><c>true</c>, if the given objects are equal, otherwise <c>false</c>.</returns>
        public virtual bool Equals(T x, T y) => _equalsFunc(x, y);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public virtual int GetHashCode(T obj) => _hashFunc(obj);
    }
}
