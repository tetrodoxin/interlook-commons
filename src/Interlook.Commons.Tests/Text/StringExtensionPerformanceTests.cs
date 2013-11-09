using System;
using Interlook.Text;
using Xunit;
using Xunit.Extensions;

namespace Interlook.Text.Tests
{
	public class StringExtensionPerformanceTests
	{
		public static string[] texts = new string[]
		{
		Interlook.Tests.Properties.Resources.Long,
		Interlook.Tests.Properties.Resources.Half,
		Interlook.Tests.Properties.Resources.Short,
		Interlook.Tests.Properties.Resources.HalfDotted,
		Interlook.Tests.Properties.Resources.ShortDotted
		};

		#region Tests

		[Theory]
		[InlineData(0, 1)]
		[InlineData(0, 2)]
		public void SecureEqualsTimingTest(int original, int candidate)
		{
			var eins = texts[original].ToCharArray();
			var exact = texts[original].ToCharArray();
			var match = texts[candidate].ToCharArray();

			var origDauer = measureTime(match, match);
			var matchDauer = measureTime(eins, match);

			Assert.True(Math.Abs(origDauer - matchDauer) <= (origDauer + matchDauer) / 60, "orig=" + origDauer.ToString() + "; match=" + matchDauer.ToString());
		}

		private long measureTime(char[] orig, char[] candidate)
		{
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			var z = orig.SecureEquals(candidate);

			for (int i = 0; i < 1500000; i++)
				z = orig.SecureEquals(candidate);

			sw.Stop();
			if (z)
			{
				z = false;
			}

			return sw.ElapsedMilliseconds;
		}

		#endregion Tests
	}
}