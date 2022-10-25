using Microsoft.Extensions.DependencyInjection;

namespace SentimentAnalysis;

public static class DependencyInjectionExtensions
{
    public static void AddSentimentAnalysisServices(this IServiceCollection services)
    {
        services.AddSingleton<IScraper, WikipediaScraper>();
        services.AddSingleton<IScraper, RedditScraper>();
        services.AddSingleton<SentimentAnalyzer>();
        services.AddSingleton<ScraperResolver>();
    }
}
