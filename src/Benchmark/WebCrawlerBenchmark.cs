using BenchmarkDotNet.Attributes;
using Bloom = BloomFilter;
using WebCrawler;
using BloomFilter;

namespace Benchmark
{
    [MemoryDiagnoser]
    public class WebCrawlerBenchmark
    {
        private const int ItemCount = 10_000;
        private List<string> _urls = null!;

        [GlobalSetup]
        public void Setup()
        {
            _urls = UrlGenerator.Generate(ItemCount);
        }

        [Benchmark(Baseline = true)]
        public void Problema()
        {
            var crawler = new WebCrawlerProblema();
            crawler.Rastrear(_urls);
        }

        [Benchmark]
        public void Bloom_Murmur()
        {
            var filter = new Bloom.BloomFilter(ItemCount, hashAlgorithm: MurmurHashAlgorithm.Create());
            var crawler = new WebCrawlerBloomFilter(filter);
            crawler.Rastrear(_urls);
        }

        [Benchmark]
        public void Bloom_Djb2()
        {
            var filter = new Bloom.BloomFilter(ItemCount, hashAlgorithm: Djb2HashAlgorithm.Create());
            var crawler = new WebCrawlerBloomFilter(filter);
            crawler.Rastrear(_urls);
        }

        [Benchmark]
        public void Bloom_Fnv1a()
        {
            var filter = new Bloom.BloomFilter(ItemCount, hashAlgorithm: Fnv1aHashAlgorithm.Create());
            var crawler = new WebCrawlerBloomFilter(filter);
            crawler.Rastrear(_urls);
        }

        [Benchmark]
        public void Couting_Murmur()
        {
            var filter = new CountingBloomFilter(ItemCount, hashAlgorithm: MurmurHashAlgorithm.Create());
            var crawler = new WebCrawlerCounting(filter);
            crawler.Rastrear(_urls);
        }

        [Benchmark]
        public void Counting_Djb2()
        {
            var filter = new CountingBloomFilter(ItemCount, hashAlgorithm: Djb2HashAlgorithm.Create());
            var crawler = new WebCrawlerCounting(filter);
            crawler.Rastrear(_urls);
        }

        [Benchmark]
        public void Counting_Fnv1a()
        {
            var filter = new CountingBloomFilter(ItemCount, hashAlgorithm: Fnv1aHashAlgorithm.Create());
            var crawler = new WebCrawlerCounting(filter);
            crawler.Rastrear(_urls);
        }
    }
}
