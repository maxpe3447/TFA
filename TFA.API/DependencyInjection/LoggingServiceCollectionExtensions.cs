using Serilog;
using Serilog.Filters;

namespace TFA.API.DependencyInjection;

public static class LoggingServiceCollectionExtensions
{
    public static IServiceCollection AddApiLogging(this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment webHostEnvironment)
    {
        return services.AddLogging(b => b.AddSerilog(new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.WithProperty("Application", "TFA.API")
    .Enrich.WithProperty("Environment", webHostEnvironment.EnvironmentName)
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(Matching.FromSource("Microsoft"))
        .WriteTo.OpenSearch(
        configuration.GetConnectionString("Logs"),
        "forum-logs-{0:yyyy.MM.dd}"))
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(Matching.FromSource("Microsoft"))
        .WriteTo.Console())
    .CreateLogger()));
    }
}
