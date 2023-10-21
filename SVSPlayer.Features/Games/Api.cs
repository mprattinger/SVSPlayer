using System.Text.Json;
using FlintSoft.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SVSPlayer.Features.Extensions;
using SVSPlayer.Features.Games.Models;

namespace SVSPlayer.Features.Games;

public static class Api
{
    public static RouteGroupBuilder MapGamesApi(this RouteGroupBuilder group)
    {
        group.MapPost("/upload", handleUpload);
        group.MapGet("/current", handleCurrent);
        group.MapGet("/currentGames", handleCurrentGames);
        return group;
    }
    
    private static async Task<IResult> handleUpload(HttpRequest req, IGamesService gamesService)
    {
        try
        {
            var games = await JsonSerializer.DeserializeAsync<List<GameEntity>>(req.Body);
            if (games == null || games.Count == 0)
            {
                return Results.BadRequest("No Games");
            }
            
            await gamesService.AddGamesAsync(games);
    
            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.BadRequest(e);
        }
    }

    private static async Task<GameEntity?> handleCurrent(HttpRequest req, IGamesService gamesService)
    {
        var result = await gamesService.GetCurrentAsync();
        
        return result;
    }

    private static async Task<IEnumerable<GameEntity>> handleCurrentGames(HttpRequest req, IGamesService gamesService)
    {
        var result = await gamesService.GetCurrentGamesAsync();

        return result;
    }
}