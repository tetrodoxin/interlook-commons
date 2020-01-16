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

using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Cryptography;

namespace Interlook.Security
{
    /// <summary>
    /// Contains helper and extension methods dealing with data encryption.
    /// </summary>
    public static class EncryptionHelper
    {
        /// <summary>
        /// The maximum AES key size in bytes.
        /// </summary>
        public const int MaxAesKeySize = 32;

        /// <summary>
        /// Default 256bit entropy data.
        /// </summary>
        public static readonly byte[] DefaultEntropy256 =
        {
            110, 18, 137, 231, 17, 56, 161, 214, 245, 110, 50,
            29, 194, 71, 176, 224, 158, 20, 9, 151, 108, 40,
            22, 100, 196, 148, 233, 214, 197, 239, 16, 190
        };

        /// <summary>
        /// Default 128bit initialization vector.
        /// </summary>
        public static readonly byte[] DefaultInitVector128 = { 217, 11, 91, 133, 117, 29, 243, 7, 198, 52, 77, 189, 202, 133, 17, 93 };

        /// <summary>
        /// Default 256bit salt data.
        /// </summary>
        public static readonly byte[] DefaultSalt256 =
        {
            79, 0, 26, 168, 175, 187, 225, 14, 105, 73, 78,
            179, 152, 51, 142, 43, 52, 111, 177, 21, 134, 81,
            94, 9, 232, 52, 249, 114, 219, 104, 200, 208
        };

        /// <summary>
        /// Creates the a key from a given passphrase by utilizing RFC2898.
        /// </summary>
        /// <param name="passphrase">The passphrase used to derive the key.</param>
        /// <param name="salt">The salt bytes to be used to derive the key.</param>
        /// <param name="keySize">Size of the key in bytes.</param>
        /// <param name="iterations">(Optional). The iterations of the used KDF to produce the key. Default is 2000.</param>
        /// <returns>A new instance of <see cref="DisposableBytes"/> containing the key data.</returns>
        public static DisposableBytes CreateKeyFromPassphrase(SecureString passphrase, byte[] salt, int keySize, int iterations = 2000)
        {
            byte[] passwordBytes = null;
            try
            {
                passwordBytes = passphrase.GetBytes();

                using (var rfcBytes = new Rfc2898DeriveBytes(passwordBytes, salt, iterations))
                {
                    return new DisposableBytes(rfcBytes.GetBytes(keySize));
                }
            }
            finally
            {
                if (passwordBytes != null)
                {
                    for (int i = 0; i < passwordBytes.Length; i++)
                        passwordBytes[i] = 0;
                }
            }
        }

        /// <summary>
        /// Decrypts the bytes contained in a base64-string into a byte array
        /// using the Windows DP-API and the scope of the local machine
        /// and the default entropy data in <see cref="DefaultEntropy256"/>
        /// </summary>
        /// <param name="data">The cipher text as byte array.</param>
        /// <returns>A <see cref="DisposableBytes"/> object containing the decrypted data.</returns>
        public static DisposableBytes DecryptBytesForMachine(byte[] data)
                            => DecryptBytesForMachine(data, DefaultEntropy256);

        /// <summary>
        /// Decrypts the bytes contained in a base64-string into a byte array
        /// using the Windows DP-API and the scope of the local machine.
        /// </summary>
        /// <param name="data">The cipher text as byte array.</param>
        /// <param name="entropy">A byte array used to increase the complexity of the encryption.
        /// It doesn't need to be secret, but the same for encryption and decryption, just like a salt.</param>
        /// <returns>A <see cref="DisposableBytes"/> object containing the decrypted data.</returns>
        public static DisposableBytes DecryptBytesForMachine(byte[] data, byte[] entropy)
        {
            return new DisposableBytes(ProtectedData.Unprotect(data,
                entropy,
                DataProtectionScope.LocalMachine));
        }

        /// <summary>
        /// Decrypts data using the Windows DP-API and the scope of the current user
        /// and the default entropy data in <see cref="DefaultEntropy256"/>.
        /// </summary>
        /// <param name="cipherData">The cipher text as byte array.</param>
        /// <returns>A <see cref="DisposableBytes"/> object containing the decrypted data.</returns>
        public static DisposableBytes DecryptBytesForUser(byte[] cipherData)
            => DecryptBytesForUser(cipherData, DefaultEntropy256);

        /// <summary>
        /// Decrypts data using the Windows DP-API and the scope of the current user.
        /// </summary>
        /// <param name="cipherData">The cipher text as byte array.</param>
        /// <param name="entropy">A byte array used to increase the complexity of the encryption.
        /// It doesn't need to be secret, but the same for encryption and decryption, just like a salt.</param>
        /// <returns>A <see cref="DisposableBytes"/> object containing the decrypted data.</returns>
        public static DisposableBytes DecryptBytesForUser(byte[] cipherData, byte[] entropy)
        {
            return new DisposableBytes(ProtectedData.Unprotect(
                cipherData,
                entropy,
                DataProtectionScope.CurrentUser));
        }

        /// <summary>
        /// Decrypts data using a certain symmetric algorithm.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <param name="key">The key.</param>
        /// <param name="initVector">The initialize vector.</param>
        /// <param name="cryptoServiceProviderFactory">
        /// A function returning an instance of a crypto provider, to be used for encryption
        /// </param>
        /// <returns>A <see cref="DisposableBytes"/> object containing the decrypted data.</returns>
        public static DisposableBytes DecryptData(byte[] cipherText, byte[] key, byte[] initVector, Func<SymmetricAlgorithm> cryptoServiceProviderFactory)
        {
            using (var cryptoServiceProvider = cryptoServiceProviderFactory())
            {
                return DecryptData(cipherText, key, initVector, cryptoServiceProvider);
            }
        }

        /// <summary>
        /// Decrypts data using a certain symmetric algorithm.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <param name="key">The key.</param>
        /// <param name="initVector">The initialize vector.</param>
        /// <param name="cryptoServiceProvider">The provider to be used for the symmetric encryption.</param>
        /// <returns>A <see cref="DisposableBytes"/> object containing the decrypted data.</returns>
        public static DisposableBytes DecryptData(byte[] cipherText, byte[] key, byte[] initVector, SymmetricAlgorithm cryptoServiceProvider)
        {
            var list = new List<byte>(cipherText.Length);

            cryptoServiceProvider.Key = key;
            cryptoServiceProvider.IV = initVector;
            using (ICryptoTransform decryptor = cryptoServiceProvider.CreateDecryptor(cryptoServiceProvider.Key, cryptoServiceProvider.IV))
            {
                using (var memoryStream = new MemoryStream(cipherText))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        for (int index = cryptoStream.ReadByte(); index >= 0; index = cryptoStream.ReadByte())
                            list.Add((byte)index);
                    }
                }
            }

            var result = list.ToArray();
            for (var i = 0; i < list.Count; i++) list[i] = 0;   // clean temporary list with plaintext
            list.Clear();
            return new DisposableBytes(result);
        }

        /// <summary>
        /// Decrypts data using AES256.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <param name="key">The key.</param>
        /// <param name="initVector">The initialize vector.</param>
        /// <returns>A <see cref="DisposableBytes"/> object containing the decrypted data.</returns>
        public static DisposableBytes DecrytDataAes(byte[] cipherText, byte[] key, byte[] initVector)
            => DecryptData(cipherText, key, initVector, createAesCryptoProvider);

        /// <summary>
        /// Encrypts data using the Windows DP-API and the scope of the local machine
        /// and the default entropy data in <see cref="DefaultEntropy256"/>.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <returns>A byte array containing the encrypted data.</returns>
        public static byte[] EncryptBytesForMachine(byte[] data)
            => EncryptBytesForMachine(data, DefaultEntropy256);

        /// <summary>
        /// Encrypts data using the Windows DP-API and the scope of the local machine.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="entropy">A byte array used to increase the complexity of the encryption.
        /// It doesn't need to be secret, but the same for encryption and decryption, just like a salt.</param>
        /// <returns>A byte array containing the encrypted data.</returns>
        public static byte[] EncryptBytesForMachine(byte[] data, byte[] entropy)
            => ProtectedData.Protect(
                            data,
                            entropy,
                            DataProtectionScope.LocalMachine);

        /// <summary>
        /// Encrypts data using the Windows DP-API and the scope of the current user
        /// and the default entropy data in <see cref="DefaultEntropy256"/>.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <returns>A byte array containing the encrypted data.</returns>
        public static byte[] EncryptBytesForUser(byte[] data)
            => EncryptBytesForUser(data, DefaultEntropy256);

        /// <summary>
        /// Encrypts data using the Windows DP-API and the scope of the current user.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="entropy">A byte array used to increase the complexity of the encryption.
        /// It doesn't need to be secret, but the same for encryption and decryption, just like a salt.</param>
        /// <returns>A byte array containing the encrypted data.</returns>
        public static byte[] EncryptBytesForUser(byte[] data, byte[] entropy)
            => ProtectedData.Protect(data,
                            entropy,
                            DataProtectionScope.CurrentUser);

        /// <summary>
        /// Encrypts data using a certain symmetric algorithm.
        /// </summary>
        /// <param name="plainText">The data to encrypt.</param>
        /// <param name="key">The key used for symmectric encryption.</param>
        /// <param name="initVector">The initialize vector.</param>
        /// <param name="cryptoServiceProviderFactory">
        /// A function returning an instance of a crypto provider, to be used for encryption
        /// </param>
        /// <returns>A byte array containing encrypted data.</returns>
        public static byte[] EncryptData(byte[] plainText, byte[] key, byte[] initVector, Func<SymmetricAlgorithm> cryptoServiceProviderFactory)
        {
            using (var cryptoServiceProvider = cryptoServiceProviderFactory())
            {
                return EncryptData(plainText, key, initVector, cryptoServiceProvider);
            }
        }

        /// <summary>
        /// Encrypts data using a certain symmetric algorithm.
        /// </summary>
        /// <param name="plainText">The data to encrypt.</param>
        /// <param name="key">The key used for symmectric encryption.</param>
        /// <param name="initVector">The initialize vector.</param>
        /// <param name="cryptoServiceProvider">The provider to be used for the symmetric encryption.</param>
        /// <returns>A byte array containing encrypted data.</returns>
        public static byte[] EncryptData(byte[] plainText, byte[] key, byte[] initVector, SymmetricAlgorithm cryptoServiceProvider)
        {
            byte[] numArray;

            cryptoServiceProvider.KeySize = key.Length * 8;
            cryptoServiceProvider.Key = key;
            cryptoServiceProvider.IV = initVector;

            using (ICryptoTransform encryptor = cryptoServiceProvider.CreateEncryptor(cryptoServiceProvider.Key, cryptoServiceProvider.IV))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainText, 0, plainText.Length);
                        cryptoStream.FlushFinalBlock();
                        numArray = memoryStream.ToArray();
                    }
                }
            }

            return numArray;
        }

        /// <summary>
        /// Encrypts data using AES256.
        /// </summary>
        /// <param name="plainText">The data to encrypt.</param>
        /// <param name="key">The key used for symmectric encryption.</param>
        /// <param name="initVector">The initialize vector.</param>
        /// <returns>A byte array containing encrypted data.</returns>
        public static byte[] EncryptDataAes(byte[] plainText, byte[] key, byte[] initVector)
            => EncryptData(plainText, key, initVector, createAesCryptoProvider);

        private static SymmetricAlgorithm createAesCryptoProvider() => Aes.Create();
    }
}