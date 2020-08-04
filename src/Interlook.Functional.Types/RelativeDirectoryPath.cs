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
        internal RelativeDirectoryPath(SomeString trimmedPath) : base(trimmedPath, true)
        {
        }
    }
}