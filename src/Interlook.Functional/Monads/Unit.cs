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

namespace Interlook.Monads
{
    /// <summary>
    /// A type with only one single value.
    /// Is used in monads instead of <c>void</c>.
    /// </summary>
    public struct Unit
    {
        private static Lazy<Unit> defValue = new Lazy<Unit>(() => new Unit());

        /// <summary>
        /// Gets the very single value.
        /// </summary>
        public static Unit Default => defValue.Value;

        /// <summary>
        /// Returns the <see cref="Unit"/> type/instance after invoking an action
        /// to simulate a call of a <c>void</c> method.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="action"/> was <c>null</c>.</exception>
        public static Unit Return(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            action();
            return Default;
        }
    }
}