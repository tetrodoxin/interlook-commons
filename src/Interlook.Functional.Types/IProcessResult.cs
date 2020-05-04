using System.Collections.Generic;

namespace Interlook.Functional.Types
{
    /// <summary>
    /// Interface for results of <see cref="ProcessHelper"/>
    /// </summary>
    public interface IProcessResult
    {
        /// <summary>
        /// Returns a sequence of error strings.
        /// </summary>
        IReadOnlyCollection<string> Errors { get; }

        /// <summary>
        /// Returns a sequence of standard-output string.
        /// </summary>
        IReadOnlyCollection<string> Output { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IProcessResult"/> represents
        /// a successful process execution.
        /// </summary>
        bool Success { get; }
    }
}