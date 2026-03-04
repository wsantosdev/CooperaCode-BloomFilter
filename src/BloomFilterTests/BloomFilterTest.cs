using BloomFilter;
using ConsoleApp;
using WebCrawler;

namespace BloomFilterTests
{
    public class BloomFilterTest
    {
        [Fact]
        public void Should_FalsePositive_Be_Lower_Than_1_Percent()
        {
            //Arrange
            int itemCount = 10_000;
            var filter = new CountingBloomFilter(itemCount, 3, MurmurHashAlgorithm.Create());
            var crawler = new WebCrawlerSolucao(filter);
            crawler.Rastrear(UrlGenerator.Generate(itemCount));

            int absentItemCount = 100_000;
            var absentUrls = UrlGenerator.GenerateWithTld(absentItemCount, ".club");

            //Act
            var falsePositiveCount = 0;
            for (var i = 0; i < itemCount; i++)
            {
                if (crawler.JaVisitou(absentUrls[i]))
                    falsePositiveCount++;
            }

            //Assert
            Assert.True(absentItemCount * 1 / 100 > falsePositiveCount);
        }

        [Fact]
        public void Should_Not_Have_False_Negatives()
        {
            //Arrange
            int itemCount = 10_000;
            var urls = UrlGenerator.Generate(itemCount);

            var filter = new CountingBloomFilter(itemCount, 3, MurmurHashAlgorithm.Create());
            var crawler = new WebCrawlerSolucao(filter);
            crawler.Rastrear(urls);

            //Act
            var falseNegativeCount = 0;
            for (var i = 0; i < itemCount; i++)
            {
                if (!crawler.JaVisitou(urls[i]))
                    falseNegativeCount++;
            }

            //Assert
            Assert.Equal(0, falseNegativeCount);
        }

        [Fact]
        public void Should_Solucao_Be_Faster_Than_Problema()
        {
            //Arrange
            int itemCount = 100_000;
            var urls = UrlGenerator.Generate(itemCount);
            var crawlerProblema = new WebCrawlerProblema();
            var crawlerSolucao = new WebCrawlerSolucao(new CountingBloomFilter(itemCount, 3, MurmurHashAlgorithm.Create()));

            //Assert
            long atual = GC.GetAllocatedBytesForCurrentThread();
            crawlerProblema.Rastrear(urls);
            long totalProblema = GC.GetAllocatedBytesForCurrentThread() - atual;

            atual = GC.GetAllocatedBytesForCurrentThread();
            crawlerSolucao.Rastrear(urls);
            long totalSolucao = GC.GetAllocatedBytesForCurrentThread() - atual;

            Assert.True(totalProblema > totalSolucao);
        }
    }
}
