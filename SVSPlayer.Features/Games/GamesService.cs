using FlintSoft.Extensions;
using Microsoft.Extensions.Logging;
using SVSPlayer.Features.Games.Models;
using SVSPlayer.Infrastructure;

namespace SVSPlayer.Features.Games;

public interface IGamesService
{
    Task<IEnumerable<GameEntity>> GetAllGamesAsync();
    Task<GameEntity?> GetGameAsync(string rowKey);
    Task AddOrUpdateGameAsync(GameEntity game);
    Task AddGamesAsync(IEnumerable<GameEntity> games);
    Task DeleteGameAsync(string rowKey);
    Task<IEnumerable<GameEntity>> GetWithFilterAsync(string filter);
    Task<GameEntity?> GetCurrentAsync();
    Task<IEnumerable<GameEntity>> GetCurrentGamesAsync();
}

public class GamesService : IGamesService
{
    private readonly ILogger<GamesService> _logger;
    private readonly IRepository<GameEntity> _repository;

    private const string KEY = "GAMES";

    public GamesService(ILogger<GamesService> logger, IRepository<GameEntity> repository)
    {
        _logger = logger;
        _repository = repository;

        _repository.SetPartitionKey(KEY);
    }

    public async Task<GameEntity?> GetCurrentAsync()
    {
        var m = DateTime.Now.FirstDayOfWeek(DayOfWeek.Monday).Date.ToUniversalTime();
        var s = m.AddDays(8);

        return await Task.Run(() =>
        {

            var t = _repository
                .TableClient
                .Query<GameEntity>()
                .First(x => x.Start >= m
                            && x.Start <= s
                            && x.PartitionKey == _repository.GetPartitionKey());
            return t;
        });
    }

    public async Task<IEnumerable<GameEntity>> GetCurrentGamesAsync()
    {
        var y1 = DateTime.Now.Month >= 7 ? DateTime.Now.Year : DateTime.Now.Year - 1;
        var y2 = DateTime.Now.Month >= 7 ? DateTime.Now.Year + 1 : DateTime.Now.Year;

        var s = new DateTime(y1, 7, 1).Date.ToUniversalTime();
        var e = new DateTime(y2, 7, 1).Date.ToUniversalTime();

        return await Task.Run(() =>
        {
            var t = _repository
                .TableClient
                .Query<GameEntity>()
                .Where(x => x.PartitionKey == _repository.GetPartitionKey()
                            && x.Start >= s && x.Start < e)
                .OrderBy(x => x.Start)
                .ToList();

            return t;
        });
    }

    public async Task<IEnumerable<GameEntity>> GetAllGamesAsync() => await _repository.GetAllAsync();

    public async Task<GameEntity?> GetGameAsync(string rowKey) => await _repository.GetByKeyAsync(rowKey);

    public async Task AddOrUpdateGameAsync(GameEntity game) => await _repository.AddOrUpdateAsync(game);

    public async Task<IEnumerable<GameEntity>> GetWithFilterAsync(string filter) => await _repository.GetWithFilterAsync(filter);

    public async Task AddGamesAsync(IEnumerable<GameEntity> games) => await _repository.AddMultipleAsync(games);

    public async Task DeleteGameAsync(string rowKey) => await _repository.RemoveByKeyAsync(rowKey);
}