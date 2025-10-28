using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Utils;
namespace Shin_Megami_Tensei_Model.Units;

public class NullUnit: Unit
{
    private const string EmptyName = "Empty";

    public NullUnit() : base(EmptyName, CreateEmptyStats(), AffinitySet.CreateAllNeutral())
    {
    }

    public new bool IsAlive()
    {
        return false;
    }

    public override bool IsEmpty()
    {
        return true;
    }
    
    private static UnitStats CreateEmptyStats()
    {
        return new UnitStats(
            maxHP: 0,
            maxMP: 0,
            str: 0,
            skl: 0,
            mag: 0,
            spd: 0,
            lck: 0
        );
    }
}