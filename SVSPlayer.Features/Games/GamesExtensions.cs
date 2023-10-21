using Microsoft.AspNetCore.Builder;

namespace SVSPlayer.Features.Games;

public static class GamesExtensions
{
    public static WebApplication UseGames(this WebApplication app)
    {
        app.MapGroup("/api/games")
            .MapGamesApi();

        return app;
    }
}