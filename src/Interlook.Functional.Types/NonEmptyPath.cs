#region license

//MIT License

//Copyright(c) 2013-2020 Andreas Hübner

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

#endregion 
using Interlook.Monads;
using Interlook.Text;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using io = System.IO;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]

namespace Interlook.Functional.Types
{
    /// <summary>
    /// Base type of non-empty path references.
    /// </summary>
    public abstract class NonEmptyPath : AnyPath
    {
        private const string DoubleDot = "..";
        private static string SneakyBegin = DoubleDot + io.Path.DirectorySeparatorChar;
        private static string AltSneakyBegin = DoubleDot + io.Path.AltDirectorySeparatorChar;
        private static string SneakyEnd = io.Path.DirectorySeparatorChar + DoubleDot;
        private static string AltSneakyEnd = io.Path.AltDirectorySeparatorChar + DoubleDot;

        private static string[] SneakyMiddles = new string[]
            {
            string.Concat(io.Path.DirectorySeparatorChar, DoubleDot, io.Path.DirectorySeparatorChar),
            string.Concat(io.Path.AltDirectorySeparatorChar, DoubleDot, io.Path.DirectorySeparatorChar),
            string.Concat(io.Path.DirectorySeparatorChar, DoubleDot, io.Path.AltDirectorySeparatorChar),
            string.Concat(io.Path.AltDirectorySeparatorChar, DoubleDot, io.Path.AltDirectorySeparatorChar),
            };

        private static char[] DirectorySeparatorChars = new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.VolumeSeparatorChar };

        private static SomeString DirectorySeparatorCharAppendix = new SomeString(io.Path.DirectorySeparatorChar.ToString());

        // In Windows, character case is ignored in path-names
        private static Lazy<StringComparison> _directoryStringComparer
            = new Lazy<StringComparison>(() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);

        private Lazy<int> _hash;

        /// <summary>
        /// Returns the path as <see cref="NonEmptyString"/>
        /// </summary>
        protected abstract NonEmptyString GetStringValue();

        internal NonEmptyPath()
        {
            _hash = new Lazy<int>(() => GetStringValue().GetHashCode());
            _isSneakyPath = new Lazy<bool>(() => checkSneakyPath(GetStringValue()));
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="NonEmptyPath"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// The string representation of the path object.
        /// </returns>
        public static implicit operator string(NonEmptyPath path) => path?.GetStringValue() ?? string.Empty;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() => _hash.Value;

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() => GetStringValue();

        //internal static Either<Exception, NonEmptyString> GetTrimmedPathString(NonEmptyString filePath)
        //    => filePath.ToExceptionEither()
        //        .Bind(path => EndsInDirectorySeparator(path)
        //            ? NonEmptyString.Create(path.Value.Substring(0, path.Length - 1))
        //            : filePath.ToExceptionEither());

        //internal SomeString CombinePathName(SomeString pathSuffix)
        //    => PathInternal.Concat($"{io.Path.DirectorySeparatorChar}{pathSuffix}");

        //internal SomeString CombinePathName(string pathSuffix)
        //    => PathInternal.Concat($"{io.Path.DirectorySeparatorChar}{pathSuffix}");

        //internal SomeString CombinePathName(char pathSuffix)
        //    => PathInternal.Concat($"{io.Path.DirectorySeparatorChar}{pathSuffix}");

        internal static Maybe<FileName> GetFileName(NonEmptyString path)
        {
            return path.SplitAtLastOccurenceOfAny(DirectorySeparatorChars)
                .MapEither(whole => new FileName(whole).ToMaybe(),
                    tuple => tuple.Right is NonEmptyString nonEmpty
                        ? new FileName(nonEmpty).ToMaybe()
                        : Nothing<FileName>.Instance); // path ends with separator => no file name
        }

        internal static Either<Exception, NonEmptyString> GetDirectoryPath(NonEmptyString pathString)
        {
            try
            {
                var r = System.IO.Path.GetDirectoryName(pathString.Value);
                if (r is string s && s.Length > 0 && AnyString.Create(s) is NonEmptyString noneEmpty)
                {
                    return noneEmpty.ToExceptionEither();
                }
                else if (r == null)
                {
                    return pathString.ToExceptionEither();    // <null> means pathString was a root
                }
            }
            catch
            { }

            return Either.Left<Exception, NonEmptyString>(new ArgumentException("Path did not contain valid directory."));
        }

        private Lazy<bool> _isSneakyPath;

        /// <summary>
        /// Gets a value indicating whether this instance is a sneaky path,
        /// thus containing directory traversal with '..'
        /// </summary>
        public bool IsSneakyPath => _isSneakyPath.Value;

        private bool checkSneakyPath(NonEmptyString path)
            => path.StartsWith(SneakyBegin)
                || path.StartsWith(AltSneakyBegin)
                || path.EndsWith(SneakyEnd)
                || path.EndsWith(AltSneakyEnd)
                || path.Equals(DoubleDot)
                || SneakyMiddles.Any(m => path.Contains(m));

        private static bool isDirectorySeparatorChar(char c) => c == io.Path.DirectorySeparatorChar || c == io.Path.AltDirectorySeparatorChar;

        internal class NonEmptyPathString
        {
            public NonEmptyString Path { get; }

            public int RootLength { get; }

            public NonEmptyString TrimmedPath;

            private NonEmptyPathString(NonEmptyString path, NonEmptyString trimmedPath, int rootLength)
            {
                Path = path;
                TrimmedPath = trimmedPath;
                RootLength = rootLength;
            }

            internal static Either<Exception, NonEmptyPathString> Create(string path)
                => NonEmptyString.Create(path)
                    .AddOuterException(ex => new Exception("Path must not be empty and consist of actual characters, not just whitespaces", ex))
                    .Bind(Create);

            internal static Either<Exception, NonEmptyPathString> Create(NonEmptyString path)
                => path.ToExceptionEither()
                    .Bind(origPath => origPath.Value.IndexOfAny(io.Path.GetInvalidPathChars()).ToExceptionEither()
                        .FailIf<Exception, int>(pos => pos >= 0, pos => new ArgumentException($"Path contains invalid character '{origPath.Value[pos]}' at position {pos}.", nameof(path)))
                        .Select(_ => origPath))
                    .FailIf(p =>
                        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && pathEndsWithPeriodOrSpace(p),
                        p => new ArgumentException("In Windows paths must not end with '.'", nameof(path)))
                    .Select(p => (Path: p, RootLen: (io.Path.GetPathRoot(p.Value) ?? string.Empty).Length))
                    .Select(p => (p.Path, p.RootLen, Trimmed: getTrimmedPathString(p.Path, p.RootLen)))
                    .Select(p => new NonEmptyPathString(p.Path, p.Trimmed, p.RootLen));


            private static bool pathEndsWithPeriodOrSpace(NonEmptyString trimmedPathString)
                => Try.Invoke(() => io.Path.GetFileName(trimmedPathString.Value))
                    .Then(p => p.IsNeitherNullNorEmpty() && p.Length > 1 && p[p.Length - 1] == '.')
                    .MapTry(succ => succ, error => true);

            internal NonEmptyPathString Combine(NonEmptyString path)
            {
                var newPath = EndsInDirectorySeparator()
                        ? Path.Concat(path)
                        : Path.Concat(DirectorySeparatorCharAppendix).Concat(path);
                return new NonEmptyPathString(newPath, getTrimmedPathString(newPath, RootLength), RootLength);
            }

            internal bool EndsInDirectorySeparator() => isDirectorySeparatorChar(Path.LastChar);

            internal SomeString GetPathWithEnsuredDirectorySeparatorTail()
                => DirectorySeparatorCharAppendix.Prepend(TrimmedPath);

            private static NonEmptyString getTrimmedPathString(NonEmptyString s, int rootLen)
            {
                string trimmed = s.Value.TrimEnd(DirectorySeparatorChars);
                return trimmed.Length < rootLen ? s : NonEmptyString.Create(trimmed).GetRight();
            }
        }
    }
}