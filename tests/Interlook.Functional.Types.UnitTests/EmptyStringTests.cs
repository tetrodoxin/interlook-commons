using FluentAssertions;
using System;
using Xunit;

namespace Interlook.Functional.Types.Tests
{
    public class EmptyStringTests
    {
        [Fact()]
        public void Contains_Characters_Negative()
        {
            var result = EmptyString.Default.Contains("a");
            result.Should().BeFalse("an EmptyString does not contain a character.");

            result = EmptyString.Default.Contains("Alphanumeric8");
            result.Should().BeFalse("an EmptyString does not contain characters.");
        }

        [Fact()]
        public void Contains_EmptyString()
        {
            var result = EmptyString.Default.Contains(string.Empty);
            result.Should().BeTrue("An EmptyString contains an empty string.");
        }

        [Fact()]
        public void Contains_EmptyStringInstance()
        {
            var result = EmptyString.Default.Contains(string.Empty);
            result.Should().BeTrue("an EmptyString contains an EmptyString instance.");
        }

        [Fact()]
        public void Contains_Null()
        {
            var result = EmptyString.Default.Contains(null);
            result.Should().BeTrue("An EmptyString should handle <null> as contained.");
        }

        [Fact()]
        public void Contains_WhiteSpaces_Negative()
        {
            var result = EmptyString.Default.Contains(" ");
            result.Should().BeFalse("an EmptyString does not contain a whitespace.");

            result = EmptyString.Default.Contains("    ");
            result.Should().BeFalse("an EmptyString does not contain whitespaces.");
        }

        [Fact()]
        public void Create_FromEmptyString()
        {
            var obj = StringBase.Create(string.Empty);
            obj.Should().BeOfType<EmptyString>("Calling StringBase.Create() with an empty string must return an EmptyString instance.");
        }

        [Fact()]
        public void Create_FromNullString()
        {
            var obj = StringBase.Create(null);
            obj.Should().BeOfType<EmptyString>("Calling StringBase.Create() with <null> must return an EmptyString instance.");
        }

        [Theory]
        [InlineData(" ", "an emptyString does not end with a whitespace")]
        [InlineData("   ", "an emptyString does not end with whitespaces")]
        [InlineData("alpha8", "an emptyString does not end with characters")]
        public void EndsWith_Characters_Negative(string s, string errortext)
        {
            var result = EmptyString.Default.EndsWith(s);
            result.Should().BeFalse(errortext);
        }

        [Fact()]
        public void EndsWith_EmptyString()
        {
            var result = EmptyString.Default.EndsWith(string.Empty);
            result.Should().BeTrue("An EmptyString ends with an empty string.");
        }

        [Fact()]
        public void EndsWith_EmptyStringInstance()
        {
            var result = EmptyString.Default.StartsWith(string.Empty);
            result.Should().BeTrue("an EmptyString ends with an EmptyString instance.");
        }

        [Fact()]
        public void EndsWith_Null()
        {
            var result = EmptyString.Default.StartsWith(null);
            result.Should().BeTrue("An EmptyString should handle <null> as a valid ending.");
        }

        [Fact()]
        public void Equals_DefaultAndEmptyCreated()
        {
            var empty = EmptyString.Default;
            var other = StringBase.Create(string.Empty);

            var result = empty.Equals(other);

            result.Should().BeTrue("Two instances of EmptyString must be equal.");
        }

        [Fact()]
        public void Equals_StringEmpty()
        {
            var empty = EmptyString.Default;
            var other = string.Empty;

            var result = empty.Equals(other);

            result.Should().BeTrue("An EmptyString instance must be equal to an empty string.");
        }

        [Theory]
        [InlineData(" ", "an emptyString does not start with a whitespace")]
        [InlineData("   ", "an emptyString does not start with whitespaces")]
        [InlineData("alpha8", "an emptyString does not start with characters")]
        public void StartsWith_Characters_Negative(string s, string errortext)
        {
            var result = EmptyString.Default.StartsWith(s);
            result.Should().BeFalse(errortext);
        }

        [Fact()]
        public void StartsWith_EmptyString()
        {
            var result = EmptyString.Default.StartsWith(string.Empty);
            result.Should().BeTrue("An EmptyString starts with an empty string.");
        }

        [Fact()]
        public void StartsWith_EmptyStringInstance()
        {
            var result = EmptyString.Default.StartsWith(string.Empty);
            result.Should().BeTrue("an EmptyString starts with an EmptyString instance.");
        }

        [Fact()]
        public void StartsWith_Null()
        {
            var result = EmptyString.Default.StartsWith(null);
            result.Should().BeTrue("An EmptyString should handle <null> as a valid start.");
        }

        [Fact()]
        public void ToString_IsEmptyString()
        {
            var result = EmptyString.Default.ToString();

            result.Should().BeEmpty("ToString() of EmptyString must return an emtpy string.");
        }

        [Fact]
        public void IndexOf1_Empty()
        {
            var result = EmptyString.Default.IndexOf(string.Empty);
            result.Should().Be(0, "an EmptyString instance has an empty string at index 0");
        }

        [Fact]
        public void IndexOf2_Empty()
        {
            var result = EmptyString.Default.IndexOf(string.Empty, 0);
            result.Should().Be(0, "an EmptyString instance has an empty string at index 0");
        }

        [Fact]
        public void IndexOf3_Negative()
        {
            EmptyString.Default.Invoking(r => r.IndexOf(string.Empty, 0, 1))
                .Should().Throw<ArgumentOutOfRangeException>("A count cannot be valid in an empty string.");
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
            var result = EmptyString.Default.IndexOf(string.Empty, comparer);
            result.Should().Be(0, $"an EmptyString instance has an empty string at index 0, even with comparer {comparer.ToString()}");
        }

        [Theory]
        [InlineData(StringComparison.OrdinalIgnoreCase)]
        [InlineData(StringComparison.Ordinal)]
        [InlineData(StringComparison.CurrentCulture)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(StringComparison.InvariantCulture)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase)]
        public void IndexOf5_Empty(StringComparison comparer)
        {
            var result = EmptyString.Default.IndexOf(string.Empty, 0, comparer);
            result.Should().Be(0, $"an EmptyString instance has an empty string at index 0, even with comparer {comparer.ToString()}");
        }

        [Theory]
        [InlineData(StringComparison.OrdinalIgnoreCase)]
        [InlineData(StringComparison.Ordinal)]
        [InlineData(StringComparison.CurrentCulture)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(StringComparison.InvariantCulture)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase)]
        public void IndexOf6_Empty(StringComparison comparer)
        {
            EmptyString.Default.Invoking(r => r.IndexOf(string.Empty, 0, 1, comparer))
                .Should().Throw<ArgumentOutOfRangeException>("A count cannot be valid in an empty string, independent from comparer.");
        }
    }
}