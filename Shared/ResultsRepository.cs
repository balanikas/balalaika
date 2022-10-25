using System.Net;
using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Options;

namespace Shared;

public class ResultsRepository
{
    private readonly IAmazonS3 _client;
    private readonly AppOptions _options;

    public ResultsRepository(IAmazonS3 client, AppOptions options)
    {
        _client = client;
        _options = options;
    }


    public async Task<bool> CreateBucketAsync()
    {
        try
        {
            if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, _options.S3BucketName))
            {
                return true;
            }
            
            var request = new PutBucketRequest { BucketName = _options.S3BucketName, UseClientRegion = true };
            var response = await _client.PutBucketAsync(request);
            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("Error creating bucket");
                return false;
            }

            return true;
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error creating bucket: '{ex.Message}'");
            return false;
        }
    }

    public async Task<bool> UploadResultAsync<T>(
        string objectName,
        T data)
    {
        var request = new PutObjectRequest
        {
            BucketName = _options.S3BucketName, Key = objectName, ContentBody = JsonSerializer.Serialize(data)
        };

        var response = await _client.PutObjectAsync(request);
        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine("Error uploading results");
            return false;
        }

        return true;
    }

    public async Task<IEnumerable<T>> List<T>() 
    {
        var results = new List<T>();
        try
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _options.S3BucketName
                //MaxKeys = 5,
            };

            var response = new ListObjectsV2Response();

            do
            {
                response = await _client.ListObjectsV2Async(request);

                foreach (var x in response.S3Objects.OrderBy(x => x.LastModified))
                {
                    var o = await _client.GetObjectAsync(_options.S3BucketName, x.Key);

                    var a = JsonSerializer.Deserialize<T>(o.ResponseStream);
                    results.Add(a);
                }

                // If the response is truncated, set the request ContinuationToken
                // from the NextContinuationToken property of the response.
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return results;
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error encountered on server. Message:'{ex.Message}' getting list of objects.");
            return results;
        }
    }
}


