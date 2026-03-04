// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;

namespace ConsoleApp
{
    public sealed class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<WebCrawlerBenchmark>();
        }
    }
}


