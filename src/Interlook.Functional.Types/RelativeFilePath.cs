using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]
namespace Interlook.Functional.Types
{
    /// <summary>
    /// A non-empty reference of a relative path of a file, thus having no rooted directory
    /// and does not end with <see cref="System.IO.Path.DirectorySeparatorChar"/>
    /// </summary>
    /// <seealso cref="NonEmptyPath" />
    public sealed class RelativeFilePath : RelativePath
    {
        internal RelativeFilePath(SomeString trimmedPath) : base(trimmedPath, false)
        {
        }
    }
}