﻿#region license

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

namespace Interlook.Components
{
    /// <summary>
    /// An implementation of <see cref="DisposableToken"/> that executes
    /// a delegate, when token object is being disposed.
    /// </summary>
    public class DelegateDisposableToken : DisposableToken
    {
        private Action _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateDisposableToken"/> class.
        /// </summary>
        /// <param name="action">The action to perform at disposal.</param>
        public DelegateDisposableToken(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            _action = action;
        }

        /// <summary>
        /// Action, that is performed, when object is disposing
        /// just before <see cref="GC.SuppressFinalize(object)" /> is called.
        /// </summary>
        protected override void DoDisposeAction()
        {
            if (_action != null)
            {
                _action();
                _action = null;
            }
        }
    }
}