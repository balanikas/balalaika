using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Lambda;

public class Function
{
    private readonly ResultsRepository _repository;
    private readonly BenchmarkService _benchmarkService;

    public Function()
    {
        var startup = new Startup();
        var provider = startup.ConfigureServices();
        _repository = provider.GetRequiredService<ResultsRepository>();
        _benchmarkService = provider.GetRequiredService<BenchmarkService>();
    }

    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var rec in sqsEvent.Records)
        {
            var message = JsonSerializer.Deserialize<Message>(rec.Body);
            Console.WriteLine($"Received body {rec.Body}");
            var created = await _repository.CreateBucketAsync();
            var result = await _benchmarkService.Run(message.Code);
            System.Console.WriteLine(result);
            if (created)
            {
                await _repository.UploadResultAsync(message.ExecutionId.ToString(), result);
                Console.WriteLine("uploaded result");
            }
        }

        await Task.CompletedTask;
    }
}
