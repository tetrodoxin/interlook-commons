using Interlook.Monads;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using io = System.IO;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]
namespace Interlook.Functional.Types
{
    /// <summary>
    /// A non-empty path reference, where path traversal with ".." is forbidden.
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    public class NonSneakyPath : NonEmptyPath
    {
        private const string DoubleDot = "..";
        private static string SneakyBegin = DoubleDot + io.Path.DirectorySeparatorChar;
        private static string SneakyEnd = io.Path.DirectorySeparatorChar + DoubleDot;
        private static string SneakyMiddle = string.Concat(io.Path.DirectorySeparatorChar, DoubleDot, io.Path.DirectorySeparatorChar);

        internal NonSneakyPath(SomeString trimmedPath, bool isDirectory) : base(trimmedPath, isDirectory)
        {
        }

        internal static Either<Exception, NonSneakyPath> CreateNonSneaky(NonEmptyPath path)
                    => path.ToExceptionEither()
            .FailIf(p => isSneakyPath(p), p => new ArgumentException($"'{p}' is a sneaky path (contains '..'-notation).", nameof(path)))
            .Select(p => new NonSneakyPath(p.TrimmedPathInternal, p.IsDirectory));
       
        private static bool isSneakyPath(NonEmptyPath path)
            => path.Path.StartsWith(SneakyBegin)
                || path.Path.EndsWith(SneakyEnd)
                || path.Path.Contains(SneakyMiddle)
                || path.Path.Equals(DoubleDot);
    }
}