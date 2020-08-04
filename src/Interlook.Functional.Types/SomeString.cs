namespace Interlook.Functional.Types
{
    /// <summary>
    /// A functional string object containing actual characters,
    /// rather than whitespace-characters only.
    /// </summary>
    public sealed class SomeString : NonEmptyString
    {
        internal SomeString(string value) : base(value)
        {
        }
    }
}