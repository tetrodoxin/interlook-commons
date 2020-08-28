using FluentAssertions;
using System;
using Xunit;

namespace Interlook.Functional.Types.UnitTests
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

            result = EmptyString.Default.Contains("a", StringComparison.InvariantCulture);
            result.Should().BeFalse("an EmptyString does not contain a character, regardless of the string comparison.");

            result = EmptyString.Default.Contains("Alphanumeric8", StringComparison.CurrentCulture);
            result.Should().BeFalse("an EmptyString does not contain characters, regardless of the string comparison.");
        }

        [Fact()]
        public void Contains_SomeCharacter_Negative()
        {
            var result = EmptyString.Default.Contains('a');
            result.Should().BeFalse("an EmptyString does not contain a character.");

            result = EmptyString.Default.Contains(' ');
            result.Should().BeFalse("an EmptyString does not contain characters, not even a whitespace.");

            result = EmptyString.Default.Contains('a', StringComparison.InvariantCulture);
            result.Should().BeFalse("an EmptyString does not contain a character, regardless of the string comparison.");

            result = EmptyString.Default.Contains(' ', StringComparison.CurrentCulture);
            result.Should().BeFalse("an EmptyString does not contain characters, not even a whitespace, regardless of the string comparison.");
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
        public void Create_FromStringEmpty()
        {
            var obj = AnyString.Create(string.Empty);
            obj.Should().BeOfType<EmptyString>("calling StringBase.Create() with an empty string must return an EmptyString instance.");
        }

        [Fact()]
        public void Create_FromStringEmpty_SameAsDefault()
        {
            var obj = AnyString.Create(string.Empty);
            obj.Should().BeSameAs(EmptyString.Default, "calling StringBase.Create() with an empty string must return the exact default instance.");
        }

        [Fact()]
        public void Create_FromStringNull()
        {
            var obj = AnyString.Create(null);
            obj.Should().BeOfType<EmptyString>("calling StringBase.Create() with <null> must return an EmptyString instance.");
        }

        [Fact()]
        public void Create_FromStringNull_SameAsDefault()
        {
            var obj = AnyString.Create(null);
            obj.Should().BeSameAs(EmptyString.Default, "calling StringBase.Create() with <null> must return the exact default instance.");
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
            result.Should().BeTrue("an EmptyString should handle <null> as a valid ending.");
        }

        [Fact()]
        public void Equals_Object_EmptyString()
        {
            var empty = EmptyString.Default;
            object other = AnyString.Create(string.Empty);

            var result = empty.Equals(other);

            result.Should().BeTrue("an EmptyString instance must always match EmptyString.");
        }

        [Fact()]
        public void Equals_Object_StringEmpty()
        {
            var empty = EmptyString.Default;
            object other = string.Empty;

            var result = empty.Equals(other);

            result.Should().BeTrue("two instances of EmptyString must be equal.");
        }

        [Fact()]
        public void Equals_Object_Null_Negative()
        {
            var empty = EmptyString.Default;

            var result = empty.Equals((object)null);

            result.Should().BeFalse("an EmptyString instance never equals <null>.");
        }

        [Fact()]
        public void Equals_Object_StringNonEmpty_Negative()
        {
            var empty = EmptyString.Default;
            object other = "Alphanumeric";

            var result = empty.Equals(other);

            result.Should().BeFalse("an EmptyString instance never equals a non empty string.");
        }

        [Fact()]
        public void Equals_String_StringNonEmpty_Negative()
        {
            var empty = EmptyString.Default;
            string other = "Alphanumeric";

            var result = empty.Equals(other);

            result.Should().BeFalse("an EmptyString instance never equals a non empty string.");
        }

        [Fact()]
        public void Equals_String_Empty()
        {
            var empty = EmptyString.Default;
            var other = string.Empty;

            var result = empty.Equals(other);

            result.Should().BeTrue("An EmptyString instance must be equal to an empty string.");
        }

        [Fact()]
        public void Equals_String_Null_Negative()
        {
            var empty = EmptyString.Default;
            var result = empty.Equals((string)null);

            result.Should().BeFalse("an EmptyString instance never equals <null>.");
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