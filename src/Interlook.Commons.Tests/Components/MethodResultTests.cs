using System;
using Xunit;

namespace Interlook.Components.Tests

{
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

            var ex = Assert.Throws<InvalidOperationException>(() => actual.ThrowOnError());
            Assert.Equal(ErrorException.Message, ex.Message);
        }

        [Fact]
        public void CreateFailedWithCodeAndMessageTest()
        {
            var actual = MethodResult.CreateFailed(ErrorCode, ErrorMessage);

            Assert.Equal(ErrorMessage, actual.ReturnMessage);
            Assert.Equal(ErrorCode, actual.ReturnCode);
        }

        [Fact]
        public void CreateFailedWithCodeAndExceptionTest()
        {
            var actual = MethodResult.CreateFailed(ErrorCode, ErrorException);

            var ex = Assert.Throws<InvalidOperationException>(() => actual.ThrowOnError());
            Assert.Equal(ErrorException.Message, ex.Message);
            Assert.Equal(ErrorCode, actual.ReturnCode);
        }

        #endregion Factory methods tests

        #region Behavior tests

        [Fact]
        public void SuccessResultDoesNotThrowTest()
        {
            var actual = MethodResult.CreateSuccess(SuccessObject);

            actual.ThrowOnError();
        }

        [Fact]
        public void SuccessCastsToTrueImplicitelyTest()
        {
            var actual = MethodResult.CreateSuccess(SuccessObject);

            Assert.True(actual, "Success MethodResult did not cast to true implicitely.");
        }

        [Fact]
        public void SuccessCastsToTrueExplicitelyTest()
        {
            var actual = MethodResult.CreateSuccess(SuccessObject);

            Assert.True((bool)actual, "Success MethodResult did not cast to true explicitely.");
        }

        #endregion Behavior tests
    }
}