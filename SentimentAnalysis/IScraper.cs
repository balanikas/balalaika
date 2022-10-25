namespace SentimentAnalysis;

public interface IScraper
{
    Task<string?> Download(string url);
}
