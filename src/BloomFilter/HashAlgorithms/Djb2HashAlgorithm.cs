using System;

namespace BloomFilter
{
    public sealed class Djb2HashAlgorithm : IHashAlgorithm
    {
        private Djb2HashAlgorithm() { }

        public uint Hash(ref ReadOnlySpan<byte> bytes)
        {
            uint hash = 5381;
            foreach (byte b in bytes)
                hash = ((hash << 5) + hash) + b;
            
            return hash;
        }

        public static Djb2HashAlgorithm Create()
            => new();
    }
}
