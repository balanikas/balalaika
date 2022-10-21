namespace BlazorApp.Data;

using System.Diagnostics;
using System.Text.Json;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Options;

public class BenchmarkService
{
    private readonly IAmazonSQS _client;
    private readonly AppOptions _options;

    public BenchmarkService(IAmazonSQS client, IOptions<AppOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<BenchmarkResult> Run(string code)
    {
        var options = ScriptOptions.Default.WithImports("System");
        Stopwatch watch = Stopwatch.StartNew();
        try
        {
            var evaluationResult = await CSharpScript.EvaluateAsync(code, options);

        }
        catch (CompilationErrorException e)
        {
            Console.WriteLine(string.Join(Environment.NewLine, e.Diagnostics));
        }

        var result = new BenchmarkResult
        {
            ExecutionId = Guid.NewGuid(),
            TimeTaken = watch.Elapsed

        };
        await PostToQueue(result);
        return await Task.FromResult(result);
    }

    public async Task PostToQueue(BenchmarkResult payload)
    {
        var queueUrl = await _client.GetQueueUrlAsync(_options.ComputeQueueName);
        var request = new SendMessageRequest()
        {
            MessageBody = JsonSerializer.Serialize(payload),
            QueueUrl = queueUrl.QueueUrl 
        };

        try
        {
            var result = await _client.SendMessageAsync(request);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
        }

    }
}
