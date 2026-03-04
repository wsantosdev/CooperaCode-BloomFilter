using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace BloomFilter
{
    public sealed class CountingBloomFilter
    {
        private readonly byte[] _counts;
        private readonly uint _size;
        private readonly int _hashCount;
        private readonly IHashAlgorithm _hashAlgorithm;
        
        private const uint Increment = 2654435761u; // Constante de Knuth

        public CountingBloomFilter(int expectedItems = 1, 
                                   int hashCount = 0, 
                                   IHashAlgorithm? hashAlgorithm = null)
        {
            _size = (uint)BloomFilterOptimizer.OptimalSize(expectedItems, 0.01);
            _counts = GC.AllocateUninitializedArray<byte>((int)_size);
            _hashCount = hashCount > 0 ? hashCount : BloomFilterOptimizer.OptimalHashes(expectedItems, _size);
            _hashAlgorithm = hashAlgorithm ?? new MurmurHashAlgorithm();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(string item)
        {
            uint hash = ComputeHash(item);
            
            for (int i = 0; i < _hashCount; i++)
            {
                uint index = (hash + (uint)i * Increment) % _size;
                _counts[index]++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(string item)
        {
            uint hash = ComputeHash(item);
                        
            for (int i = 0; i < _hashCount; i++)
            {
                uint index = (hash + (uint)i * Increment) % _size;
                ref byte count = ref _counts[index];
                if (count > 0) count--;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ProbablyContains(string item)
        {
            uint hash = ComputeHash(item);
            
            for (int i = 0; i < _hashCount; i++)
            {
                uint index = (hash + (uint)i * Increment) % _size;
                if (_counts[index] == 0)
                    return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint ComputeHash(string item)
        {
            int maxByteCount = Encoding.UTF8.GetMaxByteCount(item.Length);
            
            if (maxByteCount <= 1024)
            {
                Span<byte> buffer = stackalloc byte[maxByteCount];
                int written = Encoding.UTF8.GetBytes(item, buffer);
                ReadOnlySpan<byte> data = buffer[..written];
                return _hashAlgorithm.Hash(ref data);
            }
            
            byte[] pooled = ArrayPool<byte>.Shared.Rent(maxByteCount);
            try
            {
                int written = Encoding.UTF8.GetBytes(item, pooled.AsSpan());
                ReadOnlySpan<byte> data = pooled.AsSpan(0, written);
                return _hashAlgorithm.Hash(ref data);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(pooled);
            }
        }
    }
}
