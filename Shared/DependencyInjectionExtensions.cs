using Amazon.S3;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;

namespace Shared;

public static class DependencyInjectionExtensions
{
    public static void AddSharedServices(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonS3>();
        services.AddAWSService<IAmazonSQS>();
        services.AddSingleton<ResultsRepository>();
        services.AddSingleton<MessagingService>();
    }
}
