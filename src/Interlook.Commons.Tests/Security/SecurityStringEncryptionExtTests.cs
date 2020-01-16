using System.Linq;
using System.Security;
using System.Text;
using Xunit;

namespace Interlook.Security.Tests
{
    public class SecurityStringEncryptionExtTests
    {
        private static SecureString _securePass = new SecureString().InitFromCharArray("ThisIsANotSoSecurepasswordnumberseven".ToCharArray());
        private static SecureString Text1 = new SecureString().InitFromCharArray("And thou shall add the Book Of Flavor Flav to the Bible".ToCharArray());
        private static SecureString Text2 = new SecureString().InitFromCharArray("I'm the least you could do, If only life were as easy as you".ToCharArray());

        #region Hash Tests

        [Theory]
        [InlineData("Franz jagt im komplett verwahrlosten Taxi quer durch Bayern", "68ac906495480a3404beee4874ed853a037a7a8f", null)]
        [InlineData("Sylvia wagt quick den Jux bei Pforzheim", "a4117ce6498b48cbd19eaa3127bf4e6e0f2058cc", "cohagapo79")]
        public void GetSHA1Test_UTF8(string original, string expected, string salt)
        {
            string actual = null;
            if (salt == null)
            {
                actual = original.GetSHA1(Encoding.UTF8).ToLower();
            }
            else
            {
                actual = original.GetSHA1(Encoding.UTF8, salt).ToLower();
            }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Franz jagt im komplett verwahrlosten Taxi quer durch Bayern", "ca2bea5813b6914b6d75ee6975af2aa99b7f09ca", null)]
        [InlineData("Sylvia wagt quick den Jux bei Pforzheim", "8cb5880a330b3a8f06460d0c8849108a330d3706", "cohagapo79")]
        public void GetSHA1Test(string original, string expected, string salt)
        {
            string actual = null;
            if (salt == null)
            {
                actual = original.GetSHA1().ToLower();
            }
            else
            {
                actual = original.GetSHA1(salt).ToLower();
            }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Jeder wackere Bayer vertilgt bequem zwo Pfund Kalbshaxen", "fb17d1bbd81ca9a673ac934f2c07c5cbfb53d60cead085c9036439654fc7cb17", null)]
        [InlineData("Stanleys Expeditionszug quer durch Afrika wird von jedermann bewundert", "8765182232b107d08965e2bd5ebe91f4786876d818b9d80025413b485929b5da", "tsa1972QP")]
        public void GetSHA256PositiveTest_UTF8(string original, string expected, string salt)
        {
            string actual = null;
            if (salt == null)
            {
                actual = original.GetSHA256(Encoding.UTF8).ToLower();
            }
            else
            {
                actual = original.GetSHA256(Encoding.UTF8, salt).ToLower();
            }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Jeder wackere Bayer vertilgt bequem zwo Pfund Kalbshaxen", "305bbab9922019d18369618de5577cd30e774bf0fcf90546c3e870abe681213c", null)]
        [InlineData("Stanleys Expeditionszug quer durch Afrika wird von jedermann bewundert", "4a54bac1c784c06129922c1d7cf73365c62da5320ff6b86d53a10ef5f1852f19", "tsa1972QP")]
        public void GetSHA256PositiveTest(string original, string expected, string salt)
        {
            string actual = null;
            if (salt == null)
            {
                actual = original.GetSHA256().ToLower();
            }
            else
            {
                actual = original.GetSHA256(salt).ToLower();
            }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Prall vom Whisky flog Quax den Jet zu Bruch", "502273f640c3594a1a009a02212d8c20b7d98992fb2a88cef0bf95682287b114e9e376b8520108bd4115b1a38b1deb20898d1d11f07fb17013cacd30f6488390", null)]
        [InlineData("Retrofit the pudding hatch With the boink swatter", "fad5769b085dbf75349491f90f00cae4cb8e39a66d225bde45cbfc5370e47ecb6e82874f75e88a967950ca45c71397de85c0ca43feb7dc4e3d5109ad0d70d1f6", "FoxUniCharKi773")]
        public void GetSHA512PositiveTest_UTF8(string original, string expected, string salt)
        {
            string actual = null;
            if (salt == null)
            {
                actual = original.GetSHA512(Encoding.UTF8).ToLower();
            }
            else
            {
                actual = original.GetSHA512(Encoding.UTF8, salt).ToLower();
            }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Prall vom Whisky flog Quax den Jet zu Bruch", "352ccf7e667ceac6ad8850aaa67107d65579ebc00e30f545673087c7ec96c22a0ae4254e11492c00bf1c9a0eca142ae0806339d727f319de1be8fbfd9ef9c3f5", null)]
        [InlineData("Retrofit the pudding hatch With the boink swatter", "67392155d7d92c6ca83e7565013e72a33c39a67ceffd8d1d4c077c5584917c4ae4757de2f350170f57713f0eac8c17b1f43a1e1a0bec2e90b4c8836831d6c013", "FoxUniCharKi773")]
        public void GetSHA512PositiveTest(string original, string expected, string salt)
        {
            string actual = null;
            if (salt == null)
            {
                actual = original.GetSHA512().ToLower();
            }
            else
            {
                actual = original.GetSHA512(salt).ToLower();
            }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Jörg bäckt quasi zwei Haxenfüße vom Wildpony", "d622cf7d50ec25e88aa54d575bfb48d8", null)]
        [InlineData("Pressure wash the quiver bone In the bitch wrinkle", "9ef2b77d0f40e61dd2b52f9ea14d5bd7", "FoxUniCharKi773")]
        public void GetMD5PositiveTest_UTF8(string original, string expected, string salt)
        {
            string actual = null;
            if (salt == null)
            {
                actual = original.GetMD5(Encoding.UTF8).ToLower();
            }
            else
            {
                actual = original.GetMD5(Encoding.UTF8, salt).ToLower();
            }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Jörg bäckt quasi zwei Haxenfüße vom Wildpony", "91950d4814ebf3e55915670af09247a3", null)]
        [InlineData("Pressure wash the quiver bone In the bitch wrinkle", "c5c7d051ad6fc623710a823b8d2b4e76", "FoxUniCharKi773")]
        public void GetMD5PositiveTest(string original, string expected, string salt)
        {
            string actual = null;
            if (salt == null)
            {
                actual = original.GetMD5().ToLower();
            }
            else
            {
                actual = original.GetMD5(salt).ToLower();
            }

            Assert.Equal(expected, actual);
        }

        #endregion Hash Tests

        #region Excryptions

        [Fact]
        public void Machine_Encryption_Test()
        {
            var plainText = Text1;
            var encrypted = plainText.EncryptForMachine();
            var decrypted = encrypted.DecryptSecureStringForMachine();

            Assert.True(plainText.SecureStringEqual(decrypted));
        }

        [Fact]
        public void Machine_EncryptionWithUtf8_Test()
        {
            var plainText = Text1;
            var entropy = EncryptionHelper.DefaultEntropy256.Reverse().ToArray();
            var encrypted = plainText.EncryptForMachine(entropy);
            var decrypted = encrypted.DecryptSecureStringForMachine(entropy);

            Assert.True(plainText.SecureStringEqual(decrypted));
        }

        [Fact]
        public void User_Encryption_Test()
        {
            var plainText = Text1;
            var encrypted = plainText.EncryptForUser();
            var decrypted = encrypted.DecryptSecureStringForUser();

            Assert.True(plainText.SecureStringEqual(decrypted));
        }

        [Fact]
        public void User_EncryptionWithUtf8_Test()
        {
            var plainText = Text1;
            var entropy = EncryptionHelper.DefaultEntropy256.Reverse().ToArray();
            var encrypted = plainText.EncryptForUser(entropy);
            var decrypted = encrypted.DecryptSecureStringForUser(entropy);

            Assert.True(plainText.SecureStringEqual(decrypted));
        }

        [Fact]
        public void Aes_Encryption_Test()
        {
            var plainText = Text1;
            var encrypted = plainText.EncryptWith(_securePass);
            var decrypted = encrypted.DecryptSecureStringWith(_securePass);

            Assert.True(plainText.SecureStringEqual(decrypted));
        }

        [Fact]
        public void Aes_Encryption_InitVectorMatters_Test()
        {
            var plainText = Text1;

            var salt = EncryptionHelper.DefaultSalt256.ToArray();
            var iv = EncryptionHelper.DefaultInitVector128.ToArray();
            var iv2 = EncryptionHelper.DefaultInitVector128.Reverse().ToArray();

            var encrypted = plainText.EncryptWith(_securePass, iv, salt);
            var decrypted = encrypted.DecryptSecureStringWith(_securePass, iv2, salt);

            Assert.False(plainText.SecureStringEqual(decrypted));
        }

        #endregion Excryptions
    }
}