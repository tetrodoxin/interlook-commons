using FluentAssertions;
using Interlook.Monads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace Interlook.Functional.Types.UnitTests
{
    public class AbsoluteDirectoryPathTests
    {
        private static string[] RootedDirPathNaked = makeRootedPath("correct", "dir", "path");

        private static string TestRelativeDirectoryPathTailed = makePath("yet", "another", "subdir") + Path.DirectorySeparatorChar;
        private static string TestRelativeFilePath = makePath("another", "subdir", "file.xml");

        private static FileName CorrectFileName = FileName.Create("Correct.pdf").GetRight();

        [Theory]
        [MemberData(nameof(GetTailedRootedDir))]
        public void Combine_FileName(SomeString tailedRootedDir)
        {
            var path = AbsoluteDirectoryPath.Return(tailedRootedDir).GetRight();
            var filepath = path.Combine(CorrectFileName).GetRight();

            filepath.Should().BeOfType<AbsoluteFilePath>();

            var expectedFilePath = tailedRootedDir.Concat(CorrectFileName.Name);
            filepath.FullPath.Should().Be(expectedFilePath);
        }

        [Theory]
        [MemberData(nameof(GetTailedRootedDir))]
        public void Combine_RelativeDirectoryPath(SomeString tailedRootedDir)
        {
            var path = AbsoluteDirectoryPath.Return(tailedRootedDir).GetRight();
            var relativeDir = RelativeDirectoryPath.Return(TestRelativeDirectoryPathTailed).GetRight();
            var resultPath = path.Combine(relativeDir);

            resultPath.Should().BeOfType<AbsoluteDirectoryPath>();

            var expectedFilePath = tailedRootedDir.Concat(TestRelativeDirectoryPathTailed);
            resultPath.Path.Should().Be(expectedFilePath);
        }

        [Theory]
        [MemberData(nameof(GetTailedRootedDir))]
        public void Combine_RelativeFilePath(SomeString tailedRootedDir)
        {
            var path = AbsoluteDirectoryPath.Return(tailedRootedDir).GetRight();
            var relativeFile = RelativeFilePath.Return(TestRelativeFilePath).GetRight();
            var resultPath = path.Combine(relativeFile).GetRight();

            resultPath.Should().BeOfType<AbsoluteFilePath>();

            var expectedFilePath = tailedRootedDir.Concat(TestRelativeFilePath);
            resultPath.FullPath.Should().Be(expectedFilePath);
        }

        [Theory]
        [MemberData(nameof(GetTailedRootedDir))]
        public void GetParentPath(SomeString rootedDirTailedString)
        {
            var subDirName = @"deeperDir";
            var dirPathName = rootedDirTailedString.Concat(subDirName);
            var absoluteDirPath = AbsoluteDirectoryPath.Return(dirPathName).GetRight();

            var resultBasePath = absoluteDirPath.GetParentPath().GetRight();

            resultBasePath.Should().BeOfType<AbsoluteDirectoryPath>();
            resultBasePath.Path.Should().Be(rootedDirTailedString);
        }

        [Theory]
        [MemberData(nameof(GetRootStringsOnly))]
        public void Property_IsRoot_ForComplete(string root)
        {
            Either<Exception, AbsoluteDirectoryPath> rt = AbsoluteDirectoryPath.Return(root);
            var absoluteDirPath = rt.GetRight();

            absoluteDirPath.IsRoot.Should().BeTrue($"'root' is considered a complete root without any further directory name.");
        }

        [Theory]
        [MemberData(nameof(GetRootStringsOnly))]
        public void GetParentPath_OfRootDir(string root)
        {
            var absoluteDirPath = ((Either<Exception, AbsoluteDirectoryPath>)AbsoluteDirectoryPath.Return(root)).GetRight();

            var resultEither = absoluteDirPath.GetParentPath();

            resultEither.Should().BeOfType<Left<Exception, AbsoluteDirectoryPath>>("a root does not have a parent directory");
        }

        [Theory]
        [MemberData(nameof(GetNakedRootedDir))]
        public void ReturnDirectoryPath_SomeString_WithInvalidChars(SomeString nakedRootedDir)
        {
            foreach (var testChar in Path.GetInvalidPathChars())
            {
                var fullPathSomeString = nakedRootedDir.Append(Path.DirectorySeparatorChar).Concat($"prefix{testChar}suffix{Path.DirectorySeparatorChar}");
                var result = AbsoluteDirectoryPath.Return(fullPathSomeString);
                result.Should().BeOfType<Left<Exception, AbsoluteDirectoryPath>>($"the string '{fullPathSomeString}' with invalid character {testChar} must not result in an instance of {nameof(AbsoluteDirectoryPath)}.");
            }
        }

        [Theory]
        [MemberData(nameof(GetNakedRootedDir))]
        public void ReturnDirectoryPath_SomeString_WithoutSeparatorTail(SomeString nakedRootedDir)
        {
            var path = AbsoluteDirectoryPath.Return(nakedRootedDir).GetRight();

            var nameTailed = nakedRootedDir.Append(Path.DirectorySeparatorChar);

            path.Should().BeOfType<AbsoluteDirectoryPath>();
            path.Path.Should().Be(nameTailed);
        }

        [Theory]
        [MemberData(nameof(GetNakedRootedDir))]
        public void ReturnDirectoryPath_SomeString_WithSeparatorTail(SomeString nakedRootedDir)
        {
            testReturnDirectoryPath_SomeString_WithSeparator(nakedRootedDir, Path.DirectorySeparatorChar);
            testReturnDirectoryPath_SomeString_WithSeparator(nakedRootedDir, Path.AltDirectorySeparatorChar);
        }

        [Theory]
        [MemberData(nameof(GetNakedRootedDir))]
        public void ReturnDirectoryPath_String_WithInvalidChars(SomeString nakedRootedDir)
        {
            foreach (var testChar in Path.GetInvalidPathChars())
            {
                var fullPathSomeString = $"{nakedRootedDir}{Path.DirectorySeparatorChar}prefix{testChar}suffix{Path.DirectorySeparatorChar}";

                var result = AbsoluteDirectoryPath.Return(fullPathSomeString);

                result.Should().BeOfType<Left<Exception, AbsoluteDirectoryPath>>($"the string '{fullPathSomeString}' with invalid character {testChar} must not result in an instance of {nameof(AbsoluteDirectoryPath)}.");
            }
        }

        [Theory]
        [MemberData(nameof(GetNakedRootedDir))]
        public void ReturnDirectoryPath_String_WithoutSeparatorTail(SomeString nakedRootedDir)
        {
            string fullPath = nakedRootedDir.Value;
            var path = AbsoluteDirectoryPath.Return(fullPath).GetRight();

            var nameTailed = $"{fullPath}{Path.DirectorySeparatorChar}";

            path.Should().BeOfType<AbsoluteDirectoryPath>();
            path.Path.Value.Should().Be(nameTailed);
        }

        [Theory]
        [MemberData(nameof(GetNakedRootedDir))]
        public void ReturnDirectoryPath_FromStringInstance_WithSeparatorTail(SomeString nakedRootedDir)
        {
            testReturnDirectoryPath_StringInstance_WithSeparator(nakedRootedDir, Path.DirectorySeparatorChar);
            testReturnDirectoryPath_StringInstance_WithSeparator(nakedRootedDir, Path.AltDirectorySeparatorChar);
        }

        [Theory]
        [MemberData(nameof(GetNakedRootedDir))]
        public void Name_Property(SomeString nakedRootedDir)
        {
            string dirName = "LeafDir";
            SomeString baseDirSomeString = nakedRootedDir.Append(Path.DirectorySeparatorChar).Concat(dirName);
            var path = AbsoluteDirectoryPath.Return(baseDirSomeString).GetRight();

            var actual = path.Name;

            actual.Value.Should().Be(dirName);
        }

        private static void testReturnDirectoryPath_InvalidChar_FromSomeString(NonEmptyString fullPathSomeString)
        {

        }

        private static void testReturnDirectoryPath_InvalidChar_FromString(char testChar)
        {
        }

        private static void testReturnDirectoryPath_SomeString_WithSeparator(SomeString nakedRootedDir, char separator)
        {
            var fullPathSomeString = nakedRootedDir.Append(separator);
            var path = AbsoluteDirectoryPath.Return(fullPathSomeString).GetRight();

            path.Should().BeOfType<AbsoluteDirectoryPath>();
            var expectedPathSomeString = nakedRootedDir.Append(Path.DirectorySeparatorChar);
            path.Path.Should().Be(expectedPathSomeString);
        }

        private static void testReturnDirectoryPath_StringInstance_WithSeparator(string nakedRootedDir, char separator)
        {
            var fullPath = $"{nakedRootedDir}{separator}";
            var path = AbsoluteDirectoryPath.Return(fullPath).GetRight();

            path.Should().BeOfType<AbsoluteDirectoryPath>();
            var expectedFullPath = $"{nakedRootedDir}{Path.DirectorySeparatorChar}";
            path.Path.Value.Should().Be(expectedFullPath);
        }

        private static string makePath(params string[] parts)
        {
            return string.Join(Path.DirectorySeparatorChar, parts);
        }

        private static string[] makeRootedPath(params string[] parts)
        {
            var path = makePath(parts);
            var qs = new List<string> { $"{Path.DirectorySeparatorChar}{path}" };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // add DOS notation
                qs.Add($"C{Path.VolumeSeparatorChar}{Path.DirectorySeparatorChar}{path}{path}");
            }

            // leave out UNC or Device notation here

            return qs.ToArray();
        }

        public static IEnumerable<object[]> GetTailedRootedDir() => RootedDirPathNaked.Select(p => new object[] { new SomeString(p + Path.DirectorySeparatorChar) });

        public static IEnumerable<object[]> GetNakedRootedDir() => RootedDirPathNaked.Select(p => new object[] { new SomeString(p) });

        public static IEnumerable<object[]> GetRootStringsOnly()
        {
            yield return new object[] { Path.DirectorySeparatorChar.ToString() };
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                yield return new object[] { $"C{Path.VolumeSeparatorChar}{Path.DirectorySeparatorChar}" };
            }
        }
    }
}