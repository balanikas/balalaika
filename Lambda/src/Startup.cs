using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lambda;

public class Startup
{
    private readonly IConfigurationRoot Configuration;

    public Startup()
    {
        Configuration = new ConfigurationBuilder() // ConfigurationBuilder() method requires Microsoft.Extensions.Configuration NuGet package
            .SetBasePath(Directory.GetCurrentDirectory())  // SetBasePath() method requires Microsoft.Extensions.Configuration.FileExtensions NuGet package
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // AddJsonFile() method requires Microsoft.Extensions.Configuration.Json NuGet package
            .AddEnvironmentVariables() // AddEnvironmentVariables() method requires Microsoft.Extensions.Configuration.EnvironmentVariables NuGet package
            .Build();
    }

    public IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection(); 
        ConfigureLoggingAndConfigurations(services);
        ConfigureApplicationServices(services);
        IServiceProvider provider = services.BuildServiceProvider();
        return provider;
    }


    private void ConfigureLoggingAndConfigurations(ServiceCollection services)
    {
        services.AddSingleton<IConfiguration>(Configuration);
    }

    private void ConfigureApplicationServices(ServiceCollection services)
    {
        AWSOptions awsOptions = Configuration.GetAWSOptions();
        services.AddDefaultAWSOptions(awsOptions);
        services.AddAWSService<IAmazonS3>();
        services.AddSingleton<ResultsRepository>();
    }
}
