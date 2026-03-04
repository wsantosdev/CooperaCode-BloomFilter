using BenchmarkDotNet.Attributes;
using BloomFilter;
using WebCrawler;

namespace Benchmark
{
    [MemoryDiagnoser]
    public class WebCrawlerBenchmark
    {
        private const int ItemCount = 100_000;
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
        public void Murmur()
        {
            var filter = new CountingBloomFilter(ItemCount, hashAlgorithm: MurmurHashAlgorithm.Create());
            var crawler = new WebCrawlerSolucao(filter);
            crawler.Rastrear(_urls);
        }

        [Benchmark]
        public void Djb2()
        {
            var filter = new CountingBloomFilter(ItemCount, hashAlgorithm: Djb2HashAlgorithm.Create());
            var crawler = new WebCrawlerSolucao(filter);
            crawler.Rastrear(_urls);
        }

        [Benchmark]
        public void Fnv1a()
        {
            var filter = new CountingBloomFilter(ItemCount, hashAlgorithm: Fnv1aHashAlgorithm.Create());
            var crawler = new WebCrawlerSolucao(filter);
            crawler.Rastrear(_urls);
        }
    }
}
