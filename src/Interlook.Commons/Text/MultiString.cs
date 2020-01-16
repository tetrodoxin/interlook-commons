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
using System.Collections;
using System.Collections.Generic;

namespace Interlook.Text
{
    /// <summary>
    /// Class for strings, that may contain additional alternative strings, to
    /// extend the actual content.
    /// </summary>
    /// <remarks>
    /// Appended strings may be seen as kind of optional extension of the main content,
    /// as like translations, captions, remarks, categories etc.
    /// </remarks>
    public class MultiString : IEnumerable<KeyValuePair<string, string>>
    {
        private Dictionary<string, string> internalDict = new Dictionary<string, string>();

        /// <summary>
        /// Main/default content of the string
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Set or gets an additional/alternative content string.
        /// </summary>
        /// <param name="indexer">Name/identifier of the additional string or <c>null</c> or an empty string for the main string.</param>
        public string this[string indexer]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(indexer))
                {
                    return Content;
                }
                else if (internalDict.ContainsKey(indexer))
                {
                    return internalDict[indexer];
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (string.IsNullOrWhiteSpace(indexer))
                {
                    Content = value;
                }
                else
                {
                    internalDict[indexer] = value;
                }
            }
        }

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="MultiString"/> instance.
        /// </summary>
        public MultiString()
        {
            Content = string.Empty;
        }

        /// <summary>
        /// Creates a new <see cref="MultiString"/> instance with initial content.
        /// </summary>
        /// <param name="content">Content of the Multistring.</param>
        public MultiString(string content)
        {
            Content = content;
        }

        #endregion Constructors

        #region Cast operators

        /// <summary>
        /// Performs an implicit conversion from <see cref="MultiString"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator string(MultiString obj)
        {
            if (obj != null)
            {
                return obj.Content;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="MultiString"/>.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator MultiString(string str)
        {
            var result = new MultiString();
            if (!string.IsNullOrEmpty(str))
            {
                result.Content = str;
            }

            return result;
        }

        #endregion Cast operators

        #region IEnumerator

        /// <summary>
        /// Returns an enumerator that iterates through the strings.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through all contained strings.
        /// </returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            yield return new KeyValuePair<string, string>(null, Content);

            foreach (var keyPair in internalDict)
            {
                yield return keyPair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerator
    }
}