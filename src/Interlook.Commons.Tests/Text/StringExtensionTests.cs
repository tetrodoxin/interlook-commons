using Interlook.Text;
using System;
using Xunit;

namespace Interlook.Text.Tests
{
    public class StringExtensionTests
    {
        #region Tests

        [Fact]
        public void IsNullOrEmptyPositiveTest()
        {
            var s = String.Empty;
            Assert.True(s.IsNullOrEmpty(), "String.Empty did not lead to true for IsNullOrEmpty().");
            s = null;
            Assert.True(s.IsNullOrEmpty(), "NULL string did not lead to true for IsNullOrEmpty().");
        }

        [Fact]
        public void IsNullOrEmptyNegativeTest()
        {
            var s = " ";
            Assert.False(s.IsNullOrEmpty(), "A single whitespace character lead to true for IsNullOrEmpty().");
            s = "not empty";
            Assert.False(s.IsNullOrEmpty(), "the string '" + s + "' lead to true for IsNullOrEmpty().");
        }

        [Fact]
        public void AintNullNorEmptyPositiveTest()
        {
            var s = " ";
            Assert.True(s.AintNullNorEmpty(), "A single whitespace character lead to false for IsNullOrEmpty().");
            s = "not empty";
            Assert.True(s.AintNullNorEmpty(), "the string '" + s + "' lead to false for IsNullOrEmpty().");
        }

        [Fact]
        public void AintNullNorEmptyNegativeTest()
        {
            var s = String.Empty;
            Assert.False(s.AintNullNorEmpty(), "String.Empty lead to true for AintNullNorEmpty().");
            s = null;
            Assert.False(s.AintNullNorEmpty(), "NULL string lead to true for AintNullNorEmpty().");
        }

        [Fact]
        public void IsNumericOnlyPositiveTest()
        {
            var s = "123456";
            Assert.True(s.IsNumericOnly(), "the string '" + s + "' did not lead to true for IsNumericOnly() without additional chars.");
            var additionals = ",.";
            s = "17,348.25";
            Assert.True(s.IsNumericOnly(additionals), "the string '" + s + "' did not lead to true for IsNumericOnly() with additional chars '" + additionals + "'");
        }

        [Fact]
        public void IsNumericOnlyNegativeTest()
        {
            var s = "21a";
            Assert.False(s.IsNumericOnly(), "the string '" + s + "' did not lead to false for IsNumericOnly() without additional chars.");
            s = "21.7";
            Assert.False(s.IsNumericOnly(), "the string '" + s + "' did not lead to false for IsNumericOnly() without additional chars.");
            s = "   21 ";
            Assert.False(s.IsNumericOnly(), "the string '" + s + "' did not lead to false for IsNumericOnly() without additional chars.");
            s = "$17,348.25";
            var additionals = ",.";
            Assert.False(s.IsNumericOnly(additionals), "the string '" + s + "' did not lead to false for IsNumericOnly() with additional chars '" + additionals + "'");
        }

        [Fact]
        public void TrimProtectedTest()
        {
            string s = null;
            string ergebnis = s.TrimProtected();
            Assert.Null(ergebnis);
            Assert.Equal(String.Empty, String.Empty.TrimProtected());
            Assert.Equal(String.Empty, "   ".TrimProtected());
            Assert.Equal("d", "  d ".TrimProtected());
        }

        [Fact]
        public void EnsureTest()
        {
            string s = null;
            string ergebnis = s.Ensure();
            Assert.Equal(String.Empty, ergebnis);
            s = "Zwölf Boxkämpfer jagen Viktor quer über den großen Sylter Deich";
            Assert.Equal(s, s.Ensure());
            s = " ";
            Assert.Equal(s, s.Ensure());
        }

        [Theory]
        [InlineData("alfalfa", "Alfalfa", true)]
        [InlineData("CASPRYXEMon", "CASPRYXEMon", false)]
        [InlineData("CASPRYXEMon", "Caspryxemon", true)]
        public void CapitalizedFirstCharacterTest(string before, string expected, bool lowerRemainder)
        {
            string after = before.CapitalizedFirstCharacter(lowerRemainder);
            Assert.Equal(expected, after);
        }

        [Theory]
        [InlineData("FoxtrottUniformCharlieKilo")]
        [InlineData(" A quick movement of the enemy will jeopardize/vulcanize 6 gun-boats.")]
        public void SecureEqualsPositiveTest(string s)
        {
            var copy = new string(s.ToCharArray());
            Assert.True(s.SecureEquals(copy));
        }

        [Theory]
        [InlineData("In lieu of the innuendo in the end know my intent though", "In lieu of the innuendo in the end know my intent though ")]
        [InlineData(" Flatulla", "Flatulla")]
        [InlineData(" Cannonball", "cannonball")]
        [InlineData("Ett Ülptentülp örbst", "Ett Ülptentulp orbst")]
        [InlineData("Gazètte", "Gazette")]
        public void SecureEqualsNegativeTest(string s, string match)
        {
            Assert.False(s.SecureEquals(match));
        }

        #endregion Tests
    }
}