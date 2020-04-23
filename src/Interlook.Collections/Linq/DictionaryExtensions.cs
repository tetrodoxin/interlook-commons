#region license

//MIT License

//Copyright(c) 2013-2020 Andreas Hübner

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

#endregion 
#if NET451PLUS

using System;

namespace System.Collections.Generic
{
    /// <summary>
    /// Helper methods for generic dictionaries
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets the value associated with the specified key of a dictionary 
        /// or a default value, if the dictionary does not contain the key.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys of the dictionary.</typeparam>
        /// <typeparam name="TValue">Type of the values of the dictionary.</typeparam>
        /// <param name="dictionary">A dictionary.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="defaultValue">The default value to be returned, of the key was not found.</param>
        /// <returns>
        /// The value associated with the specified key, if that key was found in the dictionary;
        /// in all other cases <paramref name="defaultValue"/>.
        /// </returns>
        /// <remarks>
        /// This method does not throw <see cref="NullReferenceException"/>, it returns
        /// the value of <paramref name="defaultValue"/> instead.
        /// </remarks>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary == null || (isKeyNullReference<TKey>(key)))
            {
                return defaultValue;
            }

            TValue result;
            if (dictionary.TryGetValue(key, out result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key of a dictionary 
        /// or a default value, if the dictionary does not contain the key.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys of the dictionary.</typeparam>
        /// <typeparam name="TValue">Type of the values of the dictionary.</typeparam>
        /// <param name="dictionary">A dictionary.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>
        /// The value associated with the specified key, if that key was found in the dictionary;
        /// in all other cases the default value of the type <typeparamref name="TValue"/>.
        /// </returns>
        /// <remarks>
        /// This method does not throw <see cref="NullReferenceException"/>, it returns
        /// the default value of <typeparamref name="TValue"/> instead.
        /// </remarks>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return GetValueOrDefault(dictionary, key, default);
        }

        /// <summary>
        /// Adds a value to a dictionary, if a specified condition is met.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys of the dictionary.</typeparam>
        /// <typeparam name="TValue">Type of the values of the dictionary.</typeparam>
        /// <param name="dictionary">A dictionary.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">The new value to add.</param>
        /// <param name="condition">The condition, which specifies, if the value shall actually be added.</param>
        /// <returns>The original dictionary or a new, possibly empty, one (Fluent API). Never <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AddIf<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool condition, TKey key, TValue value)
        {
            if (dictionary != null && condition)
            {
                dictionary.Add(key, value);
            }

            return dictionary ?? new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Adds a value to a dictionary, if a specified condition is met.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys of the dictionary.</typeparam>
        /// <typeparam name="TValue">Type of the values of the dictionary.</typeparam>
        /// <param name="dictionary">A dictionary.</param>
        /// <param name="newValueFactory">Factory method for the new <see cref="KeyValuePair{TKey, TValue}"/> instance. Executed not before <paramref name="condition"/> has been successfully checked.</param>
        /// <param name="condition">The condition, which specifies, if the value shall actually be added.</param>
        /// <returns>The original dictionary or a new, possibly empty, one (Fluent API). Never <c>null</c>.</returns>
        public static IDictionary<TKey, TValue> AddIf<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool condition, Func<KeyValuePair<TKey, TValue>> newValueFactory)
        {
            if (newValueFactory == null)
                throw new ArgumentNullException(nameof(newValueFactory));

            if (dictionary != null && condition)
            {
                dictionary.Add(newValueFactory());
            }

            return dictionary ?? new Dictionary<TKey, TValue>();
        }

        private static bool isKeyNullReference<TKey>(TKey key)
        {
            return !typeof(TKey).IsValueType && (object)key == null;
        }
    }
}

#endif