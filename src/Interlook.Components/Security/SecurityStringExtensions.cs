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
using Interlook.Security;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace System.Security
{
    /// <summary>
    /// Defining extension/helper methods fo <see cref="SecureString"/>
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// Returns the provided <see cref="SecureString"/> instance
        /// in read only state.
        /// </summary>
        /// <param name="secureString">The <see cref="SecureString"/> object to make read only.</param>
        /// <returns>The <see cref="SecureString"/> instance initially provided in <paramref name="secureString"/></returns>
        public static SecureString AsReadOnly(this SecureString secureString)
        {
            secureString?.MakeReadOnly();
            return secureString;
        }

        /// <summary>
        /// Calculate a hash value of the content of the <see cref="SecureString"/> using
        /// the specified hashing algorithm.
        /// </summary>
        /// <param name="password">The <see cref="SecureString"/>, whose content shall be hashed.</param>
        /// <param name="encoding">The character encoding, that is used by the <see cref="SecureString"/></param>
        /// <param name="hasher">The hashing algorithm to use.</param>
        /// <param name="salt">(Optional). A salt value to append to the content of <paramref name="password"/></param>
        /// <returns>A string, representing the hash value of <paramref name="password"/></returns>
        public static string ComputeHashString(this SecureString password, Encoding encoding, HashAlgorithm hasher, string salt = null)
        {
            string result = null;
            if (password != null)
            {
                bool hasSalt = !string.IsNullOrEmpty(salt);

                int passwordLength = hasSalt
                    ? password.Length
                    : password.Length + salt.Length;

                var passwordChars = new char[passwordLength];    // unfortunately byte array is managed too :-(

                // Copy the password from SecureString to our char array
                IntPtr passwortPointer = Marshal.SecureStringToBSTR(password);
                Marshal.Copy(passwortPointer, passwordChars, 0, passwordLength);
                Marshal.ZeroFreeBSTR(passwortPointer);

                if (hasSalt)
                {
                    Array.Copy(salt.ToArray(), 0, passwordChars, password.Length, salt.Length);
                }

                var toHash = encoding.GetBytes(passwordChars);

                // Hash the char array
                var hashedPasswordBytes = hasher.ComputeHash(toHash);

                // Wipe the character array from memory (at least increments security, but vulnerability remains)
                for (int i = 0; i < passwordChars.Length; i++)
                {
                    passwordChars[i] = '\0';
                }

                result = convertToHexString(hashedPasswordBytes);
            }
            return result;
        }

        /// <summary>
        /// Returns a <see cref="SecureString"/> with unicode (UTF-16) content encrypted via DP-API for the local machine
        /// using the default entropy data in <see cref="EncryptionHelper.DefaultEntropy256"/>.
        /// </summary>
        /// <param name="encryptedData">The actual string content to decrypt,
        /// which was initially encrypted via Windows DP-API for the current user.</param>
        /// <returns>A read only <see cref="SecureString"/> containing the decrypted content.</returns>
        public static SecureString DecryptSecureStringForMachine(this byte[] encryptedData)
            => DecryptSecureStringForMachine(encryptedData, EncryptionHelper.DefaultEntropy256);

        /// <summary>
        /// Returns a <see cref="SecureString"/> with unicode (UTF-16) content encrypted via DP-API for the local machine
        /// </summary>
        /// <param name="encryptedData">The actual string content to decrypt,
        /// which was initially encrypted via Windows DP-API for the current user.</param>
        /// <param name="entropy">A byte array used to increase the complexity of the encryption.
        /// It doesn't need to be secret, but the same for encryption and decryption, just like a salt.</param>
        /// <returns>A read only <see cref="SecureString"/> containing the decrypted content.</returns>
        public static SecureString DecryptSecureStringForMachine(this byte[] encryptedData, byte[] entropy)
        {
            using (var decryptedData = EncryptionHelper.DecryptBytesForMachine(encryptedData, entropy))
            {
                return createReadOnlySecureStringFromBytes(decryptedData);
            }
        }

        /// <summary>
        /// Returns a <see cref="SecureString"/> with unicode (UTF-16) content encrypted via DP-API for the current user
        /// using the default entropy data in <see cref="EncryptionHelper.DefaultEntropy256"/>.
        /// </summary>
        /// <param name="encryptedData">The actual string content to decrypt,
        /// which was initially encrypted via Windows DP-API for the current user.</param>
        /// <returns>A read only <see cref="SecureString"/> containing the decrypted content.</returns>
        public static SecureString DecryptSecureStringForUser(this byte[] encryptedData)
            => DecryptSecureStringForUser(encryptedData, EncryptionHelper.DefaultEntropy256);

        /// <summary>
        /// Returns a <see cref="SecureString"/> with unicode (UTF-16) content encrypted via DP-API for the current user
        /// </summary>
        /// <param name="encryptedData">The actual string content to decrypt,
        /// which was initially encrypted via Windows DP-API for the current user.</param>
        /// <param name="entropy">A byte array used to increase the complexity of the encryption.
        /// It doesn't need to be secret, but the same for encryption and decryption, just like a salt.</param>
        /// <returns>A read only <see cref="SecureString"/> containing the decrypted content.</returns>
        public static SecureString DecryptSecureStringForUser(this byte[] encryptedData, byte[] entropy)
        {
            using (var decryptedData = EncryptionHelper.DecryptBytesForUser(encryptedData, entropy))
            {
                return createReadOnlySecureStringFromBytes(decryptedData);
            }
        }

        /// <summary>
        /// Encrypts a <see cref="SecureString"/> using AES256
        /// </summary>
        /// <param name="encryptedData">The actual content to decrypt.</param>
        /// <param name="passPhrase">The encryption keyphrase.</param>
        /// <param name="initVector">Blockcipher initialization vector to use.</param>
        /// <param name="salt">The salt used for generating key data from password.</param>
        /// <returns>A read only <see cref="SecureString"/> containing the decrypted data.</returns>
        public static SecureString DecryptSecureStringWith(this byte[] encryptedData, SecureString passPhrase, byte[] initVector, byte[] salt)
        {
            using (var keyBytes = EncryptionHelper.CreateKeyFromPassphrase(passPhrase, salt, EncryptionHelper.MaxAesKeySize))
            {
                return decryptSecureStringAes(encryptedData, keyBytes, initVector);
            }
        }

        /// <summary>
        /// Encrypts a <see cref="SecureString"/> using AES256
        /// </summary>
        /// <param name="encryptedData">The actual content to decrypt.</param>
        /// <param name="passPhrase">The encryption keyphrase.</param>
        /// <returns>A read only <see cref="SecureString"/> containing the decrypted data.</returns>
        public static SecureString DecryptSecureStringWith(this byte[] encryptedData, SecureString passPhrase)
            => DecryptSecureStringWith(encryptedData, passPhrase, EncryptionHelper.DefaultInitVector128, EncryptionHelper.DefaultSalt256);

        /// <summary>
        /// Encrypts the unicode (UTF-16) content using Windows DPAPI for the context of the local machine
        /// using the default entropy data in <see cref="EncryptionHelper.DefaultEntropy256"/>.
        /// </summary>
        /// <param name="value">The plaintext to encrypt.</param>
        /// <returns>A byte array containing the encrypted content of <paramref name="value"/>.</returns>
        public static byte[] EncryptForMachine(this SecureString value)
            => EncryptForMachine(value, EncryptionHelper.DefaultEntropy256);

        /// <summary>
        /// Encrypts the unicode (UTF-16) content using Windows DPAPI for the context of the local machine.
        /// </summary>
        /// <param name="value">The plaintext to encrypt.</param>
        /// <param name="entropy">A byte array used to increase the complexity of the encryption.
        /// It doesn't need to be secret, but the same for encryption and decryption, just like a salt.</param>
        /// <returns>A byte array containing the encrypted content of <paramref name="value"/>.</returns>
        public static byte[] EncryptForMachine(this SecureString value, byte[] entropy)
        {
            using (var data = value.GetBytes())
            {
                return EncryptionHelper.EncryptBytesForMachine(data, entropy);
            }
        }

        /// <summary>
        /// Encrypts the unicode (UTF-16) content using Windows DPAPI for the context of the current
        /// using the default entropy data in <see cref="EncryptionHelper.DefaultEntropy256"/>.
        /// </summary>
        /// <param name="value">The plaintext to encrypt.</param>
        /// <returns>A byte array containing the encrypted content of <paramref name="value"/>.</returns>
        public static byte[] EncryptForUser(this SecureString value)
            => EncryptForUser(value, EncryptionHelper.DefaultEntropy256);

        /// <summary>
        /// Encrypts the unicode (UTF-16) content using Windows DPAPI for the context of the current.
        /// </summary>
        /// <param name="value">The plaintext to encrypt.</param>
        /// <param name="entropy">A byte array used to increase the complexity of the encryption.
        /// It doesn't need to be secret, but the same for encryption and decryption, just like a salt.</param>
        /// <returns>A byte array containing the encrypted content of <paramref name="value"/>.</returns>
        public static byte[] EncryptForUser(this SecureString value, byte[] entropy)
        {
            using (var data = value.GetBytes())
            {
                return EncryptionHelper.EncryptBytesForUser(data, entropy);
            }
        }

        /// <summary>
        /// Encrypts a <see cref="SecureString"/> using AES256
        /// </summary>
        /// <param name="plainText">String to encrypt.</param>
        /// <param name="passPhrase">The encryption keyphrase.</param>
        /// <param name="initVector">Blockcipher initialization vector to use.</param>
        /// <param name="salt">The salt used for generating key data from password.</param>
        /// <returns>A byte-array with cipher data.</returns>
        public static byte[] EncryptWith(this SecureString plainText, SecureString passPhrase, byte[] initVector, byte[] salt)
        {
            using (var keyBytes = EncryptionHelper.CreateKeyFromPassphrase(passPhrase, salt, EncryptionHelper.MaxAesKeySize))
            {
                return encryptSecureStringAes(plainText, keyBytes, initVector);
            }
        }

        /// <summary>
        /// Encrypts a unicode (UTF-16) string using AES256 and the default initialization vector and default salt.
        /// </summary>
        /// <param name="plainText">String to encrypt.</param>
        /// <param name="passPhrase">The encryption keyphrase.</param>
        /// <returns>A byte-array with cipher data.</returns>
        public static byte[] EncryptWith(this SecureString plainText, SecureString passPhrase)
            => EncryptWith(plainText, passPhrase, EncryptionHelper.DefaultInitVector128, EncryptionHelper.DefaultSalt256);

        /// <summary>
        /// Returns the content of a <see cref="SecureString" /> as bytes
        /// using unicode character encoding.
        /// </summary>
        /// <param name="secString">The <see cref="SecureString" />, whose content is to be extracted.</param>
        /// <returns>A <see cref="DisposableBytes"/> object containing the content of the secure string.</returns>
        /// <remarks>This method returns the originally secured data of the provided <see cref="SecureString"/>
        /// in a no longer secured way. Consider clearing the returned array immediately after processing,
        /// preferably by embedding the result into an <c>using</c> statement!</remarks>
        public static DisposableBytes GetBytes(this SecureString secString)
        {
            int passwordLength = (secString.Length + 1) * 2;
            byte[] passwordBytes = new byte[passwordLength];

            IntPtr passwortPointer = IntPtr.Zero;

            try
            {
                passwortPointer = Marshal.SecureStringToGlobalAllocUnicode(secString);
                Marshal.Copy(passwortPointer, passwordBytes, 0, passwordLength);
                return new DisposableBytes(passwordBytes);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(passwortPointer);
            }
        }

        /// <summary>
        /// Returns the Unicode content of a <see cref="SecureString" /> as an array of characters.
        /// </summary>
        /// <param name="secString">The <see cref="SecureString" />, whose content is to be extracted.</param>
        /// <returns>
        /// An array of characters, representing the content of <paramref name="secString" />
        /// </returns>
        /// <remarks>This method returns the originally secured data of the provided <see cref="SecureString"/>
        /// in a no longer secured way. Consider clearing the returned array immediately after processing!</remarks>
        public static char[] GetChars(this SecureString secString) => GetChars(secString, Encoding.Unicode);

        /// <summary>
        /// Returns the content of a <see cref="SecureString" /> as an array of characters.
        /// </summary>
        /// <param name="secString">The <see cref="SecureString" />, whose content is to be extracted.</param>
        /// <param name="encoding">The character encoding used by the secure string.</param>
        /// <returns>
        /// An array of characters, representing the content of <paramref name="secString" />
        /// </returns>
        /// <remarks>This method returns the originally secured data of the provided <see cref="SecureString"/>
        /// in a no longer secured way. Consider clearing the returned array immediately after processing!</remarks>
        public static char[] GetChars(this SecureString secString, Encoding encoding)
        {
            int passwordLength = (secString.Length + 1) * 2;
            byte[] passwordBytes = new byte[passwordLength];

            IntPtr passwortPointer = IntPtr.Zero;

            try
            {
                passwortPointer = Marshal.SecureStringToGlobalAllocUnicode(secString);
                Marshal.Copy(passwortPointer, passwordBytes, 0, passwordLength);
                return encoding.GetChars(passwordBytes);
            }
            finally
            {
                if (passwortPointer != IntPtr.Zero)
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(passwortPointer);
                }
            }
        }

        /// <summary>
        /// Calculate the MD5 hash of the content of the <see cref="SecureString"/>
        /// using Unicode character encoding for the <c>SecureString</c>'s content.
        /// </summary>
        /// <param name="password">The <see cref="SecureString"/>, whose content shall be hashed.</param>
        /// <param name="salt">(Optional). A salt value to append to the content of <paramref name="password"/></param>
        /// <returns>A string, representing the MD5 hash value of <paramref name="password"/></returns>
        public static string GetMD5(this SecureString password, string salt = null)
        {
            return GetMD5(password, Encoding.Unicode, salt);
        }

        /// <summary>
        /// Calculate the MD5 hash of the content of the <see cref="SecureString"/>.
        /// </summary>
        /// <param name="password">The <see cref="SecureString"/>, whose content shall be hashed.</param>
        /// <param name="encoding">The character encoding, that is used by the <see cref="SecureString"/></param>
        /// <param name="salt">(Optional). A salt value to append to the content of <paramref name="password"/></param>
        /// <returns>A string, representing the MD5 hash value of <paramref name="password"/></returns>
        public static string GetMD5(this SecureString password, Encoding encoding, string salt = null)
        {
            var md5Hasher = MD5.Create();
            return ComputeHashString(password, encoding, md5Hasher, salt);
        }

        /// <summary>
        /// Calculate the SHA-1 hash of the content of the <see cref="SecureString"/>
        /// using Unicode character encoding for the <c>SecureString</c>'s content.
        /// </summary>
        /// <param name="password">The <see cref="SecureString"/>, whose content shall be hashed.</param>
        /// <param name="salt">(Optional). A salt value to append to the content of <paramref name="password"/></param>
        /// <returns>A string, representing the SHA-256 hash value of <paramref name="password"/></returns>
        public static string GetSHA1(this SecureString password, string salt = null)
        {
            return GetSHA1(password, Encoding.Unicode, salt);
        }

        /// <summary>
        /// Calculate the SHA-1 hash of the content of the <see cref="SecureString"/>.
        /// </summary>
        /// <param name="password">The <see cref="SecureString"/>, whose content shall be hashed.</param>
        /// <param name="encoding">The character encoding, that is used by the <see cref="SecureString"/></param>
        /// <param name="salt">(Optional). A salt value to append to the content of <paramref name="password"/></param>
        /// <returns>A string, representing the SHA-256 hash value of <paramref name="password"/></returns>
        public static string GetSHA1(this SecureString password, Encoding encoding, string salt = null)
        {
            var hasher = SHA1.Create();
            string result = ComputeHashString(password, encoding, hasher, salt);

            return result;
        }

        /// <summary>
        /// Calculate the SHA-256 hash of the content of the <see cref="SecureString"/>
        /// using Unicode character encoding for the <c>SecureString</c>'s content.
        /// </summary>
        /// <param name="password">The <see cref="SecureString"/>, whose content shall be hashed.</param>
        /// <param name="salt">(Optional). A salt value to append to the content of <paramref name="password"/></param>
        /// <returns>A string, representing the SHA-256 hash value of <paramref name="password"/></returns>
        public static string GetSHA256(this SecureString password, string salt = null)
        {
            return GetSHA256(password, Encoding.Unicode, salt);
        }

        /// <summary>
        /// Calculate the SHA-256 hash of the content of the <see cref="SecureString"/>.
        /// </summary>
        /// <param name="password">The <see cref="SecureString"/>, whose content shall be hashed.</param>
        /// <param name="encoding">The character encoding, that is used by the <see cref="SecureString"/></param>
        /// <param name="salt">(Optional). A salt value to append to the content of <paramref name="password"/></param>
        /// <returns>A string, representing the SHA-256 hash value of <paramref name="password"/></returns>
        public static string GetSHA256(this SecureString password, Encoding encoding, string salt = null)
        {
            var hasher = SHA256.Create();
            string result = ComputeHashString(password, encoding, hasher, salt);

            return result;
        }

        /// <summary>
        /// Calculate the SHA-512 hash of the content of the <see cref="SecureString"/>
        /// using Unicode character encoding for the <c>SecureString</c>'s content.
        /// </summary>
        /// <param name="password">The <see cref="SecureString"/>, whose content shall be hashed.</param>
        /// <param name="salt">(Optional). A salt value to append to the content of <paramref name="password"/></param>
        /// <returns>A string, representing the SHA-512 hash value of <paramref name="password"/></returns>
        public static string GetSHA512(this SecureString password, string salt = null)
        {
            return GetSHA512(password, Encoding.Unicode, salt);
        }

        /// <summary>
        /// Calculate the SHA-512 hash of the content of the <see cref="SecureString"/>.
        /// </summary>
        /// <param name="password">The <see cref="SecureString"/>, whose content shall be hashed.</param>
        /// <param name="encoding">The character encoding, that is used by the <see cref="SecureString"/></param>
        /// <param name="salt">(Optional). A salt value to append to the content of <paramref name="password"/></param>
        /// <returns>A string, representing the SHA-512 hash value of <paramref name="password"/></returns>
        public static string GetSHA512(this SecureString password, Encoding encoding, string salt = null)
        {
            HashAlgorithm hasher = SHA512.Create();
            string result = ComputeHashString(password, encoding, hasher, salt);

            return result;
        }

        /// <summary>
        /// Returns the Unicode content of a <see cref="SecureString"/> as a string.
        /// </summary>
        /// <param name="secString">The <see cref="SecureString"/>, whose content is to be extracted.</param>
        /// <returns>A string object, representing the content of <paramref name="secString"/></returns>
        /// <remarks>This method returns originally secured data in an insecure way.</remarks>
        public static string GetValue(this SecureString secString)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// Fills a <see cref="SecureString"/> with the content of an array of characters.
        /// </summary>
        /// <param name="secureString">The <see cref="SecureString"/> to receive the characters.</param>
        /// <param name="initChars">The actual string content to be secured inside <paramref name="secureString"/></param>
        /// <returns>The instance of <see cref="SecureString"/> that was provided in <paramref name="secureString"/> (fluent API)</returns>
        public static SecureString InitFromCharArray(this SecureString secureString, char[] initChars)
        {
            if (secureString != null)
            {
                if (secureString.IsReadOnly())
                {
                    throw new InvalidOperationException("SecureString ist readonly.");
                }

                secureString.Clear();
                if (initChars != null && initChars.Length > 0)
                {
                    foreach (var c in initChars)
                    {
                        if (c == 0)
                        {
                            break;
                        }
                        secureString.AppendChar(c);
                    }
                }
            }

            return secureString;
        }

        /// <summary>
        /// Reads a password through <see cref="Console"/> into a <see cref="SecureString"/>.
        /// </summary>
        /// <param name="target"><see cref="SecureString"/>, that shall receive the password.</param>
        /// <param name="prompt">The text to output on the console before the input caret.</param>
        /// <param name="useMask">Sets, if a masking character shall be used instead of no output of entered characters.</param>
        /// <param name="mask">Character to use for masking.</param>
        /// <returns><c>true</c>, if a password has been entered;
        /// <c>false</c>, if the user cancelled.</returns>
        public static bool PromptForPassword(this SecureString target, string prompt, bool useMask = true, char mask = '*')
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            ConsoleKeyInfo key;
            Console.Write(prompt ?? string.Empty);
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Escape)
                {
                    target.Clear();
                    return false;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (target.Length > 0)
                    {
                        target.RemoveAt(target.Length - 1);

                        if (useMask)
                        {
                            // remove last mask char/string
                            Console.Write(key.KeyChar);
                            Console.Write(' ');
                            Console.Write(key.KeyChar);
                        }
                    }
                }
                else
                {
                    target.AppendChar(key.KeyChar);
                    if (useMask)
                    {
                        Console.Write(mask);
                    }
                }
            }

            Console.WriteLine();

            return true;
        }

        /// <summary>
        /// Compares two instances of <see cref="SecureString"/> by content.
        /// That way, passwords may be compared.
        /// <para>
        /// Attention: Uses 'unsafe' code.
        /// </para>
        /// </summary>
        /// <param name="s1">First Secure-String.</param>
        /// <param name="s2">Secure-String, to compare the first one with.</param>
        /// <returns><c>true</c>, if both <see cref="SecureString"/> objects match in content.</returns>
        public static bool SecureStringEqual(this SecureString s1, SecureString s2)
        {
            if (s1 == null)
            {
                return s2 == null;
            }
            if (s2 == null)
            {
                return false;
            }

            if (s1.Length != s2.Length)
            {
                return false;
            }

            IntPtr bstr1 = IntPtr.Zero;
            IntPtr bstr2 = IntPtr.Zero;

            RuntimeHelpers.PrepareConstrainedRegions();

            try
            {
                bstr1 = Marshal.SecureStringToBSTR(s1);
                bstr2 = Marshal.SecureStringToBSTR(s2);

                unsafe
                {
                    for (Char* ptr1 = (Char*)bstr1.ToPointer(), ptr2 = (Char*)bstr2.ToPointer();
                        *ptr1 != 0 && *ptr2 != 0;
                         ++ptr1, ++ptr2)
                    {
                        if (*ptr1 != *ptr2)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            finally
            {
                if (bstr1 != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(bstr1);
                }

                if (bstr2 != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(bstr2);
                }
            }
        }

        private static string convertToHexString(IEnumerable<byte> bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        private static SecureString createReadOnlySecureStringFromBytes(DisposableBytes decryptedData)
        {
            var secureString = new SecureString();

            var initChars = decryptedData.GetChars(Encoding.Unicode);
            for (int i = 0; i < initChars.Length; i++)
            {
                var c = initChars[i];
                if (c == 0)
                {
                    break;
                }
                secureString.AppendChar(c);
            }

            return secureString.AsReadOnly();
        }

        private static SecureString decryptSecureStringAes(byte[] cipherData, byte[] key, byte[] initVector)
        {
            using (var decryptedBytes = EncryptionHelper.DecrytDataAes(cipherData, key, initVector))
            {
                if (decryptedBytes != null)
                    return createReadOnlySecureStringFromBytes(decryptedBytes);
                else
                    return new SecureString().AsReadOnly();
            }
        }

        private static byte[] encryptSecureStringAes(SecureString plainText, byte[] key, byte[] initVector)
        {
            var textBytes = plainText.GetBytes();
            return EncryptionHelper.EncryptDataAes(textBytes, key, initVector);
        }
    }
}