using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Game;

public class Team
{
    private const int MaxBoardPositions = 3;
    private readonly List<Monster> _activeMonsters;
    private readonly List<Monster> _reserveMonsters;
    
    public string PlayerName { get; }
    public Samurai Leader { get; }
    
    public Team(string playerName, Samurai leader, List<Monster> monsters)
    {
        PlayerName = ValidatePlayerName(playerName);
        Leader = leader ?? throw new ArgumentNullException(nameof(leader));

        var monstersCopy = CopyMonsters(monsters);
        _activeMonsters = ExtractActiveMonstersFromList(monstersCopy);
        _reserveMonsters = ExtractReserveMonstersFromList(monstersCopy);
    }
    
    public List<Unit> GetAllBoardUnits()
    {
        var units = new List<Unit> { Leader };
        units.AddRange(_activeMonsters);
        return units;
    }

    public List<Unit> GetAliveBoardUnits()
    {
        return GetAllBoardUnits().Where(u => u.IsAlive()).ToList();
    }

    public List<Monster> GetReserveMonsters()
    {
        return new List<Monster>(_reserveMonsters);
    }

    public List<Monster> GetAliveReserveMonsters()
    {
        return _reserveMonsters.Where(m => m.IsAlive()).ToList();
    }
    
    public bool HasAnyAliveUnitOnBoard()
    {
        return GetAliveBoardUnits().Any();
    }

    private static string ValidatePlayerName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Player name cannot be empty");
        }
        return name;
    }
    
    private static List<Monster> CopyMonsters(List<Monster> monsters)
    {
        return monsters != null ? new List<Monster>(monsters) : new List<Monster>();
    }

    private static List<Monster> ExtractActiveMonstersFromList(List<Monster> monsters)
    {
        return monsters.Take(MaxBoardPositions).ToList();
    }

    private static List<Monster> ExtractReserveMonstersFromList(List<Monster> monsters)
    {
        return monsters.Skip(MaxBoardPositions).ToList();
    }
}