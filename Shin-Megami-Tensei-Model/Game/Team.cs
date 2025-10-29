using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Game;

public class Team
{
    private readonly List<Monster> _reserve;

    public string PlayerName { get; }
    public Board ActiveBoard { get; }

    public Team(string playerName, Samurai leader, List<Monster> monsters)
    {
        PlayerName = ValidatePlayerName(playerName);
            
        var monstersCopy = CopyMonsters(monsters);
        var boardMonsters = ExtractBoardMonsters(monstersCopy);
        var reserveMonsters = ExtractReserveMonsters(monstersCopy);

        ActiveBoard = new Board(leader, boardMonsters);
        _reserve = reserveMonsters;
    }

    public List<Monster> GetReserveMonsters()
    {
        return new List<Monster>(_reserve);
    }

    public List<Monster> GetAliveReserveMonsters()
    {
        return _reserve.Where(m => m.IsAlive()).ToList();
    }

    public bool HasAliveUnitsOnBoard()
    {
        return ActiveBoard.HasAliveUnits();
    }

    public void RemoveDeadMonstersFromBoard()
    {
        ActiveBoard.RemoveDeadMonsters();
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
        return monsters != null 
            ? new List<Monster>(monsters) 
            : new List<Monster>();
    }

    private static List<Monster> ExtractBoardMonsters(List<Monster> monsters)
    {
        return monsters.Take(3).ToList();
    }

    private static List<Monster> ExtractReserveMonsters(List<Monster> monsters)
    {
        return monsters.Skip(3).ToList();
    }
}