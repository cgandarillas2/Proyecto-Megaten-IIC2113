using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Game;

public class Board
{
private const int TotalPositions = 4;
private const int SamuraiPosition = 0;

private readonly Unit[] _positions;

public Board(Samurai samurai, List<Monster> monsters)
{
    ValidateSamurai(samurai);
    
    _positions = new Unit[TotalPositions];
    _positions[SamuraiPosition] = samurai;
    
    PlaceMonstersOnBoard(monsters ?? new List<Monster>());
}

public Unit GetUnitAt(int position)
{
    ValidatePosition(position);
    return _positions[position];
}

public Unit GetSamurai()
{
    return GetUnitAt(SamuraiPosition);
}

public List<Unit> GetAllUnits()
{
    return new List<Unit>(_positions);
}

public List<Unit> GetAliveUnits()
{
    List<Unit> aliveUnits = new List<Unit>();

    for (int i = 0; i < _positions.Length; i++)
    {
        if (_positions[i].IsAlive())
        {
            aliveUnits.Add(_positions[i]);
        }
    }

    return aliveUnits;
}


public List<Unit> GetNonEmptyUnits()
{
    List<Unit> nonEmptyUnits = new List<Unit>();

    for (int i = 0; i < _positions.Length; i++)
    {
        if (!_positions[i].IsEmpty())
        {
            nonEmptyUnits.Add(_positions[i]);
        }
    }

    return nonEmptyUnits;
}

public bool HasAliveUnits()
{
    for (int i = 0; i < _positions.Length; i++)
    {
        if (_positions[i].IsAlive())
        {
            return true;
        }
    }

    return false;
}

public bool IsPositionEmpty(int position)
{
    ValidatePosition(position);
    return _positions[position].IsEmpty();
}

public void PlaceUnit(Unit unit, int position)
{
    ValidatePosition(position);
    ValidateNotSamuraiPosition(position);
    ValidateUnit(unit);
    
    _positions[position] = unit;
}

public void RemoveUnit(Unit unit)
{
    for (int i = 1; i < TotalPositions; i++)
    {
        if (_positions[i] == unit)
        {
            _positions[i] = new NullUnit();
            return;
        }
    }
}

public List<Monster> RemoveDeadMonsters()
{
    var deadMonsters = new List<Monster>();
        
    for (int i = 1; i < TotalPositions; i++)
    {
        if (!_positions[i].IsAlive() && !_positions[i].IsEmpty() && _positions[i] is Monster monster)
        {
            deadMonsters.Add(monster);
            _positions[i] = new NullUnit();
        }
    }
        
    return deadMonsters;
}

private void PlaceMonstersOnBoard(List<Monster> monsters)
{
    for (int i = 1; i < TotalPositions; i++)
    {
        var monsterIndex = i - 1;
        
        _positions[i] = monsterIndex < monsters.Count
            ? monsters[monsterIndex]
            : new NullUnit();
    }
}

private static void ValidateSamurai(Samurai samurai)
{
    if (samurai == null)
    {
        throw new ArgumentNullException(nameof(samurai));
    }
}

private static void ValidateUnit(Unit unit)
{
    if (unit == null)
    {
        throw new ArgumentNullException(nameof(unit));
    }
}

private static void ValidatePosition(int position)
{
    if (position < 0 || position >= TotalPositions)
    {
        throw new ArgumentException(
            $"Position must be between 0 and {TotalPositions - 1}",
            nameof(position));
    }
}

private static void ValidateNotSamuraiPosition(int position)
{
    if (position == SamuraiPosition)
    {
        throw new InvalidOperationException(
            "Cannot modify Samurai position");
    }
}
}