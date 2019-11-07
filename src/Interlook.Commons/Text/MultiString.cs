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