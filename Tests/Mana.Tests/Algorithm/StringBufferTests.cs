using Mana.Utilities.Algorithm;
using NUnit.Framework;

namespace Tests.Algorithm
{
    [TestFixture]
    public class StringBufferTests
    {
        [Test]
        public void TestAppend()
        {
            var buffer = new StringBuffer(0);

            Assert.That(buffer.Capacity == 0);
            
            buffer.Append("Hello");
            buffer.Append(", world");

            Assert.AreEqual(buffer.ToArray(), "Hello, world".ToCharArray());
        }
        
        [Test]
        public void TestAppendFrom()
        {
            var buffer = new StringBuffer(32);
            
            buffer.Append("Hello, world");
            buffer.AppendAt(2, "aaa");
            Assert.AreEqual(buffer.ToArray(), "Heaaa, world".ToCharArray());
            
            buffer.AppendAt(11, "aaa");
            Assert.AreEqual(buffer.ToArray(), "Heaaa, worlaaa".ToCharArray());
        }
    }
}