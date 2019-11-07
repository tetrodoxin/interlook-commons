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
    /// </ summary>
    /// <typeparam name = "TItem">Data type to be processed</ typeparam>
    public class AsyncChainOfResponsibility<TItem>
    {
        private Dictionary<AsyncChainOfResponsibilityHandler<TItem>, Priority> _handlers
            = new Dictionary<AsyncChainOfResponsibilityHandler<TItem>, Priority>();

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
        public void AddHandler(AsyncChainOfResponsibilityHandler<TItem> handler, Priority priority)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            _handlers[handler] = priority;
        }

        /// <summary>
        /// Adds an processor (handler) to the chain.
        /// </summary>
        /// <param name = "handler">The handler to be added. </param>
        public void AddHandler(AsyncChainOfResponsibilityHandler<TItem> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            AddHandler(handler, Priority.Normal);
        }

        /// <summary>
        /// Passes an item to the chain for processing.
        /// </summary>
        /// <param name = "itemToProcess">The item to process. </param>
        public async Task ProcessAsync(TItem itemToProcess)
        {
            await ProcessAsync(itemToProcess, null, null);
        }

        /// <summary>
        /// Passes an item to the chain for processing.
        /// </summary>
        /// <param name = "itemToProcess">The item to process. </param>
        /// <param name="resultCallback">Optional. Callback, invoked with processed-state (<c>true/false</c>) when chain has been finished without errors.</param>
        public async Task ProcessAsync(TItem itemToProcess, Action<bool> resultCallback)
        {
            await ProcessAsync(itemToProcess, resultCallback, null);
        }

        /// <summary>
        /// Passes an item to the chain for processing.
        /// </summary>
        /// <param name = "itemToProcess">The item to process. </param>
        /// <param name="resultCallback">Optional. Callback, invoked with processed-state (<c>true/false</c>) when chain has been finished without errors.</param>
        /// <param name="errorCallback">Optional. Callback, to be invoked if an exception occurs (if <see cref="ExceptionHandling"/> is set accordningly),
        /// independent of <c>resultCallback</c>.</param>
        public async Task ProcessAsync(TItem itemToProcess, Action<bool> resultCallback, Action<Exception> errorCallback)
        {
            var handlers = _handlers.OrderByDescending(p => p.Value)
                .ToList();

            var result = false;

            foreach (var handler in handlers)
            {
                try
                {
                    var handled = await handler.Key(itemToProcess);
                    if (handled)
                    {
                        result = true;
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

            resultCallback?.Invoke(result);
        }
    }

    /// <summary>
    /// Async Handler/Processor for a Chain-Of-Responsibility.
    /// Returns <c>true</c> if it was responsible for processing.
    /// </ summary>
    /// <typeparam name = "TItem"> Data type of the item to be processed. </ typeparam>
    /// <param name = "item"> The item to process. </ param>
    /// <returns> <c> true </ c> if the item has been processed and the chain can be terminated. </ returns>
    public delegate Task<bool> AsyncChainOfResponsibilityHandler<TItem>(TItem item);
}