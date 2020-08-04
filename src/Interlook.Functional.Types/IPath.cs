using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]
namespace Interlook.Functional.Types
{
    /// <summary>
    /// Interface for path-reference-string objects.
    /// </summary>
    public interface IPath
    {
        /// <summary>Gets the path as <see cref="string"/></summary>
        string Path { get; }
    }
}