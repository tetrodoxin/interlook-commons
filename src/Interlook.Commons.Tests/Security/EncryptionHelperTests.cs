using System.Security;
using System.Text;
using Xunit;

namespace Interlook.Security.Tests
{
    public class EncryptionHelperTests
    {
        private static SecureString _securePass = new SecureString().InitFromCharArray("ThisIsANotSoSecurepasswordnumberseven".ToCharArray());
        private const string Text1 = "And thou shall add the Book Of Flavor Flav to the Bible";
        private const string Text2 = "I'm the least you could do, If only life were as easy as you";

        public static readonly byte[] Entropy =
{
            29, 194, 71, 176, 224, 158, 20, 9, 151, 108, 40,
            110, 18, 137, 231, 17, 56, 161, 214, 245, 110, 50,
            22, 214, 196, 248, 23, 14, 197, 239, 17, 190
        };

        #region CreateKeyFromPassphrase() tests

        [Theory]
        [InlineData(16)]
        [InlineData(24)]
        [InlineData(32)]
        public void CreateKeyFromPassphrase_KeySize_Test(int keySize)
        {
            var key = EncryptionHelper.CreateKeyFromPassphrase(_securePass, EncryptionHelper.DefaultSalt256, keySize);
            Assert.Equal(keySize, key.Length);
        }

        [Fact]
        public void CreateKeyFromPassphrase_DifferentSaltDifferentKeys_Test()
        {
            var salt1 = EncryptionHelper.DefaultSalt256;
            var salt2 = EncryptionHelper.DefaultEntropy256;

            var key1 = EncryptionHelper.CreateKeyFromPassphrase(_securePass, salt1, EncryptionHelper.MaxAesKeySize);
            var key2 = EncryptionHelper.CreateKeyFromPassphrase(_securePass, salt2, EncryptionHelper.MaxAesKeySize);

            Assert.NotEqual<byte[]>(key1, key2);
        }

        [Theory]
        [InlineData(2000, 2001)]
        [InlineData(100, 1000)]
        [InlineData(2000, 5000)]
        public void CreateKeyFromPassphrase_DifferentIterationsDifferentKeys_Test(int iterations, int iterations2)
        {
            var key1 = EncryptionHelper.CreateKeyFromPassphrase(_securePass, EncryptionHelper.DefaultSalt256, EncryptionHelper.MaxAesKeySize, iterations);
            var key2 = EncryptionHelper.CreateKeyFromPassphrase(_securePass, EncryptionHelper.DefaultSalt256, EncryptionHelper.MaxAesKeySize, iterations2);

            Assert.NotEqual<byte[]>(key1, key2);
        }

        [Theory]
        [InlineData("WeWillJustSwitchOneOfThisCharactersAfterwards", "WeWillJustSwitchOneOfThisCharactersAfterward5")]
        [InlineData("JustTheBeginningOfAPasswordMayChange", "IustTheBeginningOfAPasswordMayChange")]
        [InlineData("AndEvenPasswordsOrPassphrasesLongAsHellShallNotResultInSameKeysIfTheyJustDifferInOnlyOneDigitAtAll", "AndEvenPasswordsOrPassphrasesLongAsHellShallNotResultInSameKeysIfTheyJustDifferInOnlyOneDigitAtAl1")]
        public void CreateKeyFromPassphrase_DifferentPasswordsDifferentKeys_Test(string pass1, string pass2)
        {
            var p1 = new SecureString().InitFromCharArray(pass1.ToCharArray());
            var p2 = new SecureString().InitFromCharArray(pass2.ToCharArray());

            var key1 = EncryptionHelper.CreateKeyFromPassphrase(p1, EncryptionHelper.DefaultSalt256, EncryptionHelper.MaxAesKeySize);
            var key2 = EncryptionHelper.CreateKeyFromPassphrase(p2, EncryptionHelper.DefaultSalt256, EncryptionHelper.MaxAesKeySize);

            Assert.NotEqual<byte[]>(key1, key2);
        }

        #endregion CreateKeyFromPassphrase() tests

        #region DP-API encryption

        [Fact]
        public void MachineEncryption_Test()
        {
            var plainText = Text1;
            var plainBytes = Encoding.Unicode.GetBytes(plainText);

            var encryptedData = EncryptionHelper.EncryptBytesForMachine(plainBytes);
            var decrypted = EncryptionHelper.DecryptBytesForMachine(encryptedData);

            var decryptedText = Encoding.Unicode.GetString(decrypted.GetContent());

            Assert.Equal(plainText, decryptedText);
        }

        [Fact]
        public void MachineEncryption_Entropy_Test()
        {
            var encoding = Encoding.Unicode;
            var plainText = Text1;
            var plainBytes = encoding.GetBytes(plainText);
            var entropy = Entropy;

            var encryptedData = EncryptionHelper.EncryptBytesForMachine(plainBytes, entropy);
            var decrypted = EncryptionHelper.DecryptBytesForMachine(encryptedData, entropy);

            var decryptedText = encoding.GetString(decrypted.GetContent());

            Assert.Equal(plainText, decryptedText);
        }

        [Fact]
        public void UserEncryption_Test()
        {
            var plainText = Text1;
            var plainBytes = Encoding.Unicode.GetBytes(plainText);

            var encryptedData = EncryptionHelper.EncryptBytesForUser(plainBytes);
            var decrypted = EncryptionHelper.DecryptBytesForUser(encryptedData);

            var decryptedText = Encoding.Unicode.GetString(decrypted.GetContent());

            Assert.Equal(plainText, decryptedText);
        }

        [Fact]
        public void UserEncryption_Entropy_Test()
        {
            var encoding = Encoding.Unicode;
            var plainText = Text1;
            var plainBytes = encoding.GetBytes(plainText);
            var entropy = Entropy;

            var encryptedData = EncryptionHelper.EncryptBytesForUser(plainBytes, entropy);
            var decrypted = EncryptionHelper.DecryptBytesForUser(encryptedData, entropy);

            var decryptedText = encoding.GetString(decrypted.GetContent());

            Assert.Equal(plainText, decryptedText);
        }

        #endregion DP-API encryption

        #region AES encryption

        [Fact]
        public void EncryptDataAes_Test()
        {
            var plainText = Text1;
            var encoding = Encoding.Unicode;
            var plainBytes = encoding.GetBytes(plainText);
            var key = new byte[] 
            { 55, 242, 108, 96, 95, 185, 75, 107, 30, 25, 178, 80, 
                116, 247, 48, 157, 54, 91, 246, 191, 85, 10, 141, 
                144, 243, 213, 243, 253, 163, 186, 130, 1 };

            var encryptedData = EncryptionHelper.EncryptDataAes(plainBytes, key, EncryptionHelper.DefaultInitVector128);
            var decryptedData = EncryptionHelper.DecrytDataAes(encryptedData, key, EncryptionHelper.DefaultInitVector128);

            Assert.Equal(plainBytes, decryptedData);

        }

        [Fact]
        public void EncryptDataAes_NotPlain_Test()
        {
            var plainText = Text1;
            var encoding = Encoding.Unicode;
            var plainBytes = encoding.GetBytes(plainText);
            var key = new byte[]
            { 55, 242, 108, 96, 95, 185, 75, 107, 30, 25, 178, 80,
                116, 247, 48, 157, 54, 91, 246, 191, 85, 10, 141,
                144, 243, 213, 243, 253, 163, 186, 130, 1 };

            var encryptedData = EncryptionHelper.EncryptDataAes(plainBytes, key, EncryptionHelper.DefaultInitVector128);

            Assert.NotEqual(plainBytes, encryptedData);

        }

        #endregion
    }
}