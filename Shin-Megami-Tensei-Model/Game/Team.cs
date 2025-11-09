using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_Model.Collections;

namespace Shin_Megami_Tensei_Model.Game;

public class Team
{
    private readonly List<Monster> _reserve;
    private readonly List<Monster> _originalOrderMonsters;

    public string PlayerName { get; }
    public Board ActiveBoard { get; }
    public int SkillCount { get; private set; }

    public Team(string playerName, Samurai leader, IEnumerable<Monster> monsters)
    {
        PlayerName = playerName;

        _originalOrderMonsters = CopyMonsters(monsters).ToList();

        var boardMonsters = ExtractBoardMonsters(_originalOrderMonsters);
        var reserveMonsters = ExtractReserveMonsters(_originalOrderMonsters);

        ActiveBoard = new Board(leader, boardMonsters);
        _reserve = reserveMonsters.ToList();

        SkillCount = 0;
    }

    public UnitsCollection GetDeadReserveMonsters()
    {
        List<Monster> deadMonsters = new List<Monster>();

        for (int i = 0; i < _reserve.Count; i++)
        {
            if (!_reserve[i].IsAlive())
            {
                deadMonsters.Add(_reserve[i]);
            }
        }

        return new UnitsCollection(deadMonsters);
    }

    public UnitsCollection GetAllReserveMonsters()
    {
        ReorderReserveFromSelectionFile();
        return new UnitsCollection(_reserve);
    }

    public UnitsCollection GetReserveMonstersAsUnits()
    {
        ReorderReserveFromSelectionFile();
        return new UnitsCollection(_reserve);
    }

    public UnitsCollection GetAliveReserveMonsters()
    {
        List<Monster> aliveMonsters = new List<Monster>();

        for (int i = 0; i < _reserve.Count; i++)
        {
            if (_reserve[i].IsAlive())
            {
                aliveMonsters.Add(_reserve[i]);
            }
        }

        return new UnitsCollection(aliveMonsters);
    }

    public bool HasAliveUnitsOnBoard()
    {
        return ActiveBoard.HasAliveUnits();
    }
    
    public void AddMonsterToReserve(Monster monster)
    {
        bool monsterNotNullAndNotInReserve = monster != null && !_reserve.Contains(monster);
        if (monsterNotNullAndNotInReserve)
        {
            _reserve.Add(monster);
        }
    }

    public void RemoveMonsterFromReserve(Monster monster)
    {
        _reserve.Remove(monster);
    }

    public void RemoveDeadMonstersFromBoard()
    {
        var deadMonsters = ActiveBoard.RemoveDeadMonsters();
            
        foreach (var monster in deadMonsters)
        {
            AddMonsterToReserve((Monster)monster);
        }
    }

    public void ReorderReserveFromSelectionFile()
    {
        _reserve.Sort((a, b) =>
        {
            int indexA = _originalOrderMonsters.IndexOf(a);
            int indexB = _originalOrderMonsters.IndexOf(b);
            return indexA.CompareTo(indexB);
        });
    }

    public void IncrementSkillCount()
    {
        SkillCount++;
    }
    

    private static IReadOnlyList<Monster> CopyMonsters(IEnumerable<Monster> monsters)
    {
        return monsters?.ToList() ?? new List<Monster>();
    }

    private static IEnumerable<Monster> ExtractBoardMonsters(IReadOnlyList<Monster> monsters)
    {
        List<Monster> boardMonsters = new List<Monster>();
        int count = Math.Min(3, monsters.Count);

        for (int i = 0; i < count; i++)
        {
            boardMonsters.Add(monsters[i]);
        }

        return boardMonsters;
    }

    private static IReadOnlyList<Monster> ExtractReserveMonsters(IReadOnlyList<Monster> monsters)
    {
        List<Monster> reserveMonsters = new List<Monster>();

        for (int i = 3; i < monsters.Count; i++)
        {
            reserveMonsters.Add(monsters[i]);
        }

        return reserveMonsters;
    }
}