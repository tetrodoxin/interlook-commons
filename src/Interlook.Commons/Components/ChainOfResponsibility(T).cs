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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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
    public class ChainOfResponsibility<TItem>
    {
        private Dictionary<ChainOfResponsibilityHandler<TItem>, Priority> _handlers = new Dictionary<ChainOfResponsibilityHandler<TItem>, Priority>();

        /// <summary>
        /// Specifies how to handle exceptions in handlers when processing.
        /// </summary>
        public ExceptionHandling ExceptionHandling { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChainOfResponsibility{T}"/> class.
        /// </summary>
        public ChainOfResponsibility()
        {
            ExceptionHandling = ExceptionHandling.IgnoreAndContinue;
        }

        /// <summary>
        /// Adds an processor (handler) to the chain.
        /// </summary>
        /// <param name = "handler"> The handler to be added. </param>
        /// <param name = "priority"> The priority of the handler to add. </param>
        public void AddHandler(ChainOfResponsibilityHandler<TItem> handler, Priority priority)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            _handlers[handler] = priority;
        }

        /// <summary>
        /// Adds an processor (handler) to the chain.
        /// </summary>
        /// <param name = "handler"> The handler to be added. </param>
        public void AddHandler(ChainOfResponsibilityHandler<TItem> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            AddHandler(handler, Priority.Normal);
        }

        /// <summary>
        /// Passes an item to the chain for processing.
        /// </summary>
        /// <param name = "itemToProcess"> The item to process. </param>
        /// <returns> <c> true </c> if the item has been processed, otherwise <c> false </c>. </returns>
        public bool Process(TItem itemToProcess)
        {
            var handlers = _handlers.OrderByDescending(p => p.Value)
                .ToList();

            foreach (var handler in handlers)
            {
                try
                {
                    var handled = handler.Key(itemToProcess);
                    if (handled)
                    {
                        return true;
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
                        return false;
                    }
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Handler/Processor for a Chain-Of-Responsibility.
    /// Returns <c> true </c> if it was responsible for processing.
    /// </summary>
    /// <typeparam name = "TItem"> Data type of the item to be processed. </typeparam>
    /// <param name = "item"> The item to process. </param>
    /// <returns> <c> true </c> if the item has been processed and the chain can be terminated. </returns>
    public delegate bool ChainOfResponsibilityHandler<TItem>(TItem item);

    /// <summary>
    /// Enumeration of modes for dealing with exceptions in handlers
    /// </summary>
    public enum ExceptionHandling
    {
        /// <summary>
        /// Ignore exception and continue with next handler.
        /// </summary>
        IgnoreAndContinue,

        /// <summary>
        /// Abort processing and return as unhandled.
        /// </summary>
        CancelProcessing,

        /// <summary>
        /// Processing is aborted by an exception.
        /// </summary>
        ThrowException
    }

    /// <summary>
    /// Enumeration of processing priorities
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// Highest
        /// </summary>
        High = 2,

        /// <summary>
        /// Higher
        /// </summary>
        AboveNormal = 1,

        /// <summary>
        /// Normal
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Lower
        /// </summary>
        BelowNormal = -1,

        /// <summary>
        /// Lowest
        /// </summary>
        Low = -2
    }
}