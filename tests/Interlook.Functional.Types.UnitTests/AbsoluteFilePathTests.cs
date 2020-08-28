using FluentAssertions;
using Interlook.Monads;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Interlook.Functional.Types.UnitTests
{
    public class AbsoluteFilePathTests
    {
        private const string DirName = @"c:\correct\dir\path";
        private const string FileName = @"fileName.txt";
        private static SomeString DirSomeString = SomeString.Create(DirName).GetRight();
        private static SomeString FileSomeString = SomeString.Create(FileName).GetRight();

        [Fact]
        public void Directory_Property()
        {
            var fullPathSomeString = DirSomeString.Append(Path.DirectorySeparatorChar).Concat(FileSomeString);
            var resultPath = AbsoluteFilePath.Return(fullPathSomeString).GetRight();

            resultPath.Should().BeOfType<AbsoluteFilePath>();
            var expectedDirName = DirSomeString.Append(Path.DirectorySeparatorChar);
            resultPath.Directory.Path.Value.Should().Be(expectedDirName);
        }

        [Fact]
        public void ReturnFilePath_FromSomeString()
        {
            var fullPathSomeString = DirSomeString.Append(Path.DirectorySeparatorChar).Concat(FileSomeString);
            var resultPath = AbsoluteFilePath.Return(fullPathSomeString).GetRight();

            resultPath.Should().BeOfType<AbsoluteFilePath>();

            var expectedFullPathString = $"{DirName}{Path.DirectorySeparatorChar}{FileName}";
            resultPath.FullPath.Value.Should().Be(expectedFullPathString);
        }

        [Fact]
        public void ReturnFilePath_FromSomeString_WithInvalidChars()
        {
            foreach (var testChar in Path.GetInvalidPathChars())
            {
                testReturnFilePath_InvalidChar_FromSomeString(testChar);
            }
        }


        [Fact]
        public void Name_Property()
        {
            SomeString pathSomeString = DirSomeString.Append(Path.DirectorySeparatorChar).Concat(FileSomeString);
            var path = AbsoluteDirectoryPath.Return(pathSomeString).GetRight();

            var actual = path.Name;

            actual.Should().Be(FileSomeString);

        }


        [Fact]
        public void ReturnFilePath_FromSomeStringWithDirSeparatorTail_Negative()
        {
            var fullPathSomeString = DirSomeString.Append(Path.DirectorySeparatorChar).Concat(FileSomeString).Append(Path.DirectorySeparatorChar);
            var result = AbsoluteFilePath.Return(fullPathSomeString);

            result.Should().BeOfType<Left<Exception, AbsoluteFilePath>>($"Creating an instance of {nameof(AbsoluteFilePath)} with a path ending with the directory separator MUST fail.");

            fullPathSomeString = DirSomeString.Append(Path.DirectorySeparatorChar).Concat(FileSomeString).Append(Path.AltDirectorySeparatorChar);
            result = AbsoluteFilePath.Return(fullPathSomeString);

            result.Should().BeOfType<Left<Exception, AbsoluteFilePath>>($"Creating an instance of {nameof(AbsoluteFilePath)} with a path ending with the alternative directory separator MUST fail.");
        }

        [Fact]
        public void ReturnFilePath_FromString_WithInvalidChars()
        {
            foreach (var testChar in Path.GetInvalidPathChars())
            {
                testReturnFilePath_InvalidChar_FromString(testChar);
            }
        }

        [Fact]
        public void ReturnFilePath_FromStringInstance()
        {
            var fullPathString = $"{DirName}{Path.DirectorySeparatorChar}{FileName}";
            var resultPath = AbsoluteFilePath.Return(fullPathString).GetRight();

            resultPath.Should().BeOfType<AbsoluteFilePath>();

            resultPath.FullPath.Value.Should().Be(fullPathString);
        }

        [Fact]
        public void ReturnFilePath_FromStringInstanceWithDirSeparatorTail_Negative()
        {
            var fullPathString = $"{DirName}{Path.DirectorySeparatorChar}{FileName}{Path.DirectorySeparatorChar}";
            var result = AbsoluteFilePath.Return(fullPathString);

            result.Should().BeOfType<Left<Exception, AbsoluteFilePath>>($"Creating an instance of {nameof(AbsoluteFilePath)} with a path ending with the directory separator MUST fail.");

            fullPathString = $"{DirName}{Path.DirectorySeparatorChar}{FileName}{Path.AltDirectorySeparatorChar}";
            result = AbsoluteFilePath.Return(fullPathString);

            result.Should().BeOfType<Left<Exception, AbsoluteFilePath>>($"Creating an instance of {nameof(AbsoluteFilePath)} with a path ending with the alternative directory separator MUST fail.");
        }

        private static void testReturnFilePath_InvalidChar_FromSomeString(char testChar)
        {
            var fullPathSomeString = DirSomeString.Append(Path.DirectorySeparatorChar).Concat($"prefix{testChar}suffix.txt");

            var result = AbsoluteFilePath.Return(fullPathSomeString);

            result.Should().BeOfType<Left<Exception, AbsoluteFilePath>>($"the string '{fullPathSomeString}' with invalid character {testChar} must not result in an instance of {nameof(AbsoluteFilePath)}.");
        }

        private static void testReturnFilePath_InvalidChar_FromString(char testChar)
        {
            var fullPathSomeString = $"{DirName}{Path.DirectorySeparatorChar}prefix{testChar}suffix.txt";

            var result = AbsoluteFilePath.Return(fullPathSomeString);

            result.Should().BeOfType<Left<Exception, AbsoluteFilePath>>($"the string '{fullPathSomeString}' with invalid character {testChar} must not result in an instance of {nameof(AbsoluteFilePath)}.");
        }
    }
}