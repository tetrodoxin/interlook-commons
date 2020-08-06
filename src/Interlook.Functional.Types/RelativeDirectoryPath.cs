using Interlook.Monads;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]

namespace Interlook.Functional.Types
{
    /// <summary>
    /// A non-empty reference of a relative path of a directory, thus having no rooted directory
    /// and always ends with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    public sealed class RelativeDirectoryPath : RelativePath
    {
        /// <summary>
        /// The name of the directory (leaf of the path, without any separator)
        /// </summary>
        public SomeString Name { get; }

        internal RelativeDirectoryPath(SomeString trimmedPath) : base(trimmedPath, true)
        {
            Name = Try.InvokeToExceptionEither(() => System.IO.Path.GetFileName(trimmedPath))
                .Bind(dirname => StringBase.CreateSome(dirname))
                .MapEither(_ => trimmedPath, dirname => dirname);
        }
    }
}