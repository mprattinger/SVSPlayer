using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SVSPlayer.Features.Games;
using System.Globalization;

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

    public static string ToOdataDate(this DateTime theDate)
    {
        var d = theDate.Date.ToString("o", CultureInfo.InvariantCulture);
        return $"datetime'{d}'";
    }
}