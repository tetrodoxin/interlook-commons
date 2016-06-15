#region license

//MIT License

//Copyright(c) 2016 Andreas Huebner

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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace System
{
    /// <summary>
    /// Helper Methods for string encryptions
    /// </summary>
    public static class StringEncryptionExt
    {
        private const string DefaultSalt = "F[syek+c;,h:d+ovw#y;egvrz/omCc8*";
        private static readonly byte[] _entropyBytes = Encoding.UTF8.GetBytes("h7t2y=!5@YPFQEE^TYr-Rhu");
        private static byte[] DefaultInitVector = new byte[16]
        {
          (byte) 217,
          (byte) 11,
          (byte) 91,
          (byte) 133,
          (byte) 117,
          (byte) 29,
          (byte) 243,
          (byte) 7,
          (byte) 198,
          (byte) 52,
          (byte) 77,
          (byte) 189,
          (byte) 202,
          (byte) 133,
          (byte) 17,
          (byte) 93
        };

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
        {
            return DecryptStringWith(cipherData, passPhrase, DefaultInitVector, encoding);
        }

        /// <summary>
        /// Decrypts the given binary data with AES256 into a string using given character encoding
        /// and a given initialization vector.
        /// </summary>
        /// <param name="cipherData">Binary cipher data.</param>
        /// <param name="passPhrase">The encryption keyphrase.</param>
        /// <param name="iv">Blockcipher initialization vector to use.</param>
        /// <param name="encoding">Character encoding to use.</param>
        /// <returns>
        /// A <see cref="string"/>, containing the original text, that has been encrypted.
        /// </returns>
        public static string DecryptStringWith(this byte[] cipherData, SecureString passPhrase, byte[] iv, Encoding encoding)
        {
            byte[] keyBytes = createKeyBytes(passPhrase, DefaultSalt, encoding);
            return decryptStringFromBytesAES(cipherData, keyBytes, iv, encoding);
        }

        /// <summary>
        /// Encrypts a string using AES256
        /// </summary>
        /// <param name="plainText">String to encrypt.</param>
        /// <param name="passPhrase">The encryption keyphrase.</param>
        /// <param name="iv">Blockcipher initialization vector to use.</param>
        /// <returns>A byte-array with cipher data.</returns>
        public static byte[] EncryptStringWith(this string plainText, SecureString passPhrase, byte[] iv)
        {
            var encoding = Encoding.Unicode;
            var salt = DefaultSalt;
            byte[] keyBytes = createKeyBytes(passPhrase, salt, encoding);
            return encryptStringToBytesAES(plainText, keyBytes, iv, encoding);
        }

        /// <summary>
        /// Encrypts a string using AES256 and the default initialization vector.
        /// </summary>
        /// <param name="plainText">String to encrypt.</param>
        /// <param name="passPhrase">The encryption keyphrase.</param>
        /// <returns>A byte-array with cipher data.</returns>
        public static byte[] EncryptStringWith(this string plainText, SecureString passPhrase)
        {
            return EncryptStringWith(plainText, passPhrase, DefaultInitVector);
        }

        /// <summary>
        /// Decrypts a string that has been encrypted with <see cref="EncryptStringForUser(string)"/>
        /// using the Windows DPAPI for the same user.
        /// </summary>
        /// <param name="encryptedString">The <c>Base64</c>-representation of cipher data
        /// created using <see cref="EncryptStringForUser(string)"/>.</param>
        /// <returns>The decrypted plaintext.</returns>
        /// <remarks>
        /// Decryption will fail, if the cipher data has been created within another windows user context.
        /// </remarks>
        public static string DecryptStringForUser(string encryptedString)
        {
            byte[] bytes = ProtectedData.Unprotect(
                Convert.FromBase64String(encryptedString),
                _entropyBytes,
                DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Decrypts a string that has been encrypted with <see cref="EncryptStringForMachine(string)"/>
        /// using the Windows DPAPI on the same machine.
        /// </summary>
        /// <param name="encryptedString">The <c>Base64</c>-representation of cipher data
        /// created using <see cref="EncryptStringForMachine(string)"/>.</param>
        /// <returns>The decrypted plaintext.</returns>
        /// <remarks>
        /// Decryption will fail, if the cipher data has been created within another windows context/machine.
        /// </remarks>
        public static string DecryptStringForMachine(string encryptedString)
        {
            byte[] bytes = ProtectedData.Unprotect(
                Convert.FromBase64String(encryptedString),
                _entropyBytes,
                DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Encrypts the string using Windows DPAPI for the context of the local machine.
        /// </summary>
        /// <param name="value">The plaintext to encrypt.</param>
        /// <returns>A <see cref="string"/> with the <c>Base64</c> representation of the cipher data.</returns>
        public static string EncryptStringForMachine(string value)
                    => Convert.ToBase64String(
                        ProtectedData.Protect(
                            Encoding.UTF8.GetBytes(value ?? string.Empty),
                            _entropyBytes,
                            DataProtectionScope.LocalMachine));

        /// <summary>
        /// Encrypts the string using Windows DPAPI for the context of the current user.
        /// </summary>
        /// <param name="value">The plaintext to encrypt.</param>
        /// <returns>A <see cref="string"/> with the <c>Base64</c> representation of the cipher data.</returns>
        public static string EncryptStringForUser(string value)
                    => Convert.ToBase64String(
                        ProtectedData.Protect(
                            Encoding.UTF8.GetBytes(value ?? string.Empty),
                            _entropyBytes,
                            DataProtectionScope.CurrentUser));

        private static void commitToResultList(bool condition, List<string> result, StringBuilder sb)
        {
            if (condition && sb.Length > 0)
            {
                result.Add(sb.ToString());
                sb.Clear();
            }
        }

        private static bool isStringNothing(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    if (str.Trim().Length > 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static byte[] createKeyBytes(SecureString passphrase, string completeSalt, Encoding encoding)
        {
            int num1 = (passphrase.Length + 1) * 2;
            byte[] destination = new byte[Math.Max(32, num1)];
            byte[] bytes = encoding.GetBytes(completeSalt);

            if (num1 < 32)
            {
                Array.Copy((Array)bytes, 0, (Array)destination, 0, 32);
            }

            IntPtr num2 = IntPtr.Zero;
            try
            {
                num2 = Marshal.SecureStringToGlobalAllocUnicode(passphrase);
                Marshal.Copy(num2, destination, 0, num1);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Could not marshal SecureString.", ex);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(num2);
            }
            int length = 32;
            byte[] numArray = new byte[length];
            Array.Copy((Array)destination, 0, (Array)numArray, 0, length);
            return numArray;
        }

        private static byte[] decryptBytesFromBytesAES(byte[] cipherText, byte[] key, byte[] iv, Encoding encoding)
        {
            List<byte> list = new List<byte>(cipherText.Length);
            using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
            {
                cryptoServiceProvider.Key = key;
                cryptoServiceProvider.IV = iv;
                ICryptoTransform decryptor = ((SymmetricAlgorithm)cryptoServiceProvider).CreateDecryptor(cryptoServiceProvider.Key, cryptoServiceProvider.IV);
                using (MemoryStream memoryStream = new MemoryStream(cipherText))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        for (int index = cryptoStream.ReadByte(); index >= 0; index = cryptoStream.ReadByte())
                            list.Add((byte)index);
                    }
                }
            }

            return list.ToArray();
        }

        private static string decryptStringFromBytesAES(byte[] cipherText, byte[] key, byte[] iv, Encoding encoding)
        {
            byte[] bytes = decryptBytesFromBytesAES(cipherText, key, iv, encoding);
            if (bytes != null)
                return encoding.GetString(bytes);
            else
                return (string)null;
        }

        private static byte[] encryptBytesToBytesAES(byte[] plainText, byte[] key, byte[] iv)
        {
            byte[] numArray;
            using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
            {
                cryptoServiceProvider.KeySize = 256;
                cryptoServiceProvider.Key = key;
                cryptoServiceProvider.IV = iv;
                ICryptoTransform encryptor = ((SymmetricAlgorithm)cryptoServiceProvider).CreateEncryptor(cryptoServiceProvider.Key, cryptoServiceProvider.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainText, 0, plainText.Length);
                        cryptoStream.FlushFinalBlock();
                        numArray = memoryStream.ToArray();
                    }
                }
            }

            return numArray;
        }

        private static byte[] encryptStringToBytesAES(string plainText, byte[] key, byte[] iv, Encoding encoding)
        {
            return encryptBytesToBytesAES(encoding.GetBytes(plainText), key, iv);
        }
    }
}