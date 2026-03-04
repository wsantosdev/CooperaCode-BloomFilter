using System;

namespace BloomFilter
{
    public class BloomFilterOptimizer
    {
        // m = - (n * ln(p)) / (ln(2)^2)
        public static long OptimalSize(long expectedItems, double errorRatio) =>
            (long)Math.Ceiling(-1 * (expectedItems * Math.Log(errorRatio)) / Math.Pow(Math.Log(2), 2));

        // k = (m / n) * ln(2)
        public static int OptimalHashes(long expedtedItems, long size) =>
            (int)Math.Ceiling((Math.Log(2) * size) / expedtedItems);

        // p = (1 - e^(-k * n / m))^k
        public static double OptimalErrorRatio(int hashes, long size, double insertedItems) =>
            Math.Pow((1 - Math.Exp(-hashes * insertedItems / size)), hashes);

        // n = (m / k) * ln(2)
        public static long OptimalItems(int hashes, long size) => 
            (long)Math.Ceiling((Math.Log(2) * size) / hashes);
    }
}
