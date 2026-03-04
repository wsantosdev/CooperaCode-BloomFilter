using System;

namespace BloomFilter
{
    public sealed class Fnv1aHashAlgorithm : IHashAlgorithm
    {
        private const uint FNVOffsetBasis = 0x811C9DC5; // 2166136261
        private const uint FNVPrime = 0x01000193; // 16777619

        public uint Hash(ref ReadOnlySpan<byte> data)
        {
            uint hash = FNVOffsetBasis;
            foreach (byte b in data)
                unchecked // Overflow is desired for hash calculation
                {
                    hash ^= b;
                    hash *= FNVPrime;
                }
            
            return hash;
        }

        public static Fnv1aHashAlgorithm Create()
            => new();
    }
}
