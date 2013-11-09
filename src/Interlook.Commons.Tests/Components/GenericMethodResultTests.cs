using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interlook.Components;
using Xunit;
using Xunit.Extensions;

namespace Interlook.Components.Tests
{
	public class GenericMethodResultTests
	{
		#region Constants

		private static readonly MethodResultTestObject SuccessObject = new MethodResultTestObject("My piece of cake.");
		private static readonly int ErrorCode = 303;
		private static readonly string ErrorMessage = "It failed definitely.";
		private static readonly Exception ErrorException = new InvalidOperationException("This was exceptionally wrong.");

		#endregion Constants

		#region Factory methods tests

		[Fact]
		public void CreateSuccessWithObjectTest()
		{
			var actual = MethodResult<MethodResultTestObject>.CreateSuccess(SuccessObject);

			Assert.Equal(MethodResult.CODE_SUCCESS, actual.ReturnCode);
			Assert.Equal<MethodResultTestObject>(SuccessObject, actual.Result);
		}

		[Fact]
		public void CreateFailedWithErrorCodeTest()
		{
			var actual = MethodResult<MethodResultTestObject>.CreateFailed(ErrorCode);
			Assert.Equal(ErrorCode, actual.ReturnCode);
		}

		[Fact]
		public void CreateFailedWithMessageTest()
		{
			var actual = MethodResult<MethodResultTestObject>.CreateFailed(ErrorMessage);

			Assert.Equal(ErrorMessage, actual.ReturnMessage);
			Assert.NotEqual<int>(MethodResult.CODE_SUCCESS, actual.ReturnCode);
		}

		[Fact]
		public void CreateFailedWithExceptionTest()
		{
			var actual = MethodResult<MethodResultTestObject>.CreateFailed(ErrorException);

			Assert.ThrowsDelegate action = () => actual.ThrowOnError();

			Assert.Throws(ErrorException.GetType(), action);
		}

		[Fact]
		public void CreateFailedWithCodeAndMessageTest()
		{
			var actual = MethodResult<MethodResultTestObject>.CreateFailed(ErrorCode, ErrorMessage);

			Assert.Equal(ErrorMessage, actual.ReturnMessage);
			Assert.Equal<int>(ErrorCode, actual.ReturnCode);
		}

		[Fact]
		public void CreateFailedWithCodeAndExceptionTest()
		{
			var actual = MethodResult<MethodResultTestObject>.CreateFailed(ErrorCode, ErrorException);

			Assert.ThrowsDelegate action = () => actual.ThrowOnError();

			Assert.Throws(ErrorException.GetType(), action);
			Assert.Equal<int>(ErrorCode, actual.ReturnCode);
		}

		#endregion Factory methods tests

		#region Behavior tests

		[Fact]
		public void SuccessResultDoesNotThrowTest()
		{
			var actual = MethodResult<MethodResultTestObject>.CreateSuccess(SuccessObject);

			Assert.ThrowsDelegate action = () => actual.ThrowOnError();
			Assert.DoesNotThrow(action);
		}

		[Fact]
		public void SuccessCastsToTrueImplicitelyTest()
		{
			var actual = MethodResult<MethodResultTestObject>.CreateSuccess(SuccessObject);

			bool result = false;
			result = actual;

			Assert.True(result, "Success MethodResult<MethodResultTestObject> did not cast to true implicitely.");
		}

		[Fact]
		public void SuccessCastsToTrueExplicitelyTest()
		{
			var actual = MethodResult<MethodResultTestObject>.CreateSuccess(SuccessObject);

			Assert.True((bool)actual, "Success MethodResult<MethodResultTestObject> did not cast to true explicitely.");
		}

		[Fact]
		public void SuccessCastsToResultExplicitelyTest()
		{
			var actual = MethodResult<MethodResultTestObject>.CreateSuccess(SuccessObject);
			Assert.Equal<MethodResultTestObject>(SuccessObject, actual);
		}

		[Fact]
		public void SuccessCastsToResultImplicitelyTest()
		{
			var actual = MethodResult<MethodResultTestObject>.CreateSuccess(SuccessObject);
			Assert.Equal(SuccessObject, (MethodResultTestObject)actual);
		}

		[Fact]
		public void SuccessCastsToNonGenericMethodResultImplicitelyTest()
		{
			var actual = MethodResult<MethodResultTestObject>.CreateSuccess(SuccessObject);

			MethodResult mr;
			mr = actual;

			Assert.Equal(MethodResult.CODE_SUCCESS, mr.ReturnCode);
			Assert.Equal<MethodResultTestObject>(SuccessObject, mr.Result as MethodResultTestObject);
		}

		[Fact]
		public void SuccessCastsToNonGenericMethodResultExplicitelyTest()
		{
			var actual = MethodResult<MethodResultTestObject>.CreateSuccess(SuccessObject);
			var mr = (MethodResult)actual;

			Assert.Equal(MethodResult.CODE_SUCCESS, mr.ReturnCode);
			Assert.Equal<MethodResultTestObject>(SuccessObject, mr.Result as MethodResultTestObject);
		}

		#endregion Behavior tests
	}
}