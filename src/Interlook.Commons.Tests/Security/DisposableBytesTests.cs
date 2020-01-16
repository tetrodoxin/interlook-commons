using System.Linq;
using System.Text;
using Xunit;

namespace Interlook.Security.Tests
{
    public class DisposableCharsTests
    {
        public static readonly byte[] Data =
        {
            29, 194, 71, 176, 224, 158, 20, 9, 151, 108, 40,
            110, 18, 137, 231, 17, 56, 161, 214, 245, 110, 50,
            22, 214, 196, 248, 23, 14, 197, 239, 17, 190
        };

        private const string Text1 = "And thou shall add the Book Of Flavor Flav to the Bible";
        private const string Text2 = "I'm the least you could do, If only life were as easy as you";

        [Fact]
        public void Cast_Chars_Same_Test()
        {
            var data = Text1.ToCharArray();
            var d = new DisposableChars(data);

            var castedBytes = (char[])d;

            Assert.Same(data, castedBytes);
        }

        [Fact]
        public void Disposing_Chars_Test()
        {
            var chars = Text1.ToCharArray();
            var encoding = Encoding.Unicode;
            var data = encoding.GetBytes(chars);

            byte[] currentBytes = null;

            using (var d = new DisposableChars(chars))
            {
                // nothing done here
                currentBytes = d.GetBytes(encoding);
                Assert.Equal(data, currentBytes);
            }

            Assert.All(currentBytes, p => Assert.Equal(0, p));
        }

        [Fact]
        public void Disposing_MultipleChars_Test()
        {
            var chars = Text1.ToCharArray();

            var data = Encoding.Unicode.GetBytes(chars);

            byte[] currentUnicodeBytes = null;
            byte[] currentUtf8Bytes = null;
            byte[] currentAnsiBytes = null;

            using (var d = new DisposableChars(chars))
            {
                // nothing done here
                currentUnicodeBytes = d.GetBytes(Encoding.Unicode);
                currentUtf8Bytes = d.GetBytes(Encoding.UTF8);
                currentAnsiBytes = d.GetBytes(Encoding.Default);

                Assert.Equal(data, currentUnicodeBytes);
            }

            Assert.All(currentUnicodeBytes, p => Assert.Equal(0, p));
            Assert.All(currentUtf8Bytes, p => Assert.Equal(0, p));
            Assert.All(currentAnsiBytes, p => Assert.Equal(0, p));
        }

        [Fact]
        public void Disposing_Content_Test()
        {
            var data = Text1.ToCharArray();

            using (var d = new DisposableChars(data))
            {
                // nothing done here
            }

            Assert.All(data, p => Assert.Equal(0, p));
        }

        [Fact]
        public void Equals_Chars_Copy_Test()
        {
            var d = new DisposableChars(Text1.ToCharArray());

            Assert.True(d.Equals(Text1.ToCharArray()));
        }

        [Fact]
        public void Equals_Chars_Negative_Test()
        {
            var chars = Text1.ToCharArray();
            var chars2 = Text2.ToCharArray();

            var d1 = new DisposableChars(chars);

            Assert.False(d1.Equals(chars2));
        }

        [Fact]
        public void Equals_Chars_Same_Test()
        {
            var characters = Text1.ToCharArray();
            var d = new DisposableChars(characters);

            Assert.True(d.Equals(characters));
        }

        [Fact]
        public void Equals_Copy_Test()
        {
            var d1 = new DisposableChars(Text1.ToArray());
            var d2 = new DisposableChars(Text1.ToArray());

            Assert.True(d1.Equals(d2));
        }

        [Fact]
        public void Equals_Negative_Test()
        {
            var chars1 = Text1.ToCharArray();
            var chars2 = Text2.ToCharArray();

            var d1 = new DisposableChars(chars1);
            var d2 = new DisposableChars(chars2);

            Assert.False(d1.Equals(d2));
        }

        [Fact]
        public void Equals_Same_Test()
        {
            var d = new DisposableChars(Text1.ToCharArray());

            Assert.True(d.Equals(d));
        }

        [Fact]
        public void GetContent_Test()
        {
            char[] content = Text1.ToCharArray();
            var d = new DisposableChars(content);

            Assert.Same(d.GetContent(), content);
        }

        [Fact]
        public void GetBytes_Test()
        {
            var characters = Text2.ToCharArray();
            var data = Encoding.Unicode.GetBytes(characters);

            var d = new DisposableChars(characters);
            var c = d.GetBytes(Encoding.Unicode);

            Assert.Equal(data, c);
        }

        [Fact]
        public void Index_Test()
        {
            char[] data = Text2.ToCharArray();
            var d = new DisposableChars(data);

            Assert.Empty(Enumerable.Range(0, data.Length).Where(i => d[i] != data[i]));
        }

        [Fact]
        public void Length_Test()
        {
            char[] data = Text1.ToCharArray();
            var d = new DisposableChars(data);

            Assert.Equal(d.Length, data.Length);
        }
    }
}