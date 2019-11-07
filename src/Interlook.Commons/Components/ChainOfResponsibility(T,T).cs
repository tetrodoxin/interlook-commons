using System;
using System.Collections.Generic;
using System.Linq;

namespace Interlook.Components
{
    /// <summary>
    /// handler / processor for a chain-of-responsibility with return value.
    /// Returns a <see cref = "MethodResult{T}" /> in which the status of the processing (responsibility)
    /// and eventually the result is packed.
    /// </summary>
    /// <typeparam name = "TItem">The type of the item.</typeparam>
    /// <typeparam name = "TResult">The type of the result.</typeparam>
    /// <param name = "item">The item to process.</ param>
    /// <returns>
    /// A <see cref = "MethodResult{T}"/> which contains the status of the processing
    /// in the <c>ReturnCode</c> (responsible, not responsible) and possibly the result in <c> Result </c>.
    /// </returns>
    public delegate MethodResult<TResult> ChainOfResponsibilityResultHandler<TItem, TResult>(TItem item);

    /// <summary>
    /// A chain-of-responsibility implementation as a queue of delegates.
    /// An item is passed on for processing until a handler signals,
    /// that it has processed the item.
    /// Handlers can optionally be added with priorities, but not placed
    /// in a certain position in the chain.
    /// </summary>
    /// <typeparam name = "TItem">The type of the item. </typeparam>
    /// <typeparam name = "TResult">The type of the result. </typeparam>
    public class ChainOfResponsibility<TItem, TResult>
    {
        private Dictionary<ChainOfResponsibilityResultHandler<TItem, TResult>, Priority> _handlers 
            = new Dictionary<ChainOfResponsibilityResultHandler<TItem, TResult>, Priority>();

        /// <summary>
        /// Specifies how to handle exceptions in handlers when processing.
        /// </summary>
        public ExceptionHandling ExceptionHandling { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChainOfResponsibility{TItem, TResult}"/> class.
        /// </summary>
        public ChainOfResponsibility()
        {
            ExceptionHandling = ExceptionHandling.IgnoreAndContinue;
        }

        /// <summary>
        /// Adds an processor (handler) to the chain.
        /// </summary>
        /// <param name = "handler"> The handler to be added. </ param>
        /// <param name = "priority"> The priority of the handler to add. </ param>
        public void AddHandler(ChainOfResponsibilityResultHandler<TItem, TResult> handler, Priority priority)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            _handlers[handler] = priority;
        }

        /// <summary>
        /// Adds an processor (handler) to the chain.
        /// </summary>
        /// <param name = "handler"> The handler to be added. </param>
        public void AddHandler(ChainOfResponsibilityResultHandler<TItem, TResult> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            AddHandler(handler, Priority.Normal);
        }

        /// <summary>
        /// Passes an item to the chain for processing.
        /// </summary>
        /// <param name = "itemToProcess"> The item to process. </ param>
        /// <returns> The result object or <c> default (TResult) </ c> if no handler was responsible. </ returns>
        public TResult Process(TItem itemToProcess)
        {
            var handlers = _handlers.OrderByDescending(p => p.Value)
                .ToList();

            foreach (var handler in handlers)
            {
                try
                {
                    var result = handler.Key(itemToProcess);
                    if (result.IsSuccess)
                    {
                        return result;
                    }
                }
                catch
                {
                    if (ExceptionHandling == ExceptionHandling.ThrowException)
                    {
                        throw;
                    }
                    else if (ExceptionHandling == ExceptionHandling.CancelProcessing)
                    {
                        return default(TResult);
                    }
                }
            }

            return default(TResult);
        }
    }
}