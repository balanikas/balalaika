using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using SentimentAnalysis;
using Shared;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace Lambda;

public class Function
{
    private readonly ResultsRepository _repository;
    private readonly SentimentAnalyzer _sentimentAnalyzer;

    public Function()
    {
        var startup = new Startup();
        var provider = startup.ConfigureServices();
        _repository = provider.GetRequiredService<ResultsRepository>();
        _sentimentAnalyzer = provider.GetRequiredService<SentimentAnalyzer>();
    }

    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var rec in sqsEvent.Records)
        {
            
            Console.WriteLine($"Received body {rec.Body}");
            var created = await _repository.CreateBucketAsync();
            if (created)
            {
                var message = JsonSerializer.Deserialize<Message>(rec.Body);
                var request = new SentimentRequest
                {
                    Url = new Uri(message.Url), 
                    ScraperType = Enum.Parse<ScraperType>(message.ScraperType)
                };
                var sentimentResult = await _sentimentAnalyzer.Analyze(request);
                var uploadObject = new UploadObject
                {
                    Sentiment = sentimentResult.Sentiment,
                    Text = sentimentResult.Text,
                    SentimentScore = sentimentResult.SentimentScore,
                    ExecutionId = message.ExecutionId,
                    TimeStamp = message.TimeStamp, 
                    Url = message.Url
                };
                await _repository.UploadResultAsync(message.ExecutionId.ToString(), uploadObject);
                Console.WriteLine("uploaded result");
            }
        }

        await Task.CompletedTask;
    }
}
