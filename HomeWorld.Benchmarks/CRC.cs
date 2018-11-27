using BenchmarkDotNet.Attributes;
using HomeWorld.Hashing;

namespace HomeWorld.Benchmarks
{
    public class CRC
    {
        private byte[] _data;

        [GlobalSetup]
        public void Setup()
        {
            _data = new byte[4096];
        }


        [Benchmark]
        public uint Fast()
        {
            return CRC32.Fast(_data);
        }

        [Benchmark]
        public uint Slow()
        {
            return CRC32.Slow(_data);
        }
    }
}
