#region license

//MIT License

//Copyright(c) 2016 Andreas Huebner

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
using System.Text;
using Interlook.Text;

namespace Interlook.Components
{
    /// <summary>
    /// Generic class to encapsulate a method result, enriched with success(error information.
    /// </summary>
    public struct MethodResult<T>
    {
        public const int CodeSuccess = 0;

        public const int CodeErrorDefault = -1;

        #region Fields

        private Exception _exception;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The actual result.
        /// </summary>
        public T Result { get; private set; }

        /// <summary>
        /// An error message.
        /// </summary>
        public string ReturnMessage { get; private set; }

        /// <summary>
        /// Code to qualify the type of result (error, success)
        /// </summary>
        public int ReturnCode { get; private set; }

        /// <summary>
        /// Returns, if the the method result reflects an successful invocation
        /// </summary>
        public bool IsSuccess => ReturnCode == CodeSuccess;

        #endregion Properties

        #region Constructors

        internal MethodResult(T result, int returnCode, string returnMessage, Exception exception)
            : this()
        {
            this.Result = result;
            this.ReturnCode = returnCode;
            this.ReturnMessage = returnMessage;
            this._exception = exception;
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
                    StringBuilder sb = new StringBuilder("Method Failed. ")
                        .AppendFormat("Return code = {0}", this.ReturnCode);

                    if (this.ReturnMessage.AintNullNorEmpty())
                    {
                        sb.AppendFormat(", Return message = '{0}'", this.ReturnMessage);
                    }

                    throw new Exception(sb.ToString());
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static bool checkSuccess(MethodResult<T> result)
        {
            return result.ReturnCode == MethodResult.CODE_SUCCESS;
        }

        #endregion Private Methods

        #region Static Factory Methods

        /// <summary>
        /// Creates a success result with content.
        /// </summary>
        /// <param name="result">The actual result.</param>
        /// <returns>A new instance of the closed generic result with data.</returns>
        public static MethodResult<T> CreateSuccess(T result)
        {
            return new MethodResult<T>(result, MethodResult.CODE_SUCCESS, String.Empty, null);
        }

        /// <summary>
        /// Creates a failure result with error code.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <returns>A new instance of the closed generic result with error code.</returns>
        public static MethodResult<T> CreateFailed(int errorCode)
        {
            if (errorCode == MethodResult.CODE_SUCCESS) throw new ArgumentException("This code is reserved for success", nameof(errorCode));

            return new MethodResult<T>(default(T), errorCode, String.Empty, null);
        }

        /// <summary>
        /// Creates a failure result with error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>A new instance of the closed generic result with error message.</returns>
        public static MethodResult<T> CreateFailed(string errorMessage)
        {
            return new MethodResult<T>(default(T), MethodResult.CODE_ERROR_DEFAULT, errorMessage, null);
        }

        /// <summary>
        /// Creates a failure result with specified exception.
        /// </summary>
        /// <param name="ex">The exception to throw on <see cref="ThrowOnError()"/>.</param>
        /// <returns>A new instance of the closed generic result with specified exception.</returns>
        public static MethodResult<T> CreateFailed(Exception ex)
        {
            if (ex != null)
            {
                var r = CreateFailed(MethodResult.CODE_ERROR_DEFAULT, ex.Message);
                r._exception = ex;
                return r;
            }
            else
            {
                return CreateFailed(MethodResult.CODE_ERROR_DEFAULT, String.Empty);
            }
        }

        /// <summary>
        /// Creates a failure result with error code and message.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>A new instance of the closed generic result with error code and message.</returns>
        public static MethodResult<T> CreateFailed(int errorCode, string errorMessage)
        {
            if (errorCode == MethodResult.CODE_SUCCESS) throw new ArgumentException("This code is reserved for success", nameof(errorCode));

            var r = new MethodResult<T>(default(T), errorCode, errorMessage, null);
            r.ReturnCode = errorCode;
            r.ReturnMessage = errorMessage;

            return r;
        }

        /// <summary>
        /// Creates a failure result with error code and specified exception.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="ex">The exception to throw on <see cref="ThrowOnError()"/>.</param>
        /// <returns>A new instance of the closed generic result with error code and exception.</returns>
        public static MethodResult<T> CreateFailed(int errorCode, Exception ex)
        {
            if (errorCode == MethodResult.CODE_SUCCESS) throw new ArgumentException("This code is reserved for success", nameof(errorCode));

            return ex != null
                ? new MethodResult<T>(default(T), errorCode, ex.Message, ex)
                : new MethodResult<T>(default(T), errorCode, String.Empty, null);
        }

        #endregion Static Factory Methods

        #region Implicit Cast Operators

        public static implicit operator T(MethodResult<T> result)
        {
            return result.Result;
        }

        public static implicit operator MethodResult<T>(T parameter)
        {
            return CreateSuccess(parameter);
        }

        public static implicit operator bool(MethodResult<T> result)
        {
            return checkSuccess(result);
        }

        public static implicit operator MethodResult(MethodResult<T> result)
        {
            var r = new MethodResult(result.Result, result.ReturnCode, result.ReturnMessage, null);
            return r;
        }

        public static implicit operator MethodResult<T>(MethodResult result)
        {
            if (result.Result is T)
            {
                return CreateSuccess((T)result.Result);
            }
            else
            {
                return CreateFailed(result.ReturnCode, result.ReturnMessage);
            }
        }

        #endregion Implicit Cast Operators
    }
}