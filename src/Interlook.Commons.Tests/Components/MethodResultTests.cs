using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interlook.Components;
using Xunit;
using Xunit.Extensions;

public class MethodResultTests
{
	#region Constants

	private static readonly string SuccessObject = "I am legend.";
	private static readonly int SuccessInt32Object = 42;
	private static readonly int ErrorCode = 303;
	private static readonly string ErrorMessage = "It failed definitely.";
	private static readonly Exception ErrorException = new InvalidOperationException("This was exceptionally wrong.");

	#endregion Constants

	#region Factory methods tests

	[Fact]
	public void CreateSuccessTest()
	{
		var actual = MethodResult.CreateSuccess();

		Assert.Equal(MethodResult.CODE_SUCCESS, actual.ReturnCode);
	}

	[Fact]
	public void CreateSuccessWithObjectTest()
	{
		var actual = MethodResult.CreateSuccess(SuccessObject);

		Assert.Equal(MethodResult.CODE_SUCCESS, actual.ReturnCode);
		Assert.Equal(SuccessObject, actual.Result);

		actual = MethodResult.CreateSuccess(SuccessInt32Object);

		Assert.Equal(MethodResult.CODE_SUCCESS, actual.ReturnCode);
		Assert.Equal(SuccessInt32Object, actual.Result);
	}

	[Fact]
	public void CreateFailedWithErrorCodeTest()
	{
		var actual = MethodResult.CreateFailed(ErrorCode);
		Assert.Equal(ErrorCode, actual.ReturnCode);
	}

	[Fact]
	public void CreateFailedWithMessageTest()
	{
		var actual = MethodResult.CreateFailed(ErrorMessage);

		Assert.Equal(ErrorMessage, actual.ReturnMessage);
		Assert.NotEqual<int>(MethodResult.CODE_SUCCESS, actual.ReturnCode);
	}

	[Fact]
	public void CreateFailedWithExceptionTest()
	{
		var actual = MethodResult.CreateFailed(ErrorException);

		Assert.ThrowsDelegate action = () => actual.ThrowOnError();

		Assert.Throws(ErrorException.GetType(), action);
	}

	[Fact]
	public void CreateFailedWithCodeAndMessageTest()
	{
		var actual = MethodResult.CreateFailed(ErrorCode, ErrorMessage);

		Assert.Equal(ErrorMessage, actual.ReturnMessage);
		Assert.Equal<int>(ErrorCode, actual.ReturnCode);
	}

	[Fact]
	public void CreateFailedWithCodeAndExceptionTest()
	{
		var actual = MethodResult.CreateFailed(ErrorCode, ErrorException);

		Assert.ThrowsDelegate action = () => actual.ThrowOnError();

		Assert.Throws(ErrorException.GetType(), action);
		Assert.Equal<int>(ErrorCode, actual.ReturnCode);
	}

	#endregion Factory methods tests

	#region Behavior tests

	[Fact]
	public void SuccessResultDoesNotThrowTest()
	{
		var actual = MethodResult.CreateSuccess(SuccessObject);

		Assert.ThrowsDelegate action = () => actual.ThrowOnError();
		Assert.DoesNotThrow(action);
	}

	[Fact]
	public void SuccessCastsToTrueImplicitelyTest()
	{
		var actual = MethodResult.CreateSuccess(SuccessObject);

		bool result = false;
		result = actual;

		Assert.True(result, "Success MethodResult did not cast to true implicitely.");
	}

	[Fact]
	public void SuccessCastsToTrueExplicitelyTest()
	{
		var actual = MethodResult.CreateSuccess(SuccessObject);

		Assert.True((bool)actual, "Success MethodResult did not cast to true explicitely.");
	}

	#endregion Behavior tests
}