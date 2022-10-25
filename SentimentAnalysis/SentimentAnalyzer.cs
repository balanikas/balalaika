using Amazon;
using Amazon.Comprehend;
using Amazon.Comprehend.Model;

namespace SentimentAnalysis;

public class SentimentAnalyzer
{
    private readonly ScraperResolver _resolver;

    public SentimentAnalyzer(ScraperResolver resolver)
    {
        _resolver = resolver;
    }

    public async Task<SentimentResult> Analyze(string url, ScraperType scraperType)
    {
        return await Analyze(new SentimentRequest { ScraperType = scraperType, Url = new Uri(url) });
    }

    public async Task<SentimentResult> Analyze(SentimentRequest request)
    {
        var scraper = _resolver.Resolve(request.ScraperType);
        var text = await scraper.Download(request.Url.AbsoluteUri);

        if (text is null)
        {
            return await Task.FromResult(SentimentResult.Empty);
        }

        text = text.Length > 5000 ? text[..4900] : text;
        //inject client
        var comprehendClient = new AmazonComprehendClient(RegionEndpoint.USWest2);

        var detectSentimentRequest = new DetectSentimentRequest { Text = text, LanguageCode = "en" };
        var detectSentimentResponse = await comprehendClient.DetectSentimentAsync(detectSentimentRequest);

        return new SentimentResult
        {
            Text = text,
            Sentiment = detectSentimentResponse.Sentiment,
            SentimentScore = new SentimentScore
            {
                Mixed = detectSentimentResponse.SentimentScore.Mixed,
                Negative = detectSentimentResponse.SentimentScore.Negative,
                Neutral = detectSentimentResponse.SentimentScore.Neutral,
                Positive = detectSentimentResponse.SentimentScore.Positive
            }
        };
    }
}
