

using System.Diagnostics;
using System.Text.Json;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace BlazorApp.Data;

public class ResultsRepository
{
    private readonly IAmazonS3 _client;
    private readonly AppOptions _options;

    public ResultsRepository(IAmazonS3 client, IOptions<AppOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<IEnumerable<ExecutionResult>> ListBucketContentsAsync()
    {
        var results = new List<ExecutionResult>();
        try
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _options.S3BucketName,
                //MaxKeys = 5,
            };

            var response = new ListObjectsV2Response();

            do
            {
                response = await _client.ListObjectsV2Async(request);

                foreach (var x in response.S3Objects.OrderBy(x => x.LastModified))
                {
                    var o = await _client.GetObjectAsync(_options.S3BucketName, x.Key);

                    var a = JsonSerializer.Deserialize<ExecutionResult>(o.ResponseStream);
                    results.Add(a);
                }

                // If the response is truncated, set the request ContinuationToken
                // from the NextContinuationToken property of the response.
                request.ContinuationToken = response.NextContinuationToken;
            }
            while (response.IsTruncated);

            return results;
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error encountered on server. Message:'{ex.Message}' getting list of objects.");
            return results;
        }
    }
}

