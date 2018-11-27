using BenchmarkDotNet.Attributes;
using System.Runtime.Intrinsics.X86;

namespace HomeWorld.Benchmarks
{
    public class SHA1
    {
        private byte[] _data;
        private System.Security.Cryptography.SHA1 _sha;

        private uint[] _initialState;

        [GlobalSetup]
        public void Setup()
        {
            _data = new byte[4096];
            _sha = System.Security.Cryptography.SHA1.Create();
            _initialState = new uint[] { 0x67452301, 0xEFCDAB89, 0x98BADCFE, 0x10325476, 0xC3D2E1F0 };
        }

        [GlobalCleanup]
        public void CleanUp()
        {
            _sha.Dispose();
        }

        [Benchmark]
        public byte[] Slow()
        {
            return _sha.ComputeHash(_data);
        }

        [Benchmark]
        public void Fast()
        {
            Ssse3.
        }
    }
}
