using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SVSPlayer.Features.Games;

namespace SVSPlayer.Features;

public static class FeaturesExtensions
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.AddScoped<IGamesService, GamesService>();
        
        return services;
    }

    public static WebApplication UseFeatures(this WebApplication app)
    {
        app.UseGames();

        return app;
    }
}