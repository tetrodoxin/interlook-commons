using FluentAssertions;
using Interlook.Monads;
using System;
using System.IO;
using Xunit;

namespace Interlook.Functional.Types.UnitTests
{
    public class AbsoluteDirectoryPathTests
    {
        private const string DirName = @"c:\correct\dir\path";
        private const string RelativeDirectoryPath = @"yet\another\subdir\";
        private const string RelativeFilePath = @"another\subdir\file.xml";
        private static FileName CorrectFileName = FileName.Create("Correct.pdf").GetRight();
        private static SomeString DirNameSomeString = StringBase.CreateSome(DirName).GetRight();
        private static SomeString DirNameWithTailSomeString = DirNameSomeString.Concat(Path.DirectorySeparatorChar);

        [Fact]
        public void Combine_FileName()
        {
            var path = AbsolutePath.ReturnDirectoryPath(DirNameWithTailSomeString).GetRight();
            var filepath = path.Combine(CorrectFileName).GetRight();

            filepath.Should().BeOfType<AbsoluteFilePath>();

            var expectedFilePath = DirNameWithTailSomeString.Concat(CorrectFileName.Name);
            filepath.Path.Should().Be(expectedFilePath);
        }

        [Fact]
        public void Combine_NonSneakyRelativeDirectoryPath()
        {
            var path = AbsolutePath.ReturnDirectoryPath(DirNameWithTailSomeString).GetRight();
            var relativeDir = NonSneakyRelativePath.ReturnDirectoryPath(RelativeDirectoryPath).GetRight();
            var resultPath = path.Combine(relativeDir);

            resultPath.Should().BeOfType<AbsoluteDirectoryPath>();

            var expectedFilePath = DirNameWithTailSomeString.Concat(RelativeDirectoryPath);
            resultPath.Path.Should().Be(expectedFilePath);
        }

        [Fact]
        public void Combine_NonSneakyRelativeFilePath()
        {
            var path = AbsolutePath.ReturnDirectoryPath(DirNameWithTailSomeString).GetRight();
            var relativeFile = NonSneakyRelativePath.ReturnFilePath(RelativeFilePath).GetRight();
            var resultPath = path.Combine(relativeFile).GetRight();

            resultPath.Should().BeOfType<AbsoluteFilePath>();

            var expectedFilePath = DirNameWithTailSomeString.Concat(RelativeFilePath);
            resultPath.Path.Should().Be(expectedFilePath);
        }

        [Fact]
        public void Combine_RelativePath_Dir()
        {
            var absolutePath = AbsolutePath.ReturnDirectoryPath(DirNameWithTailSomeString).GetRight();
            var relativeDir = (RelativePath)RelativePath.ReturnDirectoryPath(RelativeDirectoryPath).GetRight();
            var resultPath = absolutePath.Combine(relativeDir).GetRight();

            resultPath.Should().BeOfType<AbsoluteDirectoryPath>();

            var expectedFilePath = DirNameWithTailSomeString.Concat(RelativeDirectoryPath);
            resultPath.Path.Should().Be(expectedFilePath);
        }

        [Fact]
        public void Combine_RelativePath_File()
        {
            var absolutePath = AbsolutePath.ReturnDirectoryPath(DirNameWithTailSomeString).GetRight();
            var relativeFile = (RelativePath)RelativePath.ReturnFilePath(RelativeFilePath).GetRight();
            var resultPath = absolutePath.Combine(relativeFile).GetRight();

            resultPath.Should().BeOfType<AbsoluteFilePath>();

            var expectedFilePath = DirNameWithTailSomeString.Concat(RelativeFilePath);
            resultPath.Path.Should().Be(expectedFilePath);
        }

        [Fact]
        public void GetParentPath()
        {
            var subDirName = @"deeperDir";
            var dirPathName = DirNameWithTailSomeString.Concat(subDirName);
            var absoluteDirPath = AbsolutePath.ReturnDirectoryPath(dirPathName).GetRight();

            var resultBasePath = absoluteDirPath.GetParentPath().GetRight();

            resultBasePath.Should().BeOfType<AbsoluteDirectoryPath>();
            resultBasePath.Path.Should().Be(DirNameWithTailSomeString);
        }

        [Fact]
        public void GetParentPath_OfRootDir()
        {
            var absoluteDirPath = AbsolutePath.ReturnDirectoryPath(@"C:\").GetRight();

            var resultEither = absoluteDirPath.GetParentPath();

            resultEither.Should().BeOfType<Left<Exception, AbsoluteDirectoryPath>>();
        }

        [Fact]
        public void ReturnDirectoryPath_FromSomeString_WithInvalidChars()
        {
            foreach (var testChar in Path.GetInvalidPathChars())
            {
                testReturnDirectoryPath_InvalidChar_FromSomeString(testChar);
            }
        }

        [Fact]
        public void ReturnDirectoryPath_FromSomeString_WithoutSeparatorTail()
        {
            var path = AbsolutePath.ReturnDirectoryPath(DirNameSomeString).GetRight();

            var nameTailed = DirNameSomeString.Concat(Path.DirectorySeparatorChar);

            path.Should().BeOfType<AbsoluteDirectoryPath>();
            path.Path.Should().Be(nameTailed);
        }

        [Fact]
        public void ReturnDirectoryPath_FromSomeString_WithSeparatorTail()
        {
            testReturnDirectoryPath_SomeString_WithSeparator(Path.DirectorySeparatorChar);
            testReturnDirectoryPath_SomeString_WithSeparator(Path.AltDirectorySeparatorChar);
        }

        [Fact]
        public void ReturnDirectoryPath_FromString_WithInvalidChars()
        {
            foreach (var testChar in Path.GetInvalidPathChars())
            {
                testReturnDirectoryPath_InvalidChar_FromString(testChar);
            }
        }

        [Fact]
        public void ReturnDirectoryPath_FromStringInstance_WithoutSeparatorTail()
        {
            var fullPath = DirName;
            var path = AbsolutePath.ReturnDirectoryPath(fullPath).GetRight();

            var nameTailed = $"{fullPath}{Path.DirectorySeparatorChar}";

            path.Should().BeOfType<AbsoluteDirectoryPath>();
            path.Path.Value.Should().Be(nameTailed);
        }

        [Fact]
        public void ReturnDirectoryPath_FromStringInstance_WithSeparatorTail()
        {
            testReturnDirectoryPath_StringInstance_WithSeparator(Path.DirectorySeparatorChar);
            testReturnDirectoryPath_StringInstance_WithSeparator(Path.AltDirectorySeparatorChar);
        }

        private static void testReturnDirectoryPath_InvalidChar_FromSomeString(char testChar)
        {
            var fullPathSomeString = DirNameSomeString.Concat(Path.DirectorySeparatorChar).Concat($"prefix{testChar}suffix{Path.DirectorySeparatorChar}");

            var result = AbsolutePath.ReturnDirectoryPath(fullPathSomeString);

            result.Should().BeOfType<Left<Exception, AbsoluteDirectoryPath>>($"the string '{fullPathSomeString}' with invalid character {testChar} must not result in an instance of {nameof(AbsoluteDirectoryPath)}.");
        }

        private static void testReturnDirectoryPath_InvalidChar_FromString(char testChar)
        {
            var fullPathSomeString = $"{DirName}{Path.DirectorySeparatorChar}prefix{testChar}suffix{Path.DirectorySeparatorChar}";

            var result = AbsolutePath.ReturnDirectoryPath(fullPathSomeString);

            result.Should().BeOfType<Left<Exception, AbsoluteDirectoryPath>>($"the string '{fullPathSomeString}' with invalid character {testChar} must not result in an instance of {nameof(AbsoluteDirectoryPath)}.");
        }

        private static void testReturnDirectoryPath_SomeString_WithSeparator(char separator)
        {
            var fullPathSomeString = DirNameSomeString.Concat(separator);
            var path = AbsolutePath.ReturnDirectoryPath(fullPathSomeString).GetRight();

            path.Should().BeOfType<AbsoluteDirectoryPath>();
            var expectedPathSomeString = DirNameSomeString.Concat(Path.DirectorySeparatorChar);
            path.Path.Should().Be(expectedPathSomeString);
        }

        private static void testReturnDirectoryPath_StringInstance_WithSeparator(char separator)
        {
            var fullPath = $"{DirName}{separator}";
            var path = AbsolutePath.ReturnDirectoryPath(fullPath).GetRight();

            path.Should().BeOfType<AbsoluteDirectoryPath>();
            var expectedFullPath = $"{DirName}{Path.DirectorySeparatorChar}";
            path.Path.Value.Should().Be(expectedFullPath);
        }
    }
}