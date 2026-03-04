using BloomFilter;

namespace WebCrawler
{
    public sealed class WebCrawlerCounting(CountingBloomFilter filter)
    {
        public bool JaVisitou(string url) =>
            filter.ProbablyContains(url);

        public void MarcarVisitada(string url) =>
            filter.Add(url);

        public void RemoverVisita(string url) =>
            filter.Remove(url);

        public void Rastrear(List<string> urls)
        {
            foreach (var url in urls)
                if (!JaVisitou(url))
                    MarcarVisitada(url);
        }
    }
}
