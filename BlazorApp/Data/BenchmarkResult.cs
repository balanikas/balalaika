namespace BlazorApp.Data;

public class BenchmarkResult
{
    public Guid ExecutionId { get; set; }
    public TimeSpan TimeTaken { get; set; }
}