namespace Lambda;

public class Message
{
    public Guid ExecutionId { get; set; }
    public string Url { get; set; }
    public string ScraperType { get; set; }
    public DateTime TimeStamp { get; set; }
}
