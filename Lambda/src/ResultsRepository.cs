namespace Lambda;

using System.Diagnostics;
using System.Text.Json;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

public class ResultsRepository
{
    private readonly IAmazonS3 _client;

    public ResultsRepository(IAmazonS3 client)
    {
        _client = client;
    }

    public async Task<bool> CreateBucketAsync()
    {
        try
        {
            var request = new PutBucketRequest
            {
                BucketName = "execution-results-balalaika",
                UseClientRegion = true,
            };
            if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, "execution-results-balalaika")) {
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
        Message data)
    {
        var request = new PutObjectRequest
        {
            BucketName = "execution-results-balalaika",
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

}

