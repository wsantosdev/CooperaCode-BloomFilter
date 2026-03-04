namespace ConsoleApp;

public sealed class UrlGenerator
{
    public static List<string> Generate(int count = 1_000_000)
    {
        var urls = new List<string>(count);

        string[] tlds = [".com", ".org", ".net", ".io", ".dev", ".br", ".co", ".app"];
        string[] words = ["tech", "web", "site", "blog", "shop", "news", "media", "cloud",
                         "data", "info", "portal", "hub", "lab", "digital", "online"];

        Random random = new(42);

        for (int i = 0; i < count; i++)
        {
            string domain = random.Next(4) switch
            {
                0 => $"{words[random.Next(words.Length)]}{random.Next(1, 9999)}",
                1 => $"{words[random.Next(words.Length)]}-{words[random.Next(words.Length)]}",
                2 => $"{words[random.Next(words.Length)]}{words[random.Next(words.Length)]}",
                _ => $"site{random.Next(1, 999999)}"
            };

            string tld = tlds[random.Next(tlds.Length)];
            urls.Add($"https://{domain}{tld}");
        }

        return urls;
    }
}