using System;
using Xunit;

namespace HomeWorld.Hashing.Tests
{
    public class CRC32Tests
    {
        [Fact]
        public void ComputeEqual()
        {
            var data = new ReadOnlySpan<byte>(new byte[] { (byte)'a' });
            var something = 0xC1D04330;
            var result1 = CRC32.Fast(data);
            var result2 = CRC32.Slow(data);
            Assert.Equal(result1, result2);
        }
    }
}
