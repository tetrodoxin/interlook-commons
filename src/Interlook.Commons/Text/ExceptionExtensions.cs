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
using System.Security.Cryptography;
using System.Text;

namespace Interlook.Text
{
    /// <summary>
    /// Extension methods for exceptions.
    /// </summary>
    public static class ExceptionExtensions
    {
        private const int DEFAULT_INNER_EXCEPTION_DEPTH = 10;

        /// <summary>
        /// Returns the complete, aggregated string representation of the exception
        /// including nested inner exceptions up to a recursion depth of 10.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns></returns>
        public static string ToCompleteString(this Exception ex) => ToCompleteString(ex, DEFAULT_INNER_EXCEPTION_DEPTH);

        /// <summary>
        /// Returns the complete, aggregated string representation of the exception
        /// including nested inner exceptions up to an given maximum depth.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="maxDepth">The maximum depth.</param>
        /// <returns></returns>
        public static string ToCompleteString(this Exception ex, int maxDepth) => exceptionString(ex, new List<Exception>(), 0, maxDepth);

        private static string exceptionString(Exception ex, ICollection<Exception> traversed, int deep, int maxDepth)
        {
            if (ex == null)
            {
                return String.Empty;
            }
            else if (deep >= maxDepth)
            {
                return "Reached max recursion depth for inner exceptions.";
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(ex.ToString());
                traversed.Add(ex);
                if (ex.InnerException != null && ex != ex.InnerException && !traversed.Contains(ex))
                {
                    var inner = exceptionString(ex.InnerException, traversed, deep + 1, maxDepth);
                    if (!inner.IsNullOrEmpty())
                    {
                        sb.AppendLine();
                        sb.AppendLine("---INNER EXCEPTION:");
                        sb.AppendLine(inner);
                    }
                }

                return sb.ToString();
            }
        }
    }
}