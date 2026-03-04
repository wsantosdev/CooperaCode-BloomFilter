namespace BloomFilterTests
{
    using BloomFilter;
    using WebCrawler;

    namespace BloomFilterTests
    {
        public class CountingBloomFilterTest
        {
            [Fact]
            public void Should_FalsePositive_Be_Lower_Than_1_Percent()
            {
                //Arrange
                int itemCount = 10_000;
                var filter = new BloomFilter(itemCount, 3, MurmurHashAlgorithm.Create());
                var crawler = new WebCrawlerBloomFilter(filter);
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

            [Theory]
            [InlineData(100)]
            [InlineData(1_000)]
            [InlineData(10_000)]
            [InlineData(100_000)]
            public void Should_Not_Have_False_Negatives(int itemCount)
            {
                //Arrange
                var urls = UrlGenerator.Generate(itemCount);

                var filter = new BloomFilter(itemCount, 3, MurmurHashAlgorithm.Create());
                var crawler = new WebCrawlerBloomFilter(filter);
                crawler.Rastrear(urls);

                //Act
                foreach (var url in urls)
                {
                    filter.Add(url);
                }

                //Assert
                var foundAll = urls.All(crawler.JaVisitou);
                Assert.True(foundAll);
            }

            [Fact]
            public void Should_Solucao_Be_Faster_Than_Problema()
            {
                //Arrange
                int itemCount = 100_000;
                var urls = UrlGenerator.Generate(itemCount);
                var crawlerProblema = new WebCrawlerProblema();
                var crawlerSolucao = new WebCrawlerBloomFilter(new BloomFilter(itemCount, 3, MurmurHashAlgorithm.Create()));

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

}
