using Interlook.Security;
using Xunit;

namespace Interlook.Security.Tests
{
    public class StringEncryptionExtTests
    {
        #region Tests

        [Theory]
        [InlineData("Franz jagt im komplett verwahrlosten Taxi quer durch Bayern", "68ac906495480a3404beee4874ed853a037a7a8f", null)]
        [InlineData("Sylvia wagt quick den Jux bei Pforzheim", "a4117ce6498b48cbd19eaa3127bf4e6e0f2058cc", "cohagapo79")]
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

        #endregion Tests
    }
}