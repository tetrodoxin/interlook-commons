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
using System;

using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Interlook.Security
{
    /// <summary>
    /// Helper Methods for string encryptions
    /// </summary>
    public static class StringEncryptionExt
    {
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
            if (!string.IsNullOrEmpty(salt))
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
        /// Decrypts an Unicode encoded string that has been encrypted with <see cref="EncryptStringForMachine(string)"/>
        /// using the Windows DPAPI on the same machine.
        /// </summary>
        /// <param name="encryptedTextBase64">The <c>Base64</c>-representation of cipher data
        /// created using <see cref="EncryptStringForMachine(string)"/>.</param>
        /// <returns>The decrypted plaintext.</returns>
        /// <remarks>
        /// Decryption will fail, if the cipher data has been created within another windows context/machine.
        /// </remarks>
        public static string DecryptStringForMachine(this string encryptedTextBase64)
            => DecryptStringForMachine(encryptedTextBase64, Encoding.Unicode);

        /// <summary>
        /// Decrypts a string that has been encrypted with <see cref="EncryptStringForMachine(string, Encoding)" />
        /// using the Windows DPAPI on the same machine and a given character encoding.
        /// </summary>
        /// <param name="encryptedTextBase64">The <c>Base64</c>-representation of cipher data
        /// created using <see cref="EncryptStringForMachine(string, Encoding)" />.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>
        /// The decrypted plaintext.
        /// </returns>
        /// <remarks>
        /// Decryption will fail, if the cipher data has been created within another windows context/machine.
        /// </remarks>
        public static string DecryptStringForMachine(this string encryptedTextBase64, Encoding encoding)
        {
            using (var bytes = EncryptionHelper.DecryptBytesForMachine(Convert.FromBase64String(encryptedTextBase64)))
            {
                return encoding.GetString(bytes);
            }
        }

        /// <summary>
        /// Decrypts a Unicode string that has been encrypted with <see cref="EncryptStringForUser(string)"/>
        /// using the Windows DPAPI for the same user and the default unicode encoding
        /// for decrypted plaintext.
        /// </summary>
        /// <param name="encryptedTextBase64">The <c>Base64</c>-representation of cipher data
        /// created using <see cref="EncryptStringForUser(string)"/>.</param>
        /// <returns>The decrypted plaintext.</returns>
        /// <remarks>
        /// Decryption will fail, if the cipher data has been created within another windows user context.
        /// </remarks>
        public static string DecryptStringForUser(this string encryptedTextBase64)
            => DecryptStringForUser(encryptedTextBase64, Encoding.Unicode);

        /// <summary>
        /// Decrypts a string that has been encrypted with <see cref="EncryptStringForUser(string, Encoding)"/>
        /// using the Windows DPAPI for the same user.
        /// </summary>
        /// <param name="encryptedTextBase64">The <c>Base64</c>-representation of cipher data
        /// created using <see cref="EncryptStringForUser(string, Encoding)"/>.</param>
        /// <param name="encoding">The character encoding to use for decrypted text.</param>
        /// <returns>The decrypted plaintext.</returns>
        /// <remarks>
        /// Decryption will fail, if the cipher data has been created within another windows user context.
        /// </remarks>
        public static string DecryptStringForUser(this string encryptedTextBase64, Encoding encoding)
        {
            using (var bytes = EncryptionHelper.DecryptBytesForUser(Convert.FromBase64String(encryptedTextBase64)))
            {
                return encoding.GetString(bytes);
            }
        }

        /// <summary>
        /// Decrypts the given binary data with AES256 into a unicode (UTF-16) string using given character encoding
        /// and the default initialization vector.
        /// </summary>
        /// <param name="cipherData">Binary cipher data.</param>
        /// <param name="passPhrase">The encryption keyphrase.</param>
        /// <returns>
        /// A <see cref="string"/>, containing the original text, that has been encrypted.
        /// </returns>
        public static string DecryptStringWith(this byte[] cipherData, SecureString passPhrase) 
            => DecryptStringWith(cipherData, passPhrase, EncryptionHelper.DefaultInitVector128, EncryptionHelper.DefaultSalt256, Encoding.Unicode);

        /// <summary>
        /// Decrypts the given binary data with AES256 into a string using given character encoding
        /// and the default initialization vector.
        /// </summary>
        /// <param name="cipherData">Binary cipher data.</param>
        /// <param name="passPhrase">The encryption keyphrase.</param>
        /// <param name="encoding">Character encoding to use.</param>
        /// <returns>
        /// A <see cref="string"/>, containing the original text, that has been encrypted.
        /// </returns>
        public static string DecryptStringWith(this byte[] cipherData, SecureString passPhrase, Encoding encoding)
            => DecryptStringWith(cipherData, passPhrase, EncryptionHelper.DefaultInitVector128, EncryptionHelper.DefaultSalt256, encoding);

        /// <summary>
        /// Decrypts the given binary data with AES256 into a string using given character encoding
        /// and a given initialization vector.
        /// </summary>
        /// <param name="cipherData">Binary cipher data.</param>
        /// <param name="passPhrase">The encryption keyphrase.</param>
        /// <param name="initVector">Blockcipher initialization vector to use.</param>
        /// <param name="salt">The salt used for generating key data from password.</param>
        /// <param name="encoding">Character encoding to use.</param>
        /// <returns>
        /// A <see cref="string"/>, containing the original text, that has been encrypted.
        /// </returns>
        public static string DecryptStringWith(this byte[] cipherData, SecureString passPhrase, byte[] initVector, byte[] salt, Encoding encoding)
        {
            byte[] keyBytes = EncryptionHelper.CreateKeyFromPassphrase(passPhrase, salt, EncryptionHelper.MaxAesKeySize);
            return decryptStringFromBytesAes(cipherData, keyBytes, initVector, encoding);
        }

        /// <summary>
        /// Encrypts the string using Windows DPAPI for the context of the local machine
        /// and the default character encoding <see cref="Encoding.Unicode"/>.
        /// </summary>
        /// <param name="value">The plaintext to encrypt.</param>
        /// <returns>A <see cref="string"/> with the <c>Base64</c> representation of the cipher data.</returns>
        public static string EncryptStringForMachine(this string value)
                    => EncryptStringForMachine(value, Encoding.Unicode);

        /// <summary>
        /// Encrypts the string using Windows DPAPI for the context of the local machine.
        /// </summary>
        /// <param name="value">The plaintext to encrypt.</param>
        /// <param name="encoding">The character encoding to use for <paramref name="value"/></param>
        /// <returns>A <see cref="string"/> with the <c>Base64</c> representation of the cipher data.</returns>
        public static string EncryptStringForMachine(this string value, Encoding encoding)
                    => Convert.ToBase64String(EncryptionHelper.EncryptBytesForMachine(encoding.GetBytes(value)));

        /// <summary>
        /// Encrypts the string using Windows DPAPI for the context of the current user
        /// and the default character encoding <see cref="Encoding.Unicode"/>.
        /// </summary>
        /// <param name="value">The plaintext to encrypt.</param>
        /// <returns>A <see cref="string"/> with the <c>Base64</c> representation of the cipher data.</returns>
        public static string EncryptStringForUser(this string value)
                    => EncryptStringForMachine(value, Encoding.Unicode);

        /// <summary>
        /// Encrypts the string using Windows DPAPI for the context of the current user.
        /// </summary>
        /// <param name="value">The plaintext to encrypt.</param>
        /// <param name="encoding">The character encoding to use for <paramref name="value"/></param>
        /// <returns>A <see cref="string"/> with the <c>Base64</c> representation of the cipher data.</returns>
        public static string EncryptStringForUser(this string value, Encoding encoding)
                    => Convert.ToBase64String(EncryptionHelper.EncryptBytesForMachine(encoding.GetBytes(value)));

        /// <summary>
        /// Encrypts a string using AES256
        /// </summary>
        /// <param name="plainText">String to encrypt.</param>
        /// <param name="passPhrase">The encryption keyphrase.</param>
        /// <param name="initVector">Blockcipher initialization vector to use.</param>
        /// <param name="salt">The salt used for generating key data from password.</param>
        /// <param name="encoding">Character encoding to use.</param>
        /// <returns>A byte-array with cipher data.</returns>
        public static byte[] EncryptStringWith(this string plainText, SecureString passPhrase, byte[] initVector, byte[] salt, Encoding encoding)
        {
            byte[] keyBytes = EncryptionHelper.CreateKeyFromPassphrase(passPhrase, salt, EncryptionHelper.MaxAesKeySize);
            return encryptStringToBytesAes(plainText, keyBytes, initVector, encoding);
        }

        /// <summary>
        /// Encrypts a unicode (UTF-16) string using AES256 and the default initialization vector and default salt.
        /// </summary>
        /// <param name="plainText">String to encrypt.</param>
        /// <param name="passPhrase">The encryption keyphrase.</param>
        /// <returns>A byte-array with cipher data.</returns>
        public static byte[] EncryptStringWith(this string plainText, SecureString passPhrase) 
            => EncryptStringWith(plainText, passPhrase, EncryptionHelper.DefaultInitVector128, EncryptionHelper.DefaultSalt256, Encoding.Unicode);

        /// <summary>
        /// Encrypts a string using AES256 and the default initialization vector and default salt.
        /// </summary>
        /// <param name="plainText">String to encrypt.</param>
        /// <param name="passPhrase">The encryption keyphrase.</param>
        /// <param name="encoding">Character encoding to use.</param>
        /// <returns>A byte-array with cipher data.</returns>
        public static byte[] EncryptStringWith(this string plainText, SecureString passPhrase, Encoding encoding) 
            => EncryptStringWith(plainText, passPhrase, EncryptionHelper.DefaultInitVector128, EncryptionHelper.DefaultSalt256, encoding);

        /// <summary>
        /// Computes the MD5 hash of a string.
        /// </summary>
        /// <param name="stringToHash">string to hash.</param>
        /// <param name="salt">Optional salt.</param>
        /// <returns>The string representation of the computed MD5 hash for the given string.</returns>
        public static string GetMD5(this string stringToHash, string salt = null) => GetMD5(stringToHash, Encoding.Unicode, salt);

        /// <summary>
        /// Computes the MD5 hash of a string.
        /// </summary>
        /// <param name="stringToHash">string to hash.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="salt">Optional salt.</param>
        /// <returns>
        /// The string representation of the computed MD5 hash for the given string.
        /// </returns>
        public static string GetMD5(this string stringToHash, Encoding encoding, string salt = null)
        {
            var md5Hasher = MD5.Create();
            return ComputeHashString(stringToHash, encoding, md5Hasher, salt);
        }

        /// <summary>
        /// Computes the SHA1 hash of a string.
        /// </summary>
        /// <param name="stringToHash">The string to calculate the hash code for.</param>
        /// <param name="salt">A trailing salt string, appended before hashing (optional, may be <c>null</c>).</param>
        /// <returns>The string representation of the computed SHA1 hash for the given string.</returns>
        public static string GetSHA1(this string stringToHash, string salt = null) => GetSHA1(stringToHash, Encoding.Unicode, salt);

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
        public static string GetSHA256(this string stringToHash, string salt = null) => GetSHA256(stringToHash, Encoding.Unicode, salt);

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
        public static string GetSHA512(this string stringToHash, string salt = null) => GetSHA512(stringToHash, Encoding.Unicode, salt);

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

        private static string decryptStringFromBytesAes(byte[] cipherText, byte[] key, byte[] initVector, Encoding encoding)
        {
            using (var bytes = EncryptionHelper.DecrytDataAes(cipherText, key, initVector))
            {
                if (bytes != null)
                    return encoding.GetString(bytes);
                else
                    return string.Empty;
            }
        }

        private static byte[] encryptStringToBytesAes(string plainText, byte[] key, byte[] initVector, Encoding encoding)
            => EncryptionHelper.EncryptDataAes(encoding.GetBytes(plainText), key, initVector);
    }
}