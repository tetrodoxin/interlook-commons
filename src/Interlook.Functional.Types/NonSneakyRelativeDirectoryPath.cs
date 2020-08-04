using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]
namespace Interlook.Functional.Types
{
    /// <summary>
    /// A non-empty reference of a relative path of a directory, containing no path traversal with "..",
    /// having no rooted directory and always ending with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonSneakyRelativePath" />
    public sealed class NonSneakyRelativeDirectoryPath : NonSneakyRelativePath
    {
        internal NonSneakyRelativeDirectoryPath(SomeString trimmedPath) : base(trimmedPath, true)
        {
        }
    }
}