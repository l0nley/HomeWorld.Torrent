using System;
using System.Runtime.Intrinsics.X86;

namespace HomeWorld.Hashing
{
    public class CRC32
    {
        private const uint DefaultPolynomial = 0x11EDC6F4; // intel poly
        private const uint DefaultSeed = 0xffffffffu;
        private static readonly uint[] _table;


        static CRC32()
        {
            _table = new uint[256];
            for (uint i = 0; i < 256; i++)
            {
                var entry = i;
                for (var j = 0; j < 8; j++)
                {
                    if ((entry & 1) == 1)
                    {
                        entry = (entry >> 1) ^ DefaultPolynomial;
                    }
                    else
                    {
                        entry = entry >> 1;
                    }
                }

                _table[i] = entry;
            }
        }

        public static uint Compute(ReadOnlySpan<byte> bytes)
        {
            return Sse42.IsSupported ? Fast(bytes) : Slow(bytes);
        }

        public static uint Slow(ReadOnlySpan<byte> data)
        {
            var hash = DefaultSeed;
            for (var i = 0; i < data.Length; i++)
            {
                hash = (hash >> 8) ^ _table[data[i] ^ hash & 0xff];
            }
            return ~hash;
        }

        public static uint Fast(ReadOnlySpan<byte> data)
        {
            var hash = DefaultSeed;
            for (var i = 0; i < data.Length; i++)
            {
                hash = Sse42.Crc32(hash, data[i]);
                Avx.set
            }
            return ~hash;
        }

    }
}
