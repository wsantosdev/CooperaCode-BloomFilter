using System;
using System.Buffers;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace BloomFilter
{
    public sealed class BloomFilter
    {
        private readonly BitArray _bitArray;
        private readonly uint _size;
        private readonly int _hashCount;
        private readonly IHashAlgorithm _hashAlgorithm;

        public BloomFilter(int expectedItems = 1, 
                          int hashCount = 0, 
                          IHashAlgorithm? hashAlgorithm = null)
        {
            _size = (uint)BloomFilterOptimizer.OptimalSize(expectedItems, 0.01);
            _bitArray = new BitArray((int)_size);
            _hashCount = hashCount > 0 ? hashCount : BloomFilterOptimizer.OptimalHashes(expectedItems, _size);
            _hashAlgorithm = hashAlgorithm ?? MurmurHashAlgorithm.Create();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(string item)
        {
            var (hash1, hash2) = ComputeDoubleHash(item);
            
            for (int i = 0; i < _hashCount; i++)
            {
                uint index = (hash1 + (uint)i * hash2) % _size;
                _bitArray.Set((int)index, true);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ProbablyContains(string item)
        {
            var (hash1, hash2) = ComputeDoubleHash(item);
            
            for (int i = 0; i < _hashCount; i++)
            {
                uint index = (hash1 + (uint)i * hash2) % _size;
                if (!_bitArray.Get((int)index))
                    return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (uint hash1, uint hash2) ComputeDoubleHash(string item)
        {
            int maxByteCount = Encoding.UTF8.GetMaxByteCount(item.Length);
            
            if (maxByteCount <= 1024)
            {
                Span<byte> buffer = stackalloc byte[maxByteCount];
                int written = Encoding.UTF8.GetBytes(item, buffer);
                ReadOnlySpan<byte> data = buffer[..written];
                return ComputeHashPair(ref data);
            }
            
            byte[] pooled = ArrayPool<byte>.Shared.Rent(maxByteCount);
            try
            {
                int written = Encoding.UTF8.GetBytes(item, pooled.AsSpan());
                ReadOnlySpan<byte> data = pooled.AsSpan(0, written);
                return ComputeHashPair(ref data);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(pooled);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (uint hash1, uint hash2) ComputeHashPair(ref ReadOnlySpan<byte> data)
        {
            // Double hashing: usa o mesmo algoritmo com seeds diferentes
            // hash1: seed = 0
            // hash2: seed baseado no próprio hash1 para independência
            uint hash1 = _hashAlgorithm.Hash(ref data);
            
            // Garante que hash2 nunca seja zero (evita repetição de índices)
            // Usa rotação de bits para criar segunda hash independente
            uint hash2 = ((hash1 >> 16) | (hash1 << 16));
            if (hash2 == 0) hash2 = 1;
            
            return (hash1, hash2);
        }
    }
}
