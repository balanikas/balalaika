using Amazon.Comprehend;
using Amazon.Comprehend.Model;

namespace BlazorApp.Pages;


public class Result
{
    public string Text { get; set; }
    public string Url { get; set; }
    public DateTime TimeStamp { get; set; }
    public Guid ExecutionId { get; set; }
    public SentimentType Sentiment { get; set; }
    public SentimentScore SentimentScore { get; set; }
}

