using Amazon.Comprehend;
using Amazon.Comprehend.Model;

namespace Lambda;

public class UploadObject
{
    public Guid ExecutionId { get; set; }
    public string Text { get; set; }
    public SentimentType Sentiment { get; set; }
    public SentimentScore SentimentScore { get; set; }
    
    public DateTime TimeStamp { get; set; }
    public string Url { get; set; }
}
