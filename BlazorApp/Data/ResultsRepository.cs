namespace BlazorApp.Data;

using System.Diagnostics;
using System.Text.Json;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Options;

public class ResultsRepository
{
    private readonly IAmazonS3 _client;
    private readonly AppOptions _options;

    public ResultsRepository(IAmazonS3 client, IOptions<AppOptions> options)
    {
        _client = client;
        this._options = options.Value;
    }

    public async Task<bool> CreateBucketAsync()
    {
        try
        {
            var request = new PutBucketRequest
            {
                BucketName = _options.S3BucketName,
                UseClientRegion = true,
            };
            if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, _options.S3BucketName)) {
                return true;
            }

            var response = await _client.PutBucketAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error creating bucket: '{ex.Message}'");
            return false;
        }
    }

    public async Task<bool> UploadResultAsync(
        string objectName,
        BenchmarkResult data)
    {
        var request = new PutObjectRequest
        {
            BucketName = _options.S3BucketName,
            Key = objectName,
            ContentBody = JsonSerializer.Serialize(data)
        };

        var response = await _client.PutObjectAsync(request);
        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            return true;
        }
        else
        {
            return false;
        }
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

            Console.WriteLine("--------------------------------------");
            Console.WriteLine($"Listing the contents of {_options.S3BucketName}:");
            Console.WriteLine("--------------------------------------");

            var response = new ListObjectsV2Response();

            do
            {
                response = await _client.ListObjectsV2Async(request);

                foreach(var x in response.S3Objects.OrderBy(x=> x.LastModified)){
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

