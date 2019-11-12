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
using System.Linq;
using System.Threading.Tasks;

namespace Interlook.Components
{
    /// <summary>
    /// A chain-of-responsibility implementation as a queue of delegates.
    /// An item is passed on for processing until a handler signals,
    /// that it has processed the item.
    /// Handlers can optionally be added with priorities, but not placed
    /// in a certain position in the chain.
    /// </summary>
    /// <typeparam name = "TItem">Data type to be processed</typeparam>
    /// <typeparam name="TResult">Data type of the handlers' result.</typeparam>
    public class AsyncChainOfResponsibility<TItem, TResult>
    {
        private Dictionary<AsyncChainOfResponsibilityResultHandler<TItem, TResult>, Priority> _handlers
            = new Dictionary<AsyncChainOfResponsibilityResultHandler<TItem, TResult>, Priority>();

        /// <summary>
        /// Specifies how to handle exceptions in handlers when processing.
        /// </summary>
        public ExceptionHandling ErrorHandling { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncChainOfResponsibility{T}"/> class.
        /// </summary>
        public AsyncChainOfResponsibility()
        {
            ErrorHandling = ExceptionHandling.IgnoreAndContinue;
        }

        /// <summary>
        /// Adds an processor (handler) to the chain.
        /// </summary>
        /// <param name = "handler">The handler to be added. </param>
        /// <param name = "priority">The priority of the handler to add. </param>
        public void AddHandler(AsyncChainOfResponsibilityResultHandler<TItem, TResult> handler, Priority priority)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            _handlers[handler] = priority;
        }

        /// <summary>
        /// Adds an processor (handler) to the chain.
        /// </summary>
        /// <param name = "handler">The handler to be added. </param>
        public void AddHandler(AsyncChainOfResponsibilityResultHandler<TItem, TResult> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            AddHandler(handler, Priority.Normal);
        }

        /// <summary>
        /// Passes an item to the chain for processing.
        /// </summary>
        /// <param name = "itemToProcess">The item to process. </param>
        /// <param name="resultCallback">Callback, invoked with processed-state (<c>true/false</c>) when chain has been finished without errors. Must not be <c>null</c></param>
        public async Task ProcessAsync(TItem itemToProcess, Action<TResult> resultCallback)
        {
            if (resultCallback == null) throw new ArgumentNullException(nameof(resultCallback));

            await ProcessAsync(itemToProcess, resultCallback, null);
        }

        /// <summary>
        /// Passes an item to the chain for processing.
        /// </summary>
        /// <param name = "itemToProcess">The item to process. </param>
        /// <param name="resultCallback">Callback, invoked with processed-state (<c>true/false</c>) when chain has been finished without errors. Must not be <c>null</c></param>
        /// <param name="errorCallback">Optional. Callback, to be invoked if an exception occurs (if <see cref="ExceptionHandling"/> is set accordningly),
        /// independent of <c>resultCallback</c>.</param>
        public async Task ProcessAsync(TItem itemToProcess, Action<TResult> resultCallback, Action<Exception> errorCallback)
        {
            if (resultCallback == null) throw new ArgumentNullException(nameof(resultCallback));

            var handlers = _handlers.OrderByDescending(p => p.Value)
                .ToList();

            TResult result = default(TResult);
            bool hasResult = false;

            foreach (var handler in handlers)
            {
                try
                {
                    var handled = await handler.Key(itemToProcess);
                    if (handled.IsSuccess)
                    {
                        hasResult = true;
                        result = handled.Result;

                        break;
                    }
                }
                catch (Exception ex)
                {
                    errorCallback?.Invoke(ex);

                    if (ErrorHandling == ExceptionHandling.ThrowException)
                    {
                        throw;
                    }
                    else if (ErrorHandling == ExceptionHandling.CancelProcessing)
                    {
                        break;
                    }
                }
            }

            if (hasResult)
            {
                resultCallback(result);
            }
        }
    }

    /// <summary>
    /// Async handler / processor for a chain-of-responsibility with return value.
    /// Returns a <see cref = "MethodResult{T}" /> in which the status of the processing (responsibility)
    /// and eventually the result is packed.
    /// </summary>
    /// <typeparam name = "TItem">The type of the item.</typeparam>
    /// <typeparam name = "TResult">The type of the result.</typeparam>
    /// <param name = "item">The item to process.</param>
    /// <returns>
    /// A <see cref = "MethodResult{T}"/> which contains the status of the processing
    /// in the <c>ReturnCode</c> (responsible, not responsible) and possibly the result in <c> Result </c>.
    /// </returns>
    public delegate Task<MethodResult<TResult>> AsyncChainOfResponsibilityResultHandler<TItem, TResult>(TItem item);
}