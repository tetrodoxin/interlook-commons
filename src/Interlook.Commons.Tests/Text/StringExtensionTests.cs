using System;
using Interlook.Text;
using Xunit;
using Xunit.Extensions;

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
			string ergebnis = null;
			Assert.DoesNotThrow(() => ergebnis = s.TrimProtected());
			Assert.Null(ergebnis);
			Assert.Equal(String.Empty, String.Empty.TrimProtected());
			Assert.Equal(String.Empty, "   ".TrimProtected());
			Assert.Equal("d", "  d ".TrimProtected());
		}

		[Fact]
		public void EnsureTest()
		{
			string s = null;
			string ergebnis = null;
			Assert.DoesNotThrow(() => ergebnis = s.Ensure());
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

		public void NormalizeLatinCharsPositiveTest()
		{
		}

		[Theory]
		[InlineData("FoxtrottUniformCharlieKilo")]
		[InlineData(" A quick movement of the enemy will jeopardize/vulcanize 6 gun-boats.")]
		public void SecureEqualsPositiveTest(string s)
		{
			var c = String.Copy(s);
			Assert.True(s.SecureEquals(c));
		}

		[Theory]
		[InlineData("In lieu of the innuendo in the end know my intent though", "In lieu of the innuendo in the end know my intent though ")]
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