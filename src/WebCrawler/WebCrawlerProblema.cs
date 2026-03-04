
namespace WebCrawler
{
    public sealed class WebCrawlerProblema
    {
        private readonly HashSet<string> urlsVisitadas = [];

        public bool JaVisitou(string url)
        {
            return urlsVisitadas.Contains(url);
        }

        public void MarcarVisitada(string url)
        {
            urlsVisitadas.Add(url);
        }

        public void Rastrear(List<string> urls)
        {
            foreach (var url in urls)
            {
                if (!JaVisitou(url))
                {
                    MarcarVisitada(url);
                }
            }
        }
    }
}
