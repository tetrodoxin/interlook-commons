using Interlook.Monads;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Interlook.Functional.Types
{
    /// <summary>
    /// Contains results of an execution of an external process
    /// via <see cref="ProcessHelper"/>.
    /// </summary>
    /// <seealso cref="IProcessResult" />
    public sealed class ProcessResult : IProcessResult
    {
        /// <summary>
        /// Returns a sequence of error strings.
        /// </summary>
        public IReadOnlyCollection<string> Errors { get; }

        /// <summary>
        /// Returns a sequence of standard-output string.
        /// </summary>
        public IReadOnlyCollection<string> Output { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IProcessResult" /> represents
        /// a successful process execution.
        /// <para>
        /// Depends on a value provided to the <see cref="Create(bool, List{string}, List{string})"/>
        /// as well as <see cref="Errors"/> being an empty sequence.
        /// </para>
        /// </summary>
        public bool Success { get; }

        private ProcessResult(bool success, List<string> errors, List<string> output)
        {
            Errors = errors;
            Output = output;

            Success = success && !Errors.Any();
        }

        /// <summary>
        /// Tries to create a new instance of <see cref="ProcessResult"/>.
        /// </summary>
        /// <param name="success">Determines, if the execution was successful.</param>
        /// <param name="errors">The error string.</param>
        /// <param name="output">The standard-output string..</param>
        /// <returns></returns>
        public static Either<Exception, IProcessResult> Create(bool success, List<string> errors, List<string> output)
            => ((IProcessResult)new ProcessResult(success, errors, output)).ToExceptionEither();
    }
}