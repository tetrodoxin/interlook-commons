using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interlook.Text;
using System.Diagnostics.Contracts;

namespace Interlook.Components
{
	/// <summary>
	/// Class to encapsulate a method result, enriched with success(error information.
	/// </summary>
	public struct MethodResult
	{
		#region Constants

		public const int CODE_SUCCESS = 0;
		public const int CODE_ERROR_DEFAULT = -1;

		#endregion Constants

		#region Fields

		private Exception exception;

		#endregion Fields

		#region Properties

		/// <summary>
		/// The actual result.
		/// </summary>
		public object Result { get; private set; }

		/// <summary>
		/// An error message.
		/// </summary>
		public string ReturnMessage { get; private set; }

		/// <summary>
		/// Code to qualify the type of result (error, success)
		/// </summary>
		public int ReturnCode { get; private set; }

		#endregion Properties

		#region Constructors

		internal MethodResult(object result, int returnCode, string returnMessage, Exception exception)
			: this()
		{
			this.Result = result;
			this.ReturnCode = returnCode;
			this.ReturnMessage = returnMessage;
			this.exception = exception;
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
				if (exception != null)
				{
					throw exception;
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

		private static bool checkSuccess(MethodResult result)
		{
			return result.ReturnCode == CODE_SUCCESS;
		}

		#endregion Private Methods

		#region Static Factory Methods

		/// <summary>
		/// Creates a success result without content.
		/// </summary>
		/// <returns>A new instance of the closed generic result without data.</returns>
		public static MethodResult CreateSuccess()
		{
			return new MethodResult(null, CODE_SUCCESS, String.Empty, null);
		}

		/// <summary>
		/// Creates a success result with content.
		/// </summary>
		/// <param name="result">The actual result.</param>
		/// <returns>A new instance of the result with data.</returns>
		public static MethodResult CreateSuccess(object result)
		{
			return new MethodResult(result, CODE_SUCCESS, String.Empty, null);
		}

		/// <summary>
		/// Creates a failure result with error code.
		/// </summary>
		/// <param name="errorCode">Error code.</param>
		/// <returns>A new instance of the result with error code.</returns>
		public static MethodResult CreateFailed(int errorCode)
		{
			Contract.Requires<ArgumentException>(errorCode != MethodResult.CODE_SUCCESS, "This code is reserved for success");

			return new MethodResult(null, errorCode, String.Empty, null);
		}

		/// <summary>
		/// Creates a failure result with error message.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <returns>A new instance of the result with error message.</returns>
		public static MethodResult CreateFailed(string errorMessage)
		{
			return new MethodResult(null, CODE_ERROR_DEFAULT, errorMessage, null);
		}

		/// <summary>
		/// Creates a failure result with specified exception.
		/// </summary>
		/// <param name="ex">The exception to throw on <see cref="ThrowOnError()"/>.</param>
		/// <returns>A new instance of the result with specified exception.</returns>
		public static MethodResult CreateFailed(Exception ex)
		{
			if (ex != null)
			{
				return new MethodResult(null, CODE_ERROR_DEFAULT, ex.Message, ex);
			}
			else
			{
				return new MethodResult(null, CODE_ERROR_DEFAULT, String.Empty, null);
			}
		}

		/// <summary>
		/// Creates a failure result with error code and message.
		/// </summary>
		/// <param name="errorCode">The error code.</param>
		/// <param name="errorMessage">The error message.</param>
		/// <returns>A new instance of the result with error code and message.</returns>
		public static MethodResult CreateFailed(int errorCode, string errorMessage)
		{
			return new MethodResult(null, errorCode, errorMessage, null);

		}

		/// <summary>
		/// Creates a failure result with error code and specified exception.
		/// </summary>
		/// <param name="errorCode">The error code.</param>
		/// <param name="ex">The exception to throw on <see cref="ThrowOnError()"/>.</param>
		/// <returns>A new instance of the closed generic result with error code and exception.</returns>
		public static MethodResult CreateFailed(int errorCode, Exception ex)
		{
			if (ex != null)
			{
				return new MethodResult(null, errorCode, ex.Message, ex);
			}
			else
			{
				return new MethodResult(null, errorCode, String.Empty, null);
			}
		}

		#endregion Static Factory Methods

		#region Implicit Cast Operators

		public static implicit operator bool(MethodResult result)
		{
			return checkSuccess(result);
		}

		#endregion Implicit Cast Operators
	}
}