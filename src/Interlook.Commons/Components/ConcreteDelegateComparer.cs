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
using System.Text;

namespace Interlook.Components
{
    /// <summary>
    /// Variant of <see cref="DelegateComparer{T}"/>, which only uses the equality delegate,
    /// if both arguments are not null (otherwise <c>false</c> is returned) 
    /// and are no equal references (results in <c>true</c>).
    /// Furthermore the hashing delegate is only called for non null objects and
    /// zero returned instead.
    /// </summary>
    /// <typeparam name="T">Type of the objects, that are to be compared.</typeparam>
    public class ConcreteDelegateComparer<T> : DelegateComparer<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcreteDelegateComparer{T}"/> class.
        /// </summary>
        /// <param name="equalsFunc">function, that determines the equality of two non-<c>null</c> objects of type <c>T</c>.</param>
        /// <param name="hashFunc">Function, that calculates the hashcode for a <c>T</c> object, that is not null.</param>
        public ConcreteDelegateComparer(Func<T, T, bool> equalsFunc, Func<T, int> hashFunc)
            : base(equalsFunc, hashFunc)
        {
            if (equalsFunc == null) throw new ArgumentNullException(nameof(equalsFunc));
            if (hashFunc == null) throw new ArgumentNullException(nameof(hashFunc));
        }

        /// <summary>
        /// Determines, if two given objects of type <c>T</c> are equal.
        /// </summary>
        /// <param name="x">The first object.</param>
        /// <param name="y">The object to compare.</param>
        /// <returns>
        ///   <c>true</c>, if the given objects are equal, otherwise <c>false</c>.
        /// </returns>
        public override bool Equals(T x, T y)
        {
            if ((object)x == null || (object)y == null)
            {
                return false;
            }

            if (ReferenceEquals(x, y))
            {
                return true;
            }
            
            return base.Equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode(T obj)
        {
            if((object)obj == null)
            {
                return 0;
            }
            else
            {
                return base.GetHashCode(obj);
            }
        }
    }
}