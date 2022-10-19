namespace BlazorApp.Data;

using System.Diagnostics;
using System.Text.Json;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

public class BenchmarkService
{
    public async Task<BenchmarkResult> Run(string code)
    {
        var options = ScriptOptions.Default.WithImports("System"); 
        Stopwatch watch = Stopwatch.StartNew();
        try   {
            var evaluationResult = await CSharpScript.EvaluateAsync(code, options);
            
        }
        catch (CompilationErrorException e)
        {
            Console.WriteLine(string.Join(Environment.NewLine, e.Diagnostics));
        }

        var result = new BenchmarkResult{
            ExecutionId = Guid.NewGuid(),
            TimeTaken = watch.Elapsed
            
        };
        await Post(result);
        return await Task.FromResult(result);
    }

    public async Task Post(BenchmarkResult payload)
    {
        
        var client = new AmazonSQSClient(Amazon.RegionEndpoint.GetBySystemName("us-west-2"));
        var request = new SendMessageRequest()
        {
            
            MessageBody = JsonSerializer.Serialize(payload),
            QueueUrl = "https://sqs.us-west-2.amazonaws.com/686788842590/compute-queue",
        };

        try{
            var result = await client.SendMessageAsync(request);
        }
        catch(Exception e){
            System.Console.WriteLine(e.Message);
        }
        
    }
}

public class BenchmarkResult {
    public Guid ExecutionId { get; set;}
    public TimeSpan TimeTaken { get; set;}
}