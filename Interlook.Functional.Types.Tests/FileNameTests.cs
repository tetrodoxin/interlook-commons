namespace Interlook.Functional.Types.Tests
{
    using Interlook.Functional.Types;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class FileNameTests
    {
        private FileName _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new FileName(new SomeString("TestValue1723425793"));
        }

        [Test]
        public void CanCallCreateWithString()
        {
            var name = "TestValue817876460";
            var result = FileName.Create(name);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateWithStringWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => FileName.Create(value));
        }

        [Test]
        public void CanCallCreateWithSomeString()
        {
            var name = new SomeString("TestValue1395336029");
            var result = FileName.Create(name);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallCreateWithSomeStringWithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => FileName.Create(default(SomeString)));
        }

        [Test]
        public void CanGetName()
        {
            Assert.That(_testClass.Name, Is.InstanceOf<SomeString>());
            Assert.Fail("Create or modify test");
        }
    }
}