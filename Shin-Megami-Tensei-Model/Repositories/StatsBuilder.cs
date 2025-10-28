using Shin_Megami_Tensei_Model.Repositories.Dtos;
using Shin_Megami_Tensei_Model.Stats;

namespace Shin_Megami_Tensei_Model.Repositories;

public class StatsBuilder
{
    public UnitStats BuildFromDto(StatsDto dto)
    {
        return new UnitStats(
            maxHP: dto.HP,
            maxMP: dto.MP,
            str: dto.Str,
            skl: dto.Skl,
            mag: dto.Mag,
            spd: dto.Spd,
            lck: dto.Lck
        );
    }
}