using SVSPlayer.Infrastructure;

namespace SVSPlayer.Features.Games.Models;

public class GameEntity : EntityBase
{
    public string Opponent { get; set; } = "";

    public bool AtHome { get; set; }

    public DateTimeOffset Start { get; set; }
}