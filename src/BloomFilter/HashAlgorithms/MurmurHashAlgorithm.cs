using MurmurHash;
using System;

namespace BloomFilter
{
    public sealed class MurmurHashAlgorithm : IHashAlgorithm
    {
        public uint Hash(ref ReadOnlySpan<byte> data) =>
            MurmurHash3.Hash32(ref data, 360u);
                
        public static MurmurHashAlgorithm Create()
            => new();
    }
}
