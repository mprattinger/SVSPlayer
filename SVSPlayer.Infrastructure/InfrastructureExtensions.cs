using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SVSPlayer.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var tableClientConnectionString = configuration["AzureWebJobsStorage"];

        services.AddScoped(_ => new TableServiceClient(tableClientConnectionString));
        services.AddScoped(typeof(IRepository<>), typeof(TableClientRepository<>));

        return services;
    }
}