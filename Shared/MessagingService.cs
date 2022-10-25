using System.Net;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;

namespace Shared;

public class MessagingService
{
    private readonly IAmazonSQS _client;
    private readonly AppOptions _options;

    public MessagingService(IAmazonSQS client, AppOptions options)
    {
        _client = client;
        _options = options;
    }

    public async Task<bool> PostToQueue(MessagingRequest payload)
    {
        var queueUrlResponse = await _client.GetQueueUrlAsync(_options.ComputeQueueName);
        if (queueUrlResponse.HttpStatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine($"Cannot get queue url for queue {_options.ComputeQueueName}");
            return false;
        }

        var request = new SendMessageRequest
        {
            MessageBody = JsonSerializer.Serialize(payload), QueueUrl = queueUrlResponse.QueueUrl
        };

        try
        {
            var result = await _client.SendMessageAsync(request);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Cannot send message to queue {queueUrlResponse.QueueUrl}");
            Console.WriteLine(e.Message);
            return false;
        }
    }
}
