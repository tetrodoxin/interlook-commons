using Interlook.Functional.Types;
using System;
using Xunit;
using FluentAssertions;
using Interlook.Monads;

namespace Interlook.Functional.Types.UnitTests
{
    public class FileNameTests
    {
        private SomeString fileNameString;

        public FileNameTests()
        {
            fileNameString = StringBase.CreateSome("AnOrdinaryFile.pdf").GetRight();
        }

        [Fact]
        public void Create_FromSomeString()
        {
            var fileNameEither = FileName.Create(fileNameString);

            fileNameEither.Should().BeOfType<Right<Exception, FileName>>();
            var filename = fileNameEither.GetRight();
            filename.Should().NotBeNull();
            filename.Name.Should().Be(fileNameString);
        }

        [Fact]
        public void Create_FromString_NonEmpty()
        {
            string name = fileNameString.Value;
            var fileNameEither = FileName.Create(name);

            fileNameEither.Should().BeOfType<Right<Exception, FileName>>();
            var filename = fileNameEither.GetRight();
            filename.Should().NotBeNull();
            filename.Name.Value.Should().Be(name);
        }


        // OHNE DIE EXTENSIONS KEIN FREEZE MEHR BEIM BUILD


        [Fact]
        public void Create_EmptyString_Negative()
        {
            var name = string.Empty;
            var either = FileName.Create(name);
            either.Should().BeOfType<Left<Exception, FileName>>("empty string must not result in a FileName instance.");
        }
    }
}
