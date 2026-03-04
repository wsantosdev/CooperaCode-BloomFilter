using System;

namespace BloomFilter
{
    public interface IHashAlgorithm
    {
        uint Hash(ref ReadOnlySpan<byte> data);
    }
}
