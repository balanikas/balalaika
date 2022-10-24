

using System.Diagnostics;
using System.Text.Json;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Options;

namespace Lambda;

public class ResultsRepository
{
    private readonly IAmazonS3 _client;
    private readonly AppOptions _options;

    public ResultsRepository(IAmazonS3 client, AppOptions options)
    {
        _client = client;
        this._options = options;
    }

    public async Task<bool> CreateBucketAsync()
    {
        var request = new PutBucketRequest
        {
            BucketName = _options.S3BucketName,
            UseClientRegion = true,
        };

        try
        {
            if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, _options.S3BucketName))
            {
                return true;
            }

            var response = await _client.PutBucketAsync(request);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Error creating bucket");
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
        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine($"Error uploading results");
            return false;
        }

        return true;
    }
}

