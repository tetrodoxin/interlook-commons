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

#endregion license

using System;

using System.Text;

namespace Interlook.Components
{
    /// <summary>
    /// Class to encapsulate a method result, enriched with success(error information.
    /// </summary>
    public struct MethodResult
    {
        #region Constants

        /// <summary>
        /// The code for a successful method call
        /// </summary>
        public const int CodeSuccess = 0;

        /// <summary>
        /// The default code for an error.
        /// </summary>
        public const int CodeErrorDefault = -1;

        #endregion Constants

        #region Fields

        private readonly Exception _exception;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The actual result.
        /// </summary>
        public object Result { get; }

        /// <summary>
        /// An error message.
        /// </summary>
        public string ReturnMessage { get; }

        /// <summary>
        /// Code to qualify the type of result (error, success)
        /// </summary>
        public int ReturnCode { get; }

        #endregion Properties

        #region Constructors

        internal MethodResult(object result, int returnCode, string returnMessage, Exception exception)
            : this()
        {
            Result = result;
            ReturnCode = returnCode;
            ReturnMessage = returnMessage;
            _exception = exception;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Throws an exception, if and only if the method result was an error
        /// </summary>
        public void ThrowOnError()
        {
            if (checkSuccess(this) == false)
            {
                if (_exception != null)
                {
                    throw _exception;
                }
                else
                {
                    var sb = new StringBuilder("Method Failed. ")
                        .Append($"Return code = {ReturnCode}");

                    if (!string.IsNullOrEmpty(ReturnMessage))
                    {
                        sb.Append($", Return message = '{ReturnMessage}'");
                    }

                    throw new Exception(sb.ToString());
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static bool checkSuccess(MethodResult result) => result.ReturnCode == CodeSuccess;

        #endregion Private Methods

        #region Static Factory Methods

        /// <summary>
        /// Creates a success result without content.
        /// </summary>
        /// <returns>A new instance of the closed generic result without data.</returns>
        public static MethodResult CreateSuccess() => new MethodResult(null, CodeSuccess, string.Empty, null);

        /// <summary>
        /// Creates a success result with content.
        /// </summary>
        /// <param name="result">The actual result.</param>
        /// <returns>A new instance of the result with data.</returns>
        public static MethodResult CreateSuccess(object result) => new MethodResult(result, CodeSuccess, string.Empty, null);

        /// <summary>
        /// Creates a failure result with error code.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <returns>A new instance of the result with error code.</returns>
        /// <exception cref="ArgumentException">If <paramref name="errorCode"/> was set to <see cref="CodeSuccess"/>,
        /// which is reserved for successful invocations.</exception>
        public static MethodResult CreateFailed(int errorCode)
        {
            if (errorCode == CodeSuccess) throw new ArgumentException("This code is reserved for success", nameof(errorCode));

            return new MethodResult(null, errorCode, string.Empty, null);
        }

        /// <summary>
        /// Creates a failure result with error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>A new instance of the result with error message.</returns>
        public static MethodResult CreateFailed(string errorMessage) => new MethodResult(null, CodeErrorDefault, errorMessage, null);

        /// <summary>
        /// Creates a failure result with specified exception.
        /// </summary>
        /// <param name="ex">The exception to throw on <see cref="ThrowOnError()"/>.</param>
        /// <returns>A new instance of the result with specified exception.</returns>
        public static MethodResult CreateFailed(Exception ex)
            => ex != null
                ? new MethodResult(null, CodeErrorDefault, ex.Message, ex)
                : new MethodResult(null, CodeErrorDefault, string.Empty, null);

        /// <summary>
        /// Creates a failure result with error code and message.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>A new instance of the result with error code and message.</returns>
        public static MethodResult CreateFailed(int errorCode, string errorMessage) => new MethodResult(null, errorCode, errorMessage, null);

        /// <summary>
        /// Creates a failure result with error code and specified exception.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="ex">The exception to throw on <see cref="ThrowOnError()"/>.</param>
        /// <returns>A new instance of the closed generic result with error code and exception.</returns>
        public static MethodResult CreateFailed(int errorCode, Exception ex)
            => ex != null ? new MethodResult(null, errorCode, ex.Message, ex) : new MethodResult(null, errorCode, string.Empty, null);

        #endregion Static Factory Methods

        #region Implicit Cast Operators

        /// <summary>
        /// Performs an implicit conversion from <see cref="MethodResult"/> to <see cref="bool"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// A <see cref="bool"/> value indicating, if the method result encapsulates a successful invocation.
        /// </returns>
        public static implicit operator bool(MethodResult result) => checkSuccess(result);

        #endregion Implicit Cast Operators
    }
}