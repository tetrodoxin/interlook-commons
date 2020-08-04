using Interlook.Monads;
using Interlook.Text;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using io = System.IO;

[assembly: InternalsVisibleTo("Interlook.Functional.Types.UnitTests")]
namespace Interlook.Functional.Types
{

    /// <summary>
    /// A type representing a valid file name,
    /// thus being non-empty and not containing
    /// characters, that are not valid for file names (see <see cref="io.Path.GetInvalidFileNameChars"/>)
    /// </summary>
    public class FileName
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        public SomeString Name { get; }

        private FileName(SomeString name) => Name = name;

        /// <summary>
        /// Tries to create an instance of <see cref="FileName"/>
        /// </summary>
        /// <param name="name">The name of the file. Must not be empty and not conatin any characters returned by <see cref="io.Path.GetInvalidFileNameChars"/></param>
        /// <returns>An instance of <see cref="Right{Exception, FileName}"/> containing
        /// a new instance of <see cref="FileName"/> if the path provided was a valid file name;
        /// otherwise <see cref="Left{Exception, FileName}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, FileName> Create(string name)
            => name.ToExceptionEither()
                .FailIf(_ => name == null, new ArgumentNullException(nameof(name)))
                .Bind(_ => StringBase.CreateSome(name))
                .Bind(Create);

        /// <summary>
        /// Tries to create an instance of <see cref="FileName"/>
        /// </summary>
        /// <param name="name">The name of the file. Must not be empty and not conatin any characters returned by <see cref="io.Path.GetInvalidFileNameChars"/></param>
        /// <returns>An instance of <see cref="Right{Exception, FileName}"/> containing
        /// a new instance of <see cref="FileName"/> if the path provided was a valid file name;
        /// otherwise <see cref="Left{Exception, FileName}"/> containing the respective error as <see cref="Exception"/>.</returns>
        public static Either<Exception, FileName> Create(SomeString name)
            => name.ToExceptionEither()
                .FailIf(_ => name == null, new ArgumentNullException(nameof(name)))
                .Bind(fileName => fileName.Value.IndexOfAny(io.Path.GetInvalidFileNameChars()).ToExceptionEither()
                    .FailIf(illegalPos => illegalPos >= 0, illegalPos => new FormatException($"Filename contains illegal character at position {illegalPos}"))
                    .Select(_ => new FileName(fileName)));
    }
}