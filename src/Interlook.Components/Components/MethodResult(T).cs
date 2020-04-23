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
    /// Generic class to encapsulate a method result, enriched with success(error information.
    /// </summary>
    /// <typeparam name="T">Data type of the eccapsulated return value.</typeparam>
    public struct MethodResult<T>
    {
        #region Fields

        private readonly Exception _exception;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The actual result.
        /// </summary>
        public T Result { get; }

        /// <summary>
        /// An error message.
        /// </summary>
        public string ReturnMessage { get; }

        /// <summary>
        /// Code to qualify the type of result (error, success)
        /// </summary>
        public int ReturnCode { get; }

        /// <summary>
        /// Returns, if the the method result reflects an successful invocation
        /// </summary>
        public bool IsSuccess => ReturnCode == MethodResult.CodeSuccess;

        #endregion Properties

        #region Constructors

        internal MethodResult(T result, int returnCode, string returnMessage, Exception exception)
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
                    StringBuilder sb = new StringBuilder("Method Failed. ")
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

        private static bool checkSuccess(MethodResult<T> result) => result.ReturnCode == MethodResult.CodeSuccess;

        #endregion Private Methods

        #region Static Factory Methods

        /// <summary>
        /// Creates a success result with content.
        /// </summary>
        /// <param name="result">The actual result.</param>
        /// <returns>A new instance of the closed generic result with data.</returns>
        public static MethodResult<T> CreateSuccess(T result)
        {
            return new MethodResult<T>(result, MethodResult.CodeSuccess, string.Empty, null);
        }

        /// <summary>
        /// Creates a failure result with error code.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <returns>A new instance of the closed generic result with error code.</returns>
        public static MethodResult<T> CreateFailed(int errorCode)
        {
            if (errorCode == MethodResult.CodeSuccess) throw new ArgumentException("This code is reserved for success", nameof(errorCode));

            return new MethodResult<T>(default(T), errorCode, string.Empty, null);
        }

        /// <summary>
        /// Creates a failure result with error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>A new instance of the closed generic result with error message.</returns>
        public static MethodResult<T> CreateFailed(string errorMessage) => new MethodResult<T>(default, MethodResult.CodeErrorDefault, errorMessage, null);

        /// <summary>
        /// Creates a failure result with specified exception.
        /// </summary>
        /// <param name="ex">The exception to throw on <see cref="ThrowOnError()"/>.</param>
        /// <returns>A new instance of the closed generic result with specified exception.</returns>
        public static MethodResult<T> CreateFailed(Exception ex) 
            => ex != null
                ? (MethodResult<T>)new MethodResult(default, MethodResult.CodeErrorDefault, ex.Message, ex)
                : CreateFailed(MethodResult.CodeErrorDefault, string.Empty);

        /// <summary>
        /// Creates a failure result with error code and message.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>A new instance of the closed generic result with error code and message.</returns>
        public static MethodResult<T> CreateFailed(int errorCode, string errorMessage)
        {
            if (errorCode == MethodResult.CodeSuccess) throw new ArgumentException("This code is reserved for success", nameof(errorCode));


            return new MethodResult<T>(default(T), errorCode, errorMessage, null);
        }

        /// <summary>
        /// Creates a failure result with error code and specified exception.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="ex">The exception to throw on <see cref="ThrowOnError()"/>.</param>
        /// <returns>A new instance of the closed generic result with error code and exception.</returns>
        public static MethodResult<T> CreateFailed(int errorCode, Exception ex)
        {
            if (errorCode == MethodResult.CodeSuccess) throw new ArgumentException("This code is reserved for success", nameof(errorCode));

            return ex != null
                ? new MethodResult<T>(default(T), errorCode, ex.Message, ex)
                : new MethodResult<T>(default(T), errorCode, string.Empty, null);
        }

        #endregion Static Factory Methods

        #region Implicit Cast Operators

        /// <summary>
        /// Performs an implicit conversion from <see cref="MethodResult{T}"/> to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator T(MethodResult<T> result) => result.Result;

        /// <summary>
        /// Performs an implicit conversion from <typeparamref name="T"/> to <see cref="MethodResult{T}"/>.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// A <see cref="MethodResult{T}"/> instance, representing a successfull
        /// method invocation with a result of type <typeparamref name="T"/>
        /// </returns>
        public static implicit operator MethodResult<T>(T parameter) => CreateSuccess(parameter);

        /// <summary>
        /// Performs an implicit conversion from <see cref="MethodResult{T}"/> to <see cref="bool"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// A <see cref="bool"/> value indicating, if the method result encapsulates a successful invocation.
        /// </returns>
        public static implicit operator bool(MethodResult<T> result) => checkSuccess(result);

        /// <summary>
        /// Performs an implicit conversion from <see cref="MethodResult{T}"/> to a non-generic <see cref="MethodResult"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// The non-generic version of <paramref name="result"/>.
        /// </returns>
        public static implicit operator MethodResult(MethodResult<T> result) => new MethodResult(result.Result, result.ReturnCode, result.ReturnMessage, null);

        /// <summary>
        /// Tries to perform an implicit conversion from non-generic <see cref="MethodResult"/> to the generic <see cref="MethodResult{T}"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// An generic version of <paramref name="result"/>.
        /// <para>
        /// Notice, that an instance of <see cref="MethodResult"/>, actually
        /// representing a success will be converted to a failed generic representation, if the <see cref="MethodResult.Result"/>
        /// property of <paramref name="result"/> was not of type <typeparamref name="T"/> (which is also the case for <c>null</c>)
        /// </para>
        /// </returns>
        public static implicit operator MethodResult<T>(MethodResult result)
            => result.Result is T && result.ReturnCode == MethodResult.CodeSuccess ? CreateSuccess((T)result.Result) : CreateFailed(result.ReturnCode, result.ReturnMessage);

        #endregion Implicit Cast Operators
    }
}