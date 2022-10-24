using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;

namespace BlazorApp.Data;

public class MessagingService
{
    private readonly IAmazonSQS _client;
    private readonly AppOptions _options;

    public MessagingService(IAmazonSQS client, IOptions<AppOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<bool> PostToQueue(ExecutionRequest payload)
    {
        var queueUrlResponse = await _client.GetQueueUrlAsync(_options.ComputeQueueName);
        if (queueUrlResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
        {
            System.Console.WriteLine($"Cannot get queue url for queue {_options.ComputeQueueName}");
            return false;
        }

        var request = new SendMessageRequest()
        {
            MessageBody = JsonSerializer.Serialize(payload),
            QueueUrl = queueUrlResponse.QueueUrl
        };

        try
        {
            var result = await _client.SendMessageAsync(request);
            return true;
        }
        catch (Exception e)
        {
            System.Console.WriteLine($"Cannot send message to queue {queueUrlResponse.QueueUrl}");
            System.Console.WriteLine(e.Message);
            return false;
        }

    }
}
