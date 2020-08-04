namespace Interlook.Functional.Types
{
    /// <summary>
    /// A functional string object that is non-empty,
    /// thus having a length > 0.
    /// </summary>
    public class NonEmptyString : StringBase
    {
        /// <summary>
        /// Returns the character at the specified position.
        /// </summary>
        /// <param name="charPosition">The position in the string.
        /// A negative value is subtracted from the strings length, thus indexing backwards.</param>
        /// <returns></returns>
        public char this[int charPosition] => charPosition >= 0 ? Value[charPosition] : Value[Value.Length - charPosition];

        internal NonEmptyString(string value) : base(value)
        {
        }
    }
}