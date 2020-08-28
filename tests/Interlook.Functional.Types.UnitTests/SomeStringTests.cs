using FluentAssertions;
using Interlook.Monads;
using System;
using System.Linq;
using Xunit;

namespace Interlook.Functional.Types.UnitTests
{
    public class SomeStringTests
    {
        private const string TestString = "The Test Value with\teverything.10";
        private const int TestStringParticleLength = 3;
        private static readonly char[] TestStringChars = TestString.ToCharArray();

        private Random _rnd = new Random(DateTime.Now.Millisecond);
        public SomeString SuT { get; }

        public SomeStringTests()
        {
            SuT = new SomeString(TestString);
        }

        [Fact]
        public void Contains_EmptyStringInstance()
        {
            var result = SuT.Contains(EmptyString.Default);
            result.Should().BeTrue("a SomeString instance always contains an EmptyString instance");
        }

        [Fact()]
        public void Contains_SomeCharacter()
        {
            var result = SuT.Contains(TestString[1]);
            result.Should().BeTrue("a SomeString instance always contains its second character");

            result = SuT.Contains(TestString[1], StringComparison.InvariantCulture);
            result.Should().BeTrue("a SomeString instance always contains its second character, regardless of the string comparison");

            result = SuT.Contains(TestString[1], StringComparison.CurrentCulture);
            result.Should().BeTrue("a SomeString instance always contains its second character, regardless of the string comparison");

            result = SuT.Contains(TestString[1], StringComparison.OrdinalIgnoreCase);
            result.Should().BeTrue("a SomeString instance always contains its second character, regardless of the string comparison");
        }

        [Theory()]
        [InlineData(StringComparison.OrdinalIgnoreCase)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase)]
        public void Contains_SomeCharacter_WithCaseIgnoringStringComparison(StringComparison comparison)
        {
            string currentString = "UPCASESTRING";
            var currentSut = new SomeString(currentString);

            var result = currentSut.Contains(currentString[0], comparison);
            result.Should().BeTrue("an upcase SomeString instance always contains its upcase first character, when using a case-ignoring StringComparison");

            result = currentSut.Contains(char.ToLower(currentString[0]), comparison);
            result.Should().BeTrue("an upcase SomeString instance always contains its lowcase first character, when using a case-ignoring StringComparison");

            currentString = "lowcase";
            currentSut = new SomeString(currentString);

            result = currentSut.Contains(currentString[0], comparison);
            result.Should().BeTrue("a lowcase SomeString instance always contains its lowcase first character, when using a case-ignoring StringComparison");

            result = currentSut.Contains(char.ToUpper(currentString[0]), comparison);
            result.Should().BeTrue("a lowcase SomeString instance always contains its upcase first character, when using a case-ignoring StringComparison");
        }

        [Theory()]
        [InlineData(StringComparison.Ordinal)]
        [InlineData(StringComparison.CurrentCulture)]
        [InlineData(StringComparison.InvariantCulture)]
        public void Contains_SomeCharacter_WithCaseSensitiveStringComparison(StringComparison comparison)
        {
            string currentString = "UPCASESTRING";
            var currentSut = new SomeString(currentString);

            var result = currentSut.Contains(currentString[0], comparison);
            result.Should().BeTrue("an upcase SomeString instance always contains its upcase first character, when using a case-sensitive StringComparison");

            result = currentSut.Contains(char.ToLower(currentString[0]), comparison);
            result.Should().BeFalse("an upcase SomeString instance never contains its lowcase first character, when using a case-sensitive StringComparison");

            currentString = "lowcase";
            currentSut = new SomeString(currentString);

            result = currentSut.Contains(currentString[0], comparison);
            result.Should().BeTrue("a lowcase SomeString instance always contains its lowcase first character, when using a case-sensitive StringComparison");

            result = currentSut.Contains(char.ToUpper(currentString[0]), comparison);
            result.Should().BeFalse("a lowcase SomeString instance never contains its upcase first character, when using a case-sensitive StringComparison");
        }

        [Fact()]
        public void Contains_SomeCharacter_WithoutStringComparison()
        {
            string currentString = "UPCASESTRING";
            var currentSut = new SomeString(currentString);

            var result = currentSut.Contains(currentString[0]);
            result.Should().BeTrue("an upcase SomeString instance always contains its upcase first character");

            result = currentSut.Contains(char.ToLower(currentString[0]));
            result.Should().BeFalse("an upcase SomeString instance never contains its lowcase first character");

            currentString = "lowcase";
            currentSut = new SomeString(currentString);

            result = currentSut.Contains(currentString[0]);
            result.Should().BeTrue("a lowcase SomeString instance always contains its lowcase first character");

            result = currentSut.Contains(char.ToUpper(currentString[0]));
            result.Should().BeFalse("a lowcase SomeString instance never contains its upcase first character");
        }

        [Fact()]
        public void Contains_String_EmptyString()
        {
            var result = SuT.Contains(string.Empty);

            result.Should().BeTrue("a SomeString instance always contains an empty string");
        }

        [Fact()]
        public void Contains_String_Null_Negative()
        {
            var result = SuT.Contains(null);

            result.Should().BeFalse("a SomeString instance never contains <null>");
        }

        [Fact()]
        public void Contains_String_ReversedSubstring_Negative()
        {
            var partString = new string(TestString.Substring(TestString.Length / 2).Reverse().ToArray());
            var result = SuT.Contains(partString);

            result.Should().BeFalse("a SomeString instance never contains a substring, that has been reversed");
        }

        [Fact()]
        public void Contains_String_Substring()
        {
            var partString = TestString.Substring(TestString.Length / 2);
            var result = SuT.Contains(partString);

            result.Should().BeTrue("a SomeString instance always contains a substring of itself");
        }

        [Fact()]
        public void Contains_WrongCharacter_Negative()
        {
            char testChar = '\0';
            while (true)
            {
                testChar = (char)_rnd.Next(32, 128);
                if (!TestString.Contains(char.ToUpper(testChar)) && !TestString.Contains(char.ToLower(testChar))) break;
            }

            var result = SuT.Contains(testChar);
            result.Should().BeFalse($"the SomeString '{SuT.Value}' does not contain character '{testChar}'");

            result = SuT.Contains(testChar, StringComparison.InvariantCulture);
            result.Should().BeFalse($"the SomeString '{SuT.Value}' does not contain character '{testChar}', regardless of the string comparison");

            result = SuT.Contains(testChar, StringComparison.CurrentCulture);
            result.Should().BeFalse($"the SomeString '{SuT.Value}' does not contain character '{testChar}', regardless of the string comparison");

            result = SuT.Contains(testChar, StringComparison.OrdinalIgnoreCase);
            result.Should().BeFalse($"the SomeString '{SuT.Value}' does not contain character '{testChar}', regardless of the string comparison");
        }

        [Fact]
        public void Create_FromStringEmpty_Negative()
        {
            var result = SomeString.Create(string.Empty);
            result.Should().BeOfType<Left<Exception, SomeString>>("a SomeString instance can never be created from an empty string");
        }

        [Fact]
        public void Create_FromStringNull_Negative()
        {
            var result = SomeString.Create(null);
            result.Should().BeOfType<Left<Exception, SomeString>>("a SomeString instance can never be created from <null>");
        }

        [Fact]
        public void Create_FromStringWithCharacters()
        {
            var result = SomeString.Create(TestString);
            result.Should().BeOfType<Right<Exception, SomeString>>("SomeString.Create must succeed when called with a string containing actual characters");
        }

        [Fact]
        public void Create_FromStringWithWhitespaces_Negative()
        {
            var result = SomeString.Create("  \t");
            result.Should().BeOfType<Left<Exception, SomeString>>("a SomeString instance can never be created from a whitespace string");
        }

        [Fact]
        public void EndsWith_CorrectCharacter()
        {
            char testChar = TestString[TestString.Length - 1];
            var result = SuT.EndsWith(testChar);
            result.Should().BeTrue($"the SomeString '{SuT.Value}' ends with the character '{testChar}' after all");
        }

        [Fact]
        public void EndsWith_CorrectSubstring()
        {
            string prefix = TestString.Substring(TestString.Length - 6, 5);
            var result = SuT.EndsWith(TestString);
            result.Should().BeTrue($"the SomeString '{SuT.Value}' indeed begins with the '{prefix}'");
        }

        [Fact]
        public void EndsWith_EmptyString()
        {
            var result = SuT.EndsWith(EmptyString.Default);
            result.Should().BeTrue("a SomeString instance always ends with an EmptyString instance.");
        }

        [Fact]
        public void EndsWith_Null()
        {
            var result = SuT.EndsWith(null);
            result.Should().BeFalse("a SomeString instance never ends with <null> (actually, nothing does, well... <null> maybe...).");
        }

        [Fact]
        public void EndsWith_StringEmpty()
        {
            var result = SuT.EndsWith(string.Empty);
            result.Should().BeTrue("a SomeString instance always ends with an empty string.");
        }

        [Fact]
        public void EndsWith_WrongSubstring()
        {
            string wrongSuffix = new string(TestString.Substring(TestString.Length - 6, 5).Reverse().ToArray());
            var result = SuT.EndsWith(wrongSuffix);
            result.Should().BeFalse($"the SomeString '{SuT.Value}' does not start with '{wrongSuffix}'");
        }

        [Fact]
        public void Equals_Object_EmptyString_Negative()
        {
            var result = SuT.Equals((object)EmptyString.Default);
            result.Should().BeFalse("a SomeString instance never equals an EmptyString instance");
        }

        [Fact]
        public void Equals_Object_EqualString()
        {
            var result = SuT.Equals((object)TestString);
            result.Should().BeTrue("a SomeString instance always equals the identical string as object");
        }

        [Fact]
        public void Equals_Object_Null_Negative()
        {
            var result = SuT.Equals((object)null);
            result.Should().BeFalse("a SomeString instance never equals (object)<null>");
        }

        [Fact]
        public void Equals_Object_StringEmpty_Negative()
        {
            var result = SuT.Equals((object)string.Empty);
            result.Should().BeFalse("a SomeString instance never equals an empty string, even as object");
        }

        [Fact]
        public void Equals_String_Empty_Negative()
        {
            var result = SuT.Equals(string.Empty);
            result.Should().BeFalse("a SomeString instance never equals an empty string");
        }

        [Fact]
        public void Equals_String_EqualString()
        {
            var result = SuT.Equals(TestString);
            result.Should().BeTrue("a SomeString instance always equals the identical string as string object");
        }

        [Fact]
        public void Equals_String_Null_Negative()
        {
            var result = SuT.Equals((string)null);
            result.Should().BeFalse("a SomeString instance never equals (string)<null>");
        }

        [Fact]
        public void IndexOf1_StringEmpty()
        {
            var result = SuT.IndexOf(string.Empty);
            result.Should().Be(0, "a SomeString instance always begins with an empty string, so its position must be 0.");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IndexOf1_String(bool expectedTrue)
        {
            var particle = getTestStringParticle(expectedTrue ? 5 : -1);
            var result = SuT.IndexOf(particle.String);

            result.Should().Be(particle.Position, $"the SomeString '{SuT.Value}' " + (expectedTrue
                ? $"contains the substring '{particle.String}' at position {particle.Position}"
                : $"doesn't contain the substring '{particle.String}' at all"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        public void IndexOf2_Empty(int startIndex)
        {
            var result = SuT.IndexOf(string.Empty, startIndex);
            result.Should().Be(startIndex, $"a SomeString instance always contains empty strings everywhere, including position {startIndex}.");
        }

        [Theory]
        [InlineData(true, 5, 0)]
        [InlineData(true, 5, 5)]
        [InlineData(false, 5, 6)]
        [InlineData(false, -1, 0)]
        [InlineData(false, -1, 5)]
        public void IndexOf2_String(bool expectedTrue, int index, int startIndex)
        {
            var particle = getTestStringParticle(index);
            var result = SuT.IndexOf(particle.String, startIndex);

            int expected = expectedTrue ? index : -1;
            result.Should().Be(expected, $"the SomeString '{SuT.Value}' " + (expectedTrue
                ? $"contains the substring '{particle.String}' at position {particle.Position}"
                : $"doesn't contain the substring '{particle.String}' {(index < 0 ? $"when starting search at position {startIndex}" : "at all")}"));
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 5)]
        [InlineData(3, 0)]
        [InlineData(3, 1)]
        [InlineData(4, 5)]
        public void IndexOf3_Empty(int startIndex, int count)
        {
            var result = SuT.IndexOf(string.Empty, startIndex, count);
            result.Should().Be(startIndex, $"a SomeString instance always contains empty strings everywhere, including position {startIndex} and count {count}.");
        }

        [Theory]
        [InlineData(false, 5, 0, 5)]
        [InlineData(true, 5, 0, 5 + TestStringParticleLength)]
        [InlineData(true, 5, 0, 10)]
        [InlineData(false, 5, 5, 1)]
        [InlineData(true, 5, 5, TestStringParticleLength)]
        [InlineData(false, 5, 6, 3)]
        [InlineData(false, 5, 0, 4)]
        [InlineData(false, -1, 0, 5)]
        [InlineData(false, -1, 5, 5)]
        public void IndexOf3_String(bool expectedTrue, int index, int startIndex, int count)
        {
            var particle = getTestStringParticle(index);
            var result = SuT.IndexOf(particle.String, startIndex, count);

            int expected = expectedTrue ? index : -1;
            result.Should().Be(expected, $"the SomeString '{SuT.Value}' " + (expectedTrue
                ? $"contains the substring '{particle.String}' at position {particle.Position}"
                : $"doesn't contain the substring '{particle.String}' {(index < 0 ? $"when using startindex {startIndex} and count {count}" : "at all")}"));
        }

        [Theory]
        [InlineData(StringComparison.OrdinalIgnoreCase)]
        [InlineData(StringComparison.Ordinal)]
        [InlineData(StringComparison.CurrentCulture)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(StringComparison.InvariantCulture)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase)]
        public void IndexOf4_Empty(StringComparison comparer)
        {
            var result = SuT.IndexOf(string.Empty, comparer);
            result.Should().Be(0, $"a SomeString instance always begins with an empty string, so its position must be 0, even with comparisonType {comparer}.");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IndexOf4_String(bool expectedTrue)
        {
            var index = expectedTrue ? 5 : -1;
            doTest_IndexOf4(expectedTrue, index, StringComparison.OrdinalIgnoreCase, false);
            doTest_IndexOf4(expectedTrue, index, StringComparison.CurrentCultureIgnoreCase, false);
            doTest_IndexOf4(expectedTrue, index, StringComparison.InvariantCultureIgnoreCase, false);
            doTest_IndexOf4(expectedTrue, index, StringComparison.Ordinal, false);
            doTest_IndexOf4(expectedTrue, index, StringComparison.CurrentCulture, false);
            doTest_IndexOf4(expectedTrue, index, StringComparison.InvariantCulture, false);

            doTest_IndexOf4(expectedTrue, index, StringComparison.OrdinalIgnoreCase, true);
            doTest_IndexOf4(expectedTrue, index, StringComparison.CurrentCultureIgnoreCase, true);
            doTest_IndexOf4(expectedTrue, index, StringComparison.InvariantCultureIgnoreCase, true);
            doTest_IndexOf4(false, index, StringComparison.Ordinal, true);
            doTest_IndexOf4(false, index, StringComparison.CurrentCulture, true);
            doTest_IndexOf4(false, index, StringComparison.InvariantCulture, true);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        public void IndexOf5_Empty(int startIndex)
        {
            do_IndexOf5_Empty(startIndex, StringComparison.OrdinalIgnoreCase);
            do_IndexOf5_Empty(startIndex, StringComparison.CurrentCultureIgnoreCase);
            do_IndexOf5_Empty(startIndex, StringComparison.InvariantCultureIgnoreCase);
            do_IndexOf5_Empty(startIndex, StringComparison.Ordinal);
            do_IndexOf5_Empty(startIndex, StringComparison.CurrentCulture);
            do_IndexOf5_Empty(startIndex, StringComparison.InvariantCulture);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 5)]
        [InlineData(3, 0)]
        [InlineData(3, 1)]
        [InlineData(4, 5)]
        public void IndexOf6_Empty(int startIndex, int count)
        {
            do_IndexOf6_Empty(startIndex, count, StringComparison.OrdinalIgnoreCase);
            do_IndexOf6_Empty(startIndex, count, StringComparison.CurrentCultureIgnoreCase);
            do_IndexOf6_Empty(startIndex, count, StringComparison.InvariantCultureIgnoreCase);
            do_IndexOf6_Empty(startIndex, count, StringComparison.Ordinal);
            do_IndexOf6_Empty(startIndex, count, StringComparison.CurrentCulture);
            do_IndexOf6_Empty(startIndex, count, StringComparison.InvariantCulture);
        }

        private void do_IndexOf5_Empty(int startIndex, StringComparison comparer)
        {
            var result = SuT.IndexOf(string.Empty, startIndex, comparer);
            result.Should().Be(startIndex, $"a SomeString instance always contains empty strings everywhere, including position {startIndex}, even with comparisonType {comparer}.");
        }

        private void do_IndexOf6_Empty(int startIndex, int count, StringComparison comparer)
        {
            var result = SuT.IndexOf(string.Empty, startIndex, count, comparer);
            result.Should().Be(startIndex, $"a SomeString instance always contains empty strings everywhere, including position {startIndex}, even with count {count} and comparisonType {comparer}.");
        }

        [Theory]
        [InlineData(true, 5, 0)]
        [InlineData(true, 5, 5)]
        [InlineData(false, 5, 6)]
        [InlineData(false, -1, 0)]
        [InlineData(false, -1, 5)]
        public void IndexOf5_String(bool expectedTrue, int index, int startIndex)
        {
            doTest_IndexOf5(expectedTrue, index, startIndex, StringComparison.OrdinalIgnoreCase, false);
            doTest_IndexOf5(expectedTrue, index, startIndex, StringComparison.CurrentCultureIgnoreCase, false);
            doTest_IndexOf5(expectedTrue, index, startIndex, StringComparison.InvariantCultureIgnoreCase, false);
            doTest_IndexOf5(expectedTrue, index, startIndex, StringComparison.Ordinal, false);
            doTest_IndexOf5(expectedTrue, index, startIndex, StringComparison.CurrentCulture, false);
            doTest_IndexOf5(expectedTrue, index, startIndex, StringComparison.InvariantCulture, false);

            doTest_IndexOf5(expectedTrue, index, startIndex, StringComparison.OrdinalIgnoreCase, true);
            doTest_IndexOf5(expectedTrue, index, startIndex, StringComparison.CurrentCultureIgnoreCase, true);
            doTest_IndexOf5(expectedTrue, index, startIndex, StringComparison.InvariantCultureIgnoreCase, true);
            doTest_IndexOf5(false, index, startIndex, StringComparison.Ordinal, true);
            doTest_IndexOf5(false, index, startIndex, StringComparison.CurrentCulture, true);
            doTest_IndexOf5(false, index, startIndex, StringComparison.InvariantCulture, true);
        }

        [Theory]
        [InlineData(false, 5, 0, 5)]
        [InlineData(true, 5, 0, 5 + TestStringParticleLength)]
        [InlineData(true, 5, 0, 10)]
        [InlineData(true, 5, 5, TestStringParticleLength)]
        [InlineData(false, 5, 5, 1)]
        [InlineData(false, 5, 6, 3)]
        [InlineData(false, 5, 0, 4)]
        [InlineData(false, -1, 0, 5)]
        [InlineData(false, -1, 5, 5)]
        public void IndexOf6_String(bool expectedTrue, int index, int startIndex, int count)
        {
            doTest_IndexOf6(expectedTrue, index, startIndex, count, StringComparison.OrdinalIgnoreCase, false);
            doTest_IndexOf6(expectedTrue, index, startIndex, count, StringComparison.CurrentCultureIgnoreCase, false);
            doTest_IndexOf6(expectedTrue, index, startIndex, count, StringComparison.InvariantCultureIgnoreCase, false);
            doTest_IndexOf6(expectedTrue, index, startIndex, count, StringComparison.Ordinal, false);
            doTest_IndexOf6(expectedTrue, index, startIndex, count, StringComparison.CurrentCulture, false);
            doTest_IndexOf6(expectedTrue, index, startIndex, count, StringComparison.InvariantCulture, false);

            doTest_IndexOf6(expectedTrue, index, startIndex, count, StringComparison.OrdinalIgnoreCase, true);
            doTest_IndexOf6(expectedTrue, index, startIndex, count, StringComparison.CurrentCultureIgnoreCase, true);
            doTest_IndexOf6(expectedTrue, index, startIndex, count, StringComparison.InvariantCultureIgnoreCase, true);
            doTest_IndexOf6(false, index, startIndex, count, StringComparison.Ordinal, true);
            doTest_IndexOf6(false, index, startIndex, count, StringComparison.CurrentCulture, true);
            doTest_IndexOf6(false, index, startIndex, count, StringComparison.InvariantCulture, true);
        }

        [Fact]
        public void StartsWith_CorrectCharacter()
        {
            char testChar = TestString[0];
            var result = SuT.StartsWith(testChar);
            result.Should().BeTrue($"the SomeString '{SuT.Value}' really begins with the character '{testChar}'");
        }

        [Fact]
        public void StartsWith_CorrectSubstring()
        {
            string prefix = TestString.Substring(0, 5);
            var result = SuT.StartsWith(TestString);
            result.Should().BeTrue($"the SomeString '{SuT.Value}' indeed begins with the '{prefix}'");
        }

        [Fact]
        public void StartsWith_EmptyString()
        {
            var result = SuT.StartsWith(EmptyString.Default);
            result.Should().BeTrue("a SomeString instance always starts with an EmptyString instance.");
        }

        [Fact]
        public void StartsWith_Null()
        {
            var result = SuT.StartsWith(null);
            result.Should().BeFalse("a SomeString instance never starts with <null> (as it never ends with one, either).");
        }

        [Fact]
        public void StartsWith_StringEmpty()
        {
            var result = SuT.StartsWith(string.Empty);
            result.Should().BeTrue("a SomeString instance always starts with an empty string.");
        }

        [Fact]
        public void StartsWith_WrongCharacter_Negative()
        {
            char testChar = (char)(TestString[0] + 1);
            var result = SuT.StartsWith(testChar);
            result.Should().BeFalse($"the SomeString '{SuT.Value}' doesn't begin with the character '{testChar}'");
        }

        [Fact]
        public void StartsWith_WrongSubstring()
        {
            string wrongPrefix = new string(TestString.Substring(0, 5).Reverse().ToArray());
            var result = SuT.StartsWith(wrongPrefix);
            result.Should().BeFalse($"the SomeString '{SuT.Value}' does not start with '{wrongPrefix}'");
        }

        [Fact]
        public void ToString_IsIdentical()
        {
            var result = SuT.ToString();
            result.Should().Be(TestString);
        }

        private static (int Position, string String) getTestStringParticle(int index)
        {
            return index < 0
                ? (Position: -1, String: "FAKEPARTICLE")
                : (Position: index, String: TestString.Substring(index, TestStringParticleLength));
        }

        private void doTest_IndexOf4(bool expectedTrue, int index, StringComparison comparer, bool changeCase)
        {
            var particle = getTestStringParticle(index);
            if (changeCase)
                particle = (particle.Position, String: particle.String.ToUpper());

            var result = SuT.IndexOf(particle.String, comparer);

            int expected = expectedTrue ? index : -1;
            result.Should().Be(expected, $"the SomeString '{SuT.Value}' " + (expectedTrue
                ? $"contains the substring '{particle.String}' at position {particle.Position}"
                : $"doesn't contain the substring '{particle.String}' {(index < 0 ? $"when using comparer {comparer}" : "at all")}"));
        }

        private void doTest_IndexOf5(bool expectedTrue, int index, int startIndex, StringComparison comparer, bool changeCase)
        {
            var particle = getTestStringParticle(index);
            if (changeCase)
                particle = (particle.Position, String: particle.String.ToUpper());

            var result = SuT.IndexOf(particle.String, startIndex, comparer);

            int expected = expectedTrue ? index : -1;
            result.Should().Be(expected, $"the SomeString '{SuT.Value}' " + (expectedTrue
                ? $"contains the substring '{particle.String}' at position {particle.Position}"
                : $"doesn't contain the substring '{particle.String}' {(index < 0 ? $"when using startindex {startIndex} and comparer {comparer}" : "at all")}"));
        }

        private void doTest_IndexOf6(bool expectedTrue, int index, int startIndex, int count, StringComparison comparer, bool changeCase)
        {
            var particle = getTestStringParticle(index);
            if (changeCase)
                particle = (particle.Position, String: particle.String.ToUpper());

            var result = SuT.IndexOf(particle.String, startIndex, count, comparer);

            int expected = expectedTrue ? index : -1;
            result.Should().Be(expected, $"the SomeString '{SuT.Value}' " + (expectedTrue
                ? $"contains the substring '{particle.String}' at position {particle.Position}"
                : $"doesn't contain the substring '{particle.String}' {(index < 0 ? $"when using startindex {startIndex}, count {count}, and comparer {comparer}" : "at all")}"));
        }
    }
}