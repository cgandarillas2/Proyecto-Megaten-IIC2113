using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Collections;

namespace Shin_Megami_Tensei_Model.Game.TargetFilters;

public class TargetFilterFactory
{
    public ITargetFilter CreateFilter(TargetType targetType, bool isReviveSkill = false, bool isDrainHeal = false)
    {
        return targetType switch
        {
            TargetType.Single => new AliveOpponentsFilter(),
            TargetType.All => new AliveOpponentsFilter(),
            TargetType.Multi => new AliveOpponentsFilter(),
            TargetType.Ally => isReviveSkill 
                ? new DeadAlliesFilter() 
                : new AliveAlliesFilter(),
            TargetType.Party =>  new PartyFilter(isDrainHeal),
            TargetType.Self => new SelfTargetFilter(),
            TargetType.Universal => new AllUnitsFilter(),
            _ => new AliveOpponentsFilter()
        };
    }
    
}