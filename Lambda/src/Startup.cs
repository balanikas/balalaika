using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lambda;

public class Startup
{
    private readonly IConfigurationRoot _configuration;

    public Startup()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
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
        services.AddSingleton<IConfiguration>(_configuration);
    }

    private void ConfigureApplicationServices(ServiceCollection services)
    {
        services.AddDefaultAWSOptions(_configuration.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();

        var appOptions = new AppOptions();
        _configuration.GetSection("AppOptions").Bind(appOptions);
        services.AddSingleton(appOptions);
        services.AddSingleton<ResultsRepository>();
        services.AddSingleton<BenchmarkService>();
    }
}
