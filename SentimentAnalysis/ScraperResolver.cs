namespace SentimentAnalysis;

public class ScraperResolver
{
    private readonly IEnumerable<IScraper> _scrapers;

    public ScraperResolver(IEnumerable<IScraper> scrapers)
    {
        _scrapers = scrapers;
    }

    public IScraper Resolve(ScraperType type)
    {
        return type switch
        {
            ScraperType.Wikipedia => _scrapers.OfType<WikipediaScraper>().Single(),
            ScraperType.Reddit => _scrapers.OfType<RedditScraper>().Single(),
            _ => throw new Exception()
        };
    }
}
