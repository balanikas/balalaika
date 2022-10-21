using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Lambda;

public class Function
{
    private readonly ResultsRepository _repository;

    public Function()
    {
        var startup = new Startup();
        IServiceProvider provider = startup.ConfigureServices();
        _repository = provider.GetRequiredService<ResultsRepository>();
    }

    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach(var rec in sqsEvent.Records){
            var message = JsonSerializer.Deserialize<Message>(rec.Body);
            Console.WriteLine("ExecutionId " + message.ExecutionId);
            Console.WriteLine("TimeTaken " + message.TimeTaken);
            var created = await _repository.CreateBucketAsync();
            if (created)
            {
                await _repository.UploadResultAsync(message.ExecutionId.ToString(), message);
            }
        }
        
        await Task.CompletedTask;
    }
}

public class Message
{
    public Guid ExecutionId { get; set; }
    public TimeSpan TimeTaken { get; set; }
}
