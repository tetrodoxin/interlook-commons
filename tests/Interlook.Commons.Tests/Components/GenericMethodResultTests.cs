using System;
using Xunit;

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

            Assert.Equal(MethodResult.CodeSuccess, actual.ReturnCode);
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
            Assert.NotEqual(MethodResult.CodeSuccess, actual.ReturnCode);
        }

        [Fact]
        public void CreateFailedWithExceptionTest()
        {
            var actual = MethodResult<MethodResultTestObject>.CreateFailed(ErrorException);

            var ex = Assert.Throws<InvalidOperationException>(() => actual.ThrowOnError());
            Assert.Equal(ErrorException.Message, ex.Message);
        }

        [Fact]
        public void CreateFailedWithCodeAndMessageTest()
        {
            var actual = MethodResult<MethodResultTestObject>.CreateFailed(ErrorCode, ErrorMessage);

            Assert.Equal(ErrorMessage, actual.ReturnMessage);
            Assert.Equal(ErrorCode, actual.ReturnCode);
        }

        [Fact]
        public void CreateFailedWithCodeAndExceptionTest()
        {
            var actual = MethodResult<MethodResultTestObject>.CreateFailed(ErrorCode, ErrorException);

            var ex = Assert.Throws<InvalidOperationException>(() => actual.ThrowOnError());
            Assert.Equal(ErrorException.Message, ex.Message);
            Assert.Equal(ErrorCode, actual.ReturnCode);
        }

        #endregion Factory methods tests

        #region Behavior tests

        [Fact]
        public void SuccessResultDoesNotThrowTest()
        {
            var actual = MethodResult<MethodResultTestObject>.CreateSuccess(SuccessObject);

            actual.ThrowOnError();
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

            Assert.Equal(MethodResult.CodeSuccess, mr.ReturnCode);
            Assert.Equal<MethodResultTestObject>(SuccessObject, mr.Result as MethodResultTestObject);
        }

        [Fact]
        public void SuccessCastsToNonGenericMethodResultExplicitelyTest()
        {
            var actual = MethodResult<MethodResultTestObject>.CreateSuccess(SuccessObject);
            var mr = (MethodResult)actual;

            Assert.Equal(MethodResult.CodeSuccess, mr.ReturnCode);
            Assert.Equal<MethodResultTestObject>(SuccessObject, mr.Result as MethodResultTestObject);
        }

        #endregion Behavior tests
    }
}