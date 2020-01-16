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

namespace Interlook.Components
{
    /// <summary>
    /// Baseclass for token objects, that perform an action, when they're disposed.
    /// Useful for 'terminating actions' in <c>using</c>-statements.
    /// </summary>
    public abstract class DisposableToken : IDisposable, IEquatable<DisposableToken>, IEqualityComparer<DisposableToken>
    {
        private readonly Guid _id;

        public DisposableToken()
        {
            _id = Guid.NewGuid();
        }

        public void Dispose()
        {
            DoDisposeAction();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Action, that is performed, when object is disposing
        /// just before <see cref="GC.SuppressFinalize(object)"/> is called.
        /// </summary>
        protected virtual void DoDisposeAction()
        { }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type to compare.</param>
        /// <param name="y">The second object of type to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public static bool Equals(DisposableToken x, DisposableToken y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if ((object)x == null || (object)y == null)
            {
                return false;
            }

            return Guid.Equals(x._id, y._id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public static int GetHashCode(DisposableToken obj)
        {
            if ((object)obj == null)
            {
                return 0;
            }
            else
            {
                return obj._id.GetHashCode();
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => Equals(this, obj as DisposableToken);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() => GetHashCode(this);

        /// <summary>
        /// Determines whether the specified <see cref="DisposableToken" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(DisposableToken other) => Equals(this, other);

        bool IEqualityComparer<DisposableToken>.Equals(DisposableToken x, DisposableToken y) => Equals(x, y);

        int IEqualityComparer<DisposableToken>.GetHashCode(DisposableToken obj) => GetHashCode(this);
    }
}