using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Interlook.Text
{
	/// <summary>
	/// A helper class providing methods for checking and manipulating string objects.
	/// </summary>
	public static class StringExtensions
	{
		private static Dictionary<string, string> latinNormCharsArray = new Dictionary<string, string>()
		{
			{ "Ä", "AE" },
			{ "Á", "A" },
			{ "À", "A" },
			{ "Â", "A" },
			{ "Ã", "A" },
			{ "Å", "A" },
			{ "Æ", "AE" },
			{ "Ç", "C" },
			{ "È", "E" },
			{ "É", "E" },
			{ "Ê", "E" },
			{ "Ë", "E" },
			{ "Ì", "I" },
			{ "Í", "I" },
			{ "Î", "I" },
			{ "Ï", "I" },
			{ "Ñ", "N" },
			{ "Ò", "O" },
			{ "Ó", "O" },
			{ "Ô", "O" },
			{ "Õ", "O" },
			{ "Ö", "OE" },
			{ "Ø", "OE" },
			{ "Ù", "U" },
			{ "Ú", "U" },
			{ "Û", "U" },
			{ "Ü", "UE" },
			{ "Ý", "Y" },
			{ "Ÿ", "Y" },
			{ "Š", "S" },
			{ "Œ", "OE" },
			{ "Ž", "Z" },
			{ "ß", "SS" }
		};

		private static readonly char[] maximumStringArray = new char[0xa000];

		/// <summary>
		/// Indicates whether the string is <c>null</c> or an empty string.
		/// </summary>
		/// <param name="str">The string to test.</param>
		/// <returns><c>true</c> if the given string is <c>null</c> or an empty string. Otherwise <c>false</c>.</returns>
		public static bool IsNullOrEmpty(this string str)
		{
			return String.IsNullOrEmpty(str);
		}

		/// <summary>
		/// Indicates whether the string is neither <c>null</c> nor an empty string.
		/// </summary>
		/// <param name="str">The string to test.</param>
		/// <returns><c>true</c> if the given string actually contains characters. Otherwise, if it's empty or even <c>null</c>, <c>false</c> is returned.</returns>
		public static bool AintNullNorEmpty(this string str)
		{
			return !(String.IsNullOrEmpty(str));
		}

		/// <summary>
		/// Indicates whether the string only contains decimal numbers.
		/// </summary>
		/// <param name="str">The string to test.</param>
		/// <returns><c>true</c> if the given string only contains decimal numbers.</returns>
		public static bool IsNumericOnly(this string str)
		{
			return IsNumericOnly(str, new char[0]);
		}

		/// <summary>
		/// Indicates whether the string only contains decimal numbers or special additional characters.
		/// </summary>
		/// <param name="str">The string to test.</param>
		/// <param name="acceptedAdditionalChars">A string, containing additional accepted characters.</param>
		/// <returns><c>true</c> if the given string only contains decimal numbers or the provided additional characters.</returns>
		public static bool IsNumericOnly(this string str, string acceptedAdditionalChars)
		{
			if (String.IsNullOrEmpty(acceptedAdditionalChars))
			{
				return IsNumericOnly(str, new char[0]);
			}

			return IsNumericOnly(str, acceptedAdditionalChars.ToCharArray());
		}

		/// <summary>
		/// Indicates whether the string only contains decimal numbers or special additional characters.
		/// </summary>
		/// <param name="str">The string to test.</param>
		/// <param name="acceptedAdditionalChars">An array of additional accepted characters.</param>
		/// <returns><c>true</c> if the given string only contains decimal numbers or the provided additional characters.</returns>
		public static bool IsNumericOnly(this string str, char[] acceptedAdditionalChars)
		{
			if (acceptedAdditionalChars == null)
			{
				acceptedAdditionalChars = new char[0];
			}

			for (int i = 0; i < str.Length; i++)
			{
				if (acceptedAdditionalChars.Contains(str[i])) continue;
				if ((str[i] < '0') || (str[i] > '9')) return false;
			}

			return true;
		}

		/// <summary>
		/// Removes all leading and trailing white-space characters from a string object, that even may be <c>null</c>.
		/// </summary>
		/// <param name="str">The string object to trim safely (may be null).</param>
		/// <returns>A trimmed copy of the provided string or an empty string, if the parameter string was <c>null</c>.</returns>
		public static string TrimProtected(this string str)
		{
			if (!String.IsNullOrEmpty(str))
			{
				return str.Trim();
			}
			else
			{
				return str;
			}
		}

		/// <summary>
		/// Ensures, that the corresponding string object is no <c>null</c>-reference.
		/// </summary>
		/// <param name="str">The string to test.</param>
		/// <returns>The given string itself, if it was not <c>null</c>, otherwise an empty string is returned.</returns>
		public static string Ensure(this string str)
		{
			return str ?? String.Empty;
		}

		/// <summary>
		/// Returns a string, that always begins with a capital letter.
		/// </summary>
		/// <param name="str">String, where first letter must be capital.</param>
		/// <param name="forceLowercaseRemainder">If <c>true</c>, all characters following the first are guaranteed to be lower case, otherwise left untouched.</param>
		/// <returns>A string with a guaranteed upper case first character.</returns>
		public static string CapitalizedFirstCharacter(this string str, bool forceLowercaseRemainder = false)
		{
			string result = String.Empty;
			if (!String.IsNullOrEmpty(str))
			{
				result = str.Substring(0, 1).ToUpper();
				if (str.Length > 1)
				{
					if (forceLowercaseRemainder)
					{
						result += str.Substring(1).ToLower();
					}
					else
					{
						result += str.Substring(1);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Computes the hash of the string using a specific hash algorithm.
		/// </summary>
		/// <param name="stringToHash">The string to calculate the hash code for.</param>
		/// <param name="encoding">The character encoding used by the given string.</param>
		/// <param name="algorithm">The algorithm to use fpr calculating the hash.</param>
		/// <param name="salt">A trailing salt string, appended before hashing (optional, may be <c>null</c>).</param>
		/// <returns>The string representation of the computed hash for the given string. Any possible hyphens are trimmed.</returns>
		public static string ComputeHashString(this string stringToHash, Encoding encoding, HashAlgorithm algorithm, string salt = null)
		{
			char[] passwordChars;
			if (salt.AintNullNorEmpty())
			{
				passwordChars = (stringToHash + salt).ToArray();
			}
			else
			{
				passwordChars = stringToHash.ToArray();
			}

			return BitConverter.ToString(algorithm.ComputeHash(encoding.GetBytes(passwordChars))).Replace("-", "");
		}

		/// <summary>
		/// Computes the SHA1 hash of a string.
		/// </summary>
		/// <param name="stringToHash">The string to calculate the hash code for.</param>
		/// <param name="salt">A trailing salt string, appended before hashing (optional, may be <c>null</c>).</param>
		/// <returns>The string representation of the computed SHA1 hash for the given string.</returns>
		public static string GetSHA1(this string stringToHash, string salt = null)
		{
			return GetSHA1(stringToHash, Encoding.UTF8, salt);
		}

		/// <summary>
		/// Computes the SHA1 hash of a string.
		/// </summary>
		/// <param name="stringToHash">The string to calculate the hash code for.</param>
		/// <param name="encoding">The character encoding used by the given string.</param>
		/// <param name="salt">A trailing salt string, appended before hashing (optional, may be <c>null</c>).</param>
		/// <returns>The string representation of the computed SHA1 hash for the given string.</returns>
		public static string GetSHA1(this string stringToHash, Encoding encoding, string salt = null)
		{
			var shaM = new SHA1CryptoServiceProvider();
			return ComputeHashString(stringToHash, encoding, shaM, salt);
		}

		/// <summary>
		/// Computes the SHA256 hash of a string.
		/// </summary>
		/// <param name="stringToHash">The string to calculate the hash code for.</param>
		/// <param name="salt">A trailing salt string, appended before hashing (optional, may be <c>null</c>).</param>
		/// <returns>The string representation of the computed SHA256 hash for the given string.</returns>
		public static string GetSHA256(this string stringToHash, string salt = null)
		{
			return GetSHA256(stringToHash, Encoding.UTF8, salt);
		}

		/// <summary>
		/// Computes the SHA256 hash of a string.
		/// </summary>
		/// <param name="stringToHash">The string to calculate the hash code for.</param>
		/// <param name="encoding">The character encoding used by the given string.</param>
		/// <param name="salt">A trailing salt string, appended before hashing (optional, may be <c>null</c>).</param>
		/// <returns>The string representation of the computed SHA256 hash for the given string.</returns>
		public static string GetSHA256(this string stringToHash, Encoding encoding, string salt = null)
		{
			var shaM = new SHA256Managed();
			return ComputeHashString(stringToHash, encoding, shaM, salt);
		}

		/// <summary>
		/// Computes the SHA512 hash of a string.
		/// </summary>
		/// <param name="stringToHash">The string to calculate the hash code for.</param>
		/// <param name="salt">A trailing salt string, appended before hashing (optional, may be <c>null</c>).</param>
		/// <returns>The string representation of the computed SHA512 hash for the given string.</returns>
		public static string GetSHA512(this string stringToHash, string salt = null)
		{
			return GetSHA512(stringToHash, Encoding.UTF8, salt);
		}

		/// <summary>
		/// Computes the SHA512 hash of a string.
		/// </summary>
		/// <param name="stringToHash">The string to calculate the hash code for.</param>
		/// <param name="encoding">The character encoding used by the given string.</param>
		/// <param name="salt">A trailing salt string, appended before hashing (optional, may be <c>null</c>).</param>
		/// <returns>The string representation of the computed SHA512 hash for the given string.</returns>
		public static string GetSHA512(this string stringToHash, Encoding encoding, string salt = null)
		{
			var shaM = new SHA512Managed();
			return ComputeHashString(stringToHash, encoding, shaM, salt);
		}

		/// <summary>
		/// Returns a normalized copy of a string, meaning only capitals without any accent signs (e.g. accent circumflex).
		/// </summary>
		/// <param name="str">The string to normalize.</param>
		/// <returns>A capitalized string that represents the provided string in a way to be comparable case- and accent-insensitive.</returns>
		public static string NormalizeLatinChars(this string str)
		{
			if (str.IsNullOrEmpty())
			{
				return str;
			}
			else
			{
				str = str.ToUpper();
				foreach (var pair in latinNormCharsArray)
				{
					str = str.Replace(pair.Key, pair.Value);
				}

				return str;
			}
		}

		/// <summary>
		/// Compares a string to another, by trying to achieve an almost constant time,
		/// to prevent side channel attacks (timing attacks)
		/// </summary>
		/// <param name="original">The original string</param>
		/// <param name="candidate">The string to compare to</param>
		/// <returns><c>true</c> if the two strings match exactly (so even case sensitive), otherwise <c>false</c></returns>
		public static bool SecureEquals(this string original, string candidate)
		{
			return SecureEquals(original.ToCharArray(), candidate.ToCharArray());
		}

		/// <summary>
		/// Compares an array of characters to another, by trying to achieve an almost constant time,
		/// to prevent side channel attacks (timing attacks)
		/// </summary>
		/// <param name="str">The original characters.</param>
		/// <param name="candidate">The char array to compare.</param>
		/// <returns><c>true</c> if the two arrays match exactly (so even case sensitive), otherwise <c>false</c></returns>
		public static bool SecureEquals(this char[] original, char[] candidate)
		{
			//if (candidate.Length > maximumStringArray.Length)
			//{
			//    return false;
			//}

			var jessas = Math.Min(original.Length, candidate.Length);
			var rest = candidate.Length - jessas;

			int result2 = original.Length ^ candidate.Length;
			for (int i = 0; i < jessas; i++)
			{
				result2 |= (original[i] ^ candidate[i]);
			}

			for (int i = 0; i < rest; i++)
			{
				result2 |= (original[i] ^ candidate[i]);
			}

			return result2 == 0;

			char[] copy = new char[candidate.Length];
			int bytesFromOriginal = Math.Min(original.Length, copy.Length);

			Array.Copy(original, 0, copy, 0, bytesFromOriginal);
			Array.Copy(maximumStringArray, 0, copy, bytesFromOriginal, copy.Length - bytesFromOriginal);

			int result = original.Length ^ candidate.Length;
			for (int i = 0; i < copy.Length; i++)
			{
				result |= (copy[i] ^ candidate[i]);
			}

			return result == 0;
		}
	}
}