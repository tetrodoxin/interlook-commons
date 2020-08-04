using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]
namespace Interlook.Functional.Types
{
    /// <summary>
    /// A non-empty reference of a relative path of a file, containing no path traversal with "..",
    /// having no rooted directory and never ending with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonSneakyRelativePath" />
    public sealed class NonSneakyRelativeFilePath : NonSneakyRelativePath
    {
        internal NonSneakyRelativeFilePath(SomeString trimmedPath) : base(trimmedPath, false)
        {
        }
    }
}