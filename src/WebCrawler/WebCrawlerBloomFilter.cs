using Bloom = BloomFilter;

namespace WebCrawler
{
    public sealed class WebCrawlerBloomFilter(Bloom.BloomFilter filter)
    {
        public bool JaVisitou(string url) =>
            filter.ProbablyContains(url);

        public void MarcarVisitada(string url) =>
            filter.Add(url);

        public void Rastrear(List<string> urls)
        {
            foreach (var url in urls)
                if (!JaVisitou(url))
                    MarcarVisitada(url);
        }
    }
}
