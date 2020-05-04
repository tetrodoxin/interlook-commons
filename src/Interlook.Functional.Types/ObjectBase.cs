namespace Interlook.Functional.Types
{
    /// <summary>
    /// Base class for functional objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectBase<T>
    {
        private int _hash;

        /// <summary>
        /// Gets the value of the object.
        /// </summary>
        public T Value { get; }

        internal ObjectBase(T value)
        {
            Value = value;
            _hash = value?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
            => Value?.ToString() ?? typeof(T).Name;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => _hash;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
            => other is ObjectBase<T> o
            ? GetType().IsAssignableFrom(o.GetType()) && o._hash.Equals(_hash) && object.Equals(o.Value, Value)
            : false;

        /// <summary>
        /// Performs an implicit conversion from <see cref="ObjectBase{T}"/> to <c>T</c>
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
#pragma warning disable CS8603 // Possible null reference return.
        public static implicit operator T(ObjectBase<T> o) => o != null ? o.Value ?? default : default;
#pragma warning restore CS8603 // Possible null reference return.
    }
}