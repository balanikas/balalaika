namespace Lambda;

public class BenchmarkResult
{
    public Guid ExecutionId { get; set; }
    public TimeSpan TimeTaken { get; set; }
    public string Log { get; set; } = "";
}
