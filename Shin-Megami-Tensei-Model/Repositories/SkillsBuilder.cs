using Shin_Megami_Tensei_Model.Repositories.Dtos;
using Shin_Megami_Tensei_Model.Skills;

namespace Shin_Megami_Tensei_Model.Repositories;

public class SkillsBuilder
{
    public ISkill BuildFromDto(SkillDto dto)
    {
        return new BaseSkill(
            name: dto.Name,
            cost: dto.Cost
        );
    }
}