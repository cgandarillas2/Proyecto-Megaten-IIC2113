using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Repositories.Dtos;
using Shin_Megami_Tensei_Model.Repositories.Parsers;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Skills.Heal;
using Shin_Megami_Tensei_Model.Skills.Offensive;

namespace Shin_Megami_Tensei_Model.Factories;

public class SkillFactory
{
    private readonly DamageCalculator _damageCalculator;
    private readonly SkillTypeParser _typeParser;
    private readonly TargetTypeParser _targetParser;
    private readonly HitRangeParser _hitParser;

    public SkillFactory(DamageCalculator damageCalculator)
    {
        _damageCalculator = damageCalculator ?? throw new ArgumentNullException(nameof(damageCalculator));
        _typeParser = new SkillTypeParser();
        _targetParser = new TargetTypeParser();
        _hitParser = new HitRangeParser();
    }

    public ISkill CreateSkill(SkillDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        var targetType = _targetParser.ParseTarget(dto.Target);
        var hitRange = _hitParser.ParseHits(dto.Hits);

        if (_typeParser.IsOffensiveType(dto.Type))
        {
            return CreateOffensiveSkill(dto, targetType, hitRange);
        }

        if (_typeParser.IsHealType(dto.Type))
        {
            // TEMPORAL: HACEMOS QUE SE CREE OFFENSIVA
            /*return CreateHealSkill(dto, targetType);*/
            return CreateHealSkill(dto, targetType);
        }

        if (_typeParser.IsSupportType(dto.Type))
        {
            // TEMPORAL: HACEMOS QUE SE CREE OFFENSIVA
            /*return CreateSupportSkill(dto, targetType);*/
            return CreateOffensiveSkill(dto, targetType, hitRange);
        }

        // TEMPORAL: HACEMOS QUE SE CREE OFFENSIVA
        /*throw new ArgumentException($"Unknown skill type: {dto.Type}");*/
        return CreateOffensiveSkill(dto, targetType, hitRange);
    }

    private ISkill CreateOffensiveSkill(SkillDto dto, TargetType targetType, HitRange hitRange)
    {
        var element = _typeParser.ParseElement(dto.Type);

        return element switch
        {
            Stats.Element.Phys => new PhysicalSkill(
                dto.Name,
                dto.Cost,
                dto.Power,
                targetType,
                hitRange,
                _damageCalculator),

            Stats.Element.Gun => new GunSkill(
                dto.Name,
                dto.Cost,
                dto.Power,
                targetType,
                hitRange,
                _damageCalculator),

            _ => new ElementalSkill(
                dto.Name,
                dto.Cost,
                dto.Power,
                element,
                targetType,
                hitRange,
                _damageCalculator)
        };
    }

    private ISkill CreateHealSkill(SkillDto dto, TargetType targetType)
    {
        bool isRevive = dto.Name switch
        {
            "Recarm" => true,
            "Samarecarm" => true,
            _ => false
        };

        return new HealSkill(
            dto.Name,
            dto.Cost,
            dto.Power,
            targetType,
            isRevive
        );
    }

    private ISkill CreateSpecialSkill(SkillDto dto, TargetType targetType)
    {
        throw new NotImplementedException($"Support skill not implemented yet: {dto.Name}");
    }

    private ISkill CreateSupportSkill(SkillDto dto, TargetType targetType)
    {
        // TODO: Implementar en E4
        throw new NotImplementedException($"Support skill not implemented yet: {dto.Name}");
    }
}