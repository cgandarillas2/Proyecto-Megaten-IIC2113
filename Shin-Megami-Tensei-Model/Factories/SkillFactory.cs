using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Repositories.Dtos;
using Shin_Megami_Tensei_Model.Repositories.Parsers;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Skills.Drain;
using Shin_Megami_Tensei_Model.Skills.Heal;
using Shin_Megami_Tensei_Model.Skills.InstantKill;
using Shin_Megami_Tensei_Model.Skills.Offensive;
using Shin_Megami_Tensei_Model.Skills.Special;
using Shin_Megami_Tensei_Model.Skills.Support;
using Shin_Megami_Tensei_Model.Stats;

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

        if (IsDrainSkill(dto.Name))
        {
            return CreateDrainSkill(dto, targetType, hitRange);
        }

        if (_typeParser.IsOffensiveType(dto.Type))
        {
            return CreateOffensiveSkill(dto, targetType, hitRange);
        }
        

        if (_typeParser.IsHealType(dto.Type))
        {
            return CreateHealSkill(dto, targetType, hitRange);
        }

        if (_typeParser.IsInstantKillType(dto.Type))
        {
            return CreateInstantSkill(dto, targetType, hitRange);
        }

        if (_typeParser.IsSupportType(dto.Type))
        {
            return CreateSupportSkill(dto, targetType, hitRange);
        }

        if (_typeParser.IsSpecialType(dto.Type))
        {
            return CreateSpecialSkill(dto, targetType, hitRange);
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
            Element.Phys => new PhysicalSkill(
                dto.Name,
                dto.Cost,
                dto.Power,
                targetType,
                hitRange,
                _damageCalculator),

            Element.Gun => new GunSkill(
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

    private ISkill CreateInstantSkill(SkillDto dto, TargetType targetType, HitRange hitRange)
    {
        var element = _typeParser.ParseElement(dto.Type);
        return new InstantKillSkill(
            dto.Name,
            dto.Cost,
            dto.Power,
            element,
            hitRange,
            targetType
        );
    }

    private ISkill CreateHealSkill(SkillDto dto, TargetType targetType, HitRange hitRange)
    {
        // TEMPORAL
        if (dto.Name == "Invitation")
        {
            return new InvitationSkill(
                dto.Name,
                dto.Cost,
                hitRange);
        }

        return dto.Name switch
        {
            "Recarm" or "Samarecarm" => new ReviveSkill(
                dto.Name,
                dto.Cost,
                dto.Power,
                targetType,
                hitRange),

            "Recarmdra" => new DrainHealSkill(
                dto.Name,
                dto.Cost,
                dto.Power,
                targetType,
                hitRange),

            _ => new HealSkill(
                dto.Name,
                dto.Cost,
                dto.Power,
                targetType,
                hitRange)
        };
    }

    private ISkill CreateSpecialSkill(SkillDto dto, TargetType targetType, HitRange hitRange)
    {
        return new SabbatmaSkill(
            dto.Name,
            dto.Cost,
            hitRange);
    }

    private ISkill CreateDrainSkill(SkillDto dto, TargetType targetType, HitRange hitRange)
    {
        var element = _typeParser.ParseElement(dto.Type);
        
        var drainType = dto.Name switch
        {
            "Life Drain" => DrainType.HP,
            "Spirit Drain" => DrainType.MP,
            _ => DrainType.Both
        };

        return new DrainSkill(
            dto.Name,
            dto.Cost,
            dto.Power,
            element,
            hitRange,
            targetType,
            drainType
        );
    }
    
    private bool IsDrainSkill(string skillName)
    {
        return skillName switch
        {
            "Life Drain" => true,
            "Spirit Drain" => true,
            "Energy Drain" => true,
            "Serpent of Sheol" => true,
            _ => false
        };
    }

    private ISkill CreateSupportSkill(SkillDto dto, TargetType targetType, HitRange hitRange)
    {
        var effectType = dto.Name switch
        {
            "Charge" or "Dark Energy" => SupportEffectType.ChargePhysical,
            "Concentrate" or "Gather Spirit Energy" => SupportEffectType.ChargeMagical,
            "Blood Ritual" => SupportEffectType.BloodRitual,
            _ => throw new ArgumentException($"Unknown support skill: {dto.Name}")
        };

        return new SupportSkill(
            dto.Name,
            dto.Cost,
            effectType,
            targetType,
            hitRange);
    }
}