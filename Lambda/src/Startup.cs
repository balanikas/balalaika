using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SentimentAnalysis;
using Shared;

namespace Lambda;

public class Startup
{
    private readonly IConfigurationRoot _configuration;

    public Startup()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
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

        var appOptions = new AppOptions();
        _configuration.GetSection("AppOptions").Bind(appOptions);
        services.AddSingleton(appOptions);
        services.AddSharedServices();
        services.AddSentimentAnalysisServices();
    }
}
