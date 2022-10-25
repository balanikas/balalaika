using Amazon.Comprehend;
using Amazon.Comprehend.Model;

namespace SentimentAnalysis;

public class SentimentResult
{
    public string Text { get; set; }
    public SentimentType Sentiment { get; set; }
    public SentimentScore SentimentScore { get; set; }

    public static SentimentResult Empty => new()
    {
        Sentiment = new SentimentType("N/A"), SentimentScore = new SentimentScore()
    };
}
