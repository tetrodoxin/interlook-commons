using System.Linq;
using System.Text;
using Xunit;

namespace Interlook.Security.Tests
{
    public class DisposableBytesTest
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
        public void Cast_Bytes_Same_Test()
        {
            var data = Data.ToArray();
            var d = new DisposableBytes(data);

            var castedBytes = (byte[])d;

            Assert.Same(data, castedBytes);
        }

        [Fact]
        public void Disposing_Chars_Test()
        {
            var chars = Text1.ToCharArray();
            var encoding = Encoding.Unicode;
            var data = encoding.GetBytes(chars);

            char[] currentChars = null;

            using (var d = new DisposableBytes(data))
            {
                // nothing done here
                currentChars = d.GetChars(encoding);
                Assert.Equal(chars, currentChars);
            }

            Assert.All(currentChars, p => Assert.Equal(0, p));
        }

        [Fact]
        public void Disposing_MultipleChars_Test()
        {
            var chars = Text1.ToCharArray();

            var data = Encoding.Unicode.GetBytes(chars);

            char[] currentUnicodeChars = null;
            char[] currentUtf8Chars = null;
            char[] currentAnsiChars = null;

            using (var d = new DisposableBytes(data))
            {
                // nothing done here
                currentUnicodeChars = d.GetChars(Encoding.Unicode);
                currentUtf8Chars = d.GetChars(Encoding.UTF8);
                currentAnsiChars = d.GetChars(Encoding.Default);

                Assert.Equal(chars, currentUnicodeChars);
            }

            Assert.All(currentUnicodeChars, p => Assert.Equal(0, p));
            Assert.All(currentUtf8Chars, p => Assert.Equal(0, p));
            Assert.All(currentAnsiChars, p => Assert.Equal(0, p));
        }

        [Fact]
        public void Disposing_Content_Test()
        {
            var data = Data.ToArray();

            using (var d = new DisposableBytes(data))
            {
                // nothing done here
            }

            Assert.All(data, p => Assert.Equal(0, p));
        }

        [Fact]
        public void Equals_Bytes_Copy_Test()
        {
            var d = new DisposableBytes(Data.ToArray());

            Assert.True(d.Equals(Data.ToArray()));
        }

        [Fact]
        public void Equals_Bytes_Negative_Test()
        {
            byte[] data = Data.ToArray();
            byte[] data2 = Data.Reverse().ToArray();

            data2[4] = data2[4]--;

            var d1 = new DisposableBytes(data);

            Assert.False(d1.Equals(data2));
        }

        [Fact]
        public void Equals_Bytes_Same_Test()
        {
            var d = new DisposableBytes(Data);

            Assert.True(d.Equals(Data));
        }

        [Fact]
        public void Equals_Copy_Test()
        {
            var d1 = new DisposableBytes(Data.ToArray());
            var d2 = new DisposableBytes(Data.ToArray());

            Assert.True(d1.Equals(d2));
        }

        [Fact]
        public void Equals_Negative_Test()
        {
            byte[] data = Data.ToArray();
            byte[] data2 = Data.Reverse().ToArray();

            data2[4] = data2[4]--;

            var d1 = new DisposableBytes(data);
            var d2 = new DisposableBytes(data2);

            Assert.False(d1.Equals(d2));
        }

        [Fact]
        public void Equals_Same_Test()
        {
            var d = new DisposableBytes(Data);

            Assert.True(d.Equals(d));
        }

        [Fact]
        public void GetContent_Test()
        {
            var d = new DisposableBytes(Data);

            Assert.Same(d.GetContent(), Data);
        }

        [Fact]
        public void GetChars_Test()
        {
            var characters = Text2.ToCharArray();
            var data = Encoding.Unicode.GetBytes(characters);

            var d = new DisposableBytes(data);
            var c = d.GetChars(Encoding.Unicode);

            Assert.Equal(characters, c);
        }

        [Fact]
        public void Index_Test()
        {
            var d = new DisposableBytes(Data);

            Assert.Empty(Enumerable.Range(0, Data.Length).Where(i => d[i] != Data[i]));
        }

        [Fact]
        public void Length_Test()
        {
            var d = new DisposableBytes(Data);

            Assert.Equal(d.Length, Data.Length);
        }
    }
}