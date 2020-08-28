using Interlook.Functional.Types;
using System;
using Xunit;
using FluentAssertions;
using Interlook.Monads;
using System.IO;

namespace Interlook.Functional.Types.UnitTests
{
    public class FileNameTests
    {
        private SomeString fileNameString;

        public FileNameTests()
        {
            fileNameString = SomeString.Create("AnOrdinaryFile.pdf").GetRight();
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


        [Fact]
        public void Create_FromEmptyString_Negative()
        {
            var name = string.Empty;
            var either = FileName.Create(name);
            either.Should().BeOfType<Left<Exception, FileName>>("empty string must not result in a FileName instance.");
        }

        [Fact]
        public void Create_FromString_WithInvalidChars_Negative()
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                testCreate_WithInvalidCharacter_FromString(c);
            }
        }

        [Fact]
        public void Create_FromSomeString_WithInvalidChars_Negative()
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                testCreate_WithInvalidCharacter_FromSomeString(c);
            }
        }

        private static void testCreate_WithInvalidCharacter_FromString(char testedCharacter)
        {
            string name = $"prefix{testedCharacter}suffix.txt";
            var either = FileName.Create(name);
            either.Should().BeOfType<Left<Exception, FileName>>($"the string '{name}' with invalid character {testedCharacter} must not result in a FileName instance.");
        }

        private static void testCreate_WithInvalidCharacter_FromSomeString(char testedCharacter)
        {
            SomeString nameSomeString = SomeString.Create("prefix").GetRight().Append(testedCharacter).Concat("suffix.txt");
            var either = FileName.Create(nameSomeString);
            either.Should().BeOfType<Left<Exception, FileName>>($"the string '{nameSomeString}' with invalid character {testedCharacter} must not result in a FileName instance.");
        }
    }
}
