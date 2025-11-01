using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Offensive
{
    public class PhysicalSkill : ISkill
    {
        private readonly int _power;
        private readonly HitRange _hitRange;
        private readonly DamageCalculator _damageCalculator;

        public string Name { get; }
        public int Cost { get; }
        public TargetType TargetType { get; }
        public Element Element => Element.Phys;

        public PhysicalSkill(
            string name,
            int cost,
            int power,
            TargetType targetType,
            HitRange hitRange,
            DamageCalculator damageCalculator)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Cost = cost;
            _power = power;
            TargetType = targetType;
            _hitRange = hitRange ?? throw new ArgumentNullException(nameof(hitRange));
            _damageCalculator = damageCalculator ?? throw new ArgumentNullException(nameof(damageCalculator));
        }

        public bool CanExecute(Unit user, GameState gameState)
        {
            return user.IsAlive() && user.CurrentStats.HasSufficientMP(Cost);
        }

        public SkillResult Execute(Unit user, List<Unit> targets, GameState gameState)
        {
            user.ConsumeMP(Cost);

            var hits = _hitRange.CalculateHits(gameState.GetCurrentPlayerSkillCount());
            gameState.IncrementSkillCount();
            
            var effects = new List<SkillEffect>();
            var highestPriorityAffinity = Affinity.Neutral;

            foreach (var target in targets)
            {
                if (!target.IsAlive())
                {
                    continue;
                }

                // Ejecutar TODOS los hits sobre este target
                for (int i = 0; i < hits; i++)
                {
                    var effect = ExecuteSingleHit(user, target);
                    effects.Add(effect);

                    if (GetAffinityPriority(effect.AffinityResult) > GetAffinityPriority(highestPriorityAffinity))
                    {
                        highestPriorityAffinity = effect.AffinityResult;
                    }

                    // NO detenerse si el target muere, continuar golpeando
                    // (según los ejemplos del enunciado)
                }
            }

            var turnConsumption = CalculateTurnConsumption(highestPriorityAffinity);
            return new SkillResult(effects, turnConsumption, new List<string>());
        }

        private SkillEffect ExecuteSingleHit(Unit user, Unit target)
        {
            var baseDamage = CalculatePhysicalSkillDamage(user);
            var affinity = target.Affinities.GetAffinity(Element.Phys);
            var finalDamage = ApplyAffinityMultiplier(baseDamage, affinity);

            if (affinity == Affinity.Null)
            {
                return new SkillEffect(
                    target.Name,
                    0,
                    0,
                    false,
                    affinity,
                    target.CurrentStats.CurrentHP,
                    target.CurrentStats.MaxHP,
                    Element.Phys
                );
            }

            if (affinity == Affinity.Repel)
            {
                user.TakeDamage(finalDamage);
                return new SkillEffect(
                    target.Name,
                    finalDamage,
                    0,
                    false,
                    affinity,
                    user.CurrentStats.CurrentHP,
                    user.CurrentStats.MaxHP,
                    Element.Phys
                );
            }

            if (affinity == Affinity.Drain)
            {
                target.Heal(finalDamage);
                return new SkillEffect(
                    target.Name,
                    0,
                    finalDamage,
                    false,
                    affinity,
                    target.CurrentStats.CurrentHP,
                    target.CurrentStats.MaxHP,
                    Element.Phys
                );
            }

            target.TakeDamage(finalDamage);
            var died = !target.IsAlive();

            return new SkillEffect(
                target.Name,
                finalDamage,
                0,
                died,
                affinity,
                target.CurrentStats.CurrentHP,
                target.CurrentStats.MaxHP,
                Element.Phys
            );
        }

        private double CalculatePhysicalSkillDamage(Unit attacker)
        {
            var str = attacker.CurrentStats.Str;
            var damage = Math.Sqrt(str * _power);
            return damage;
        }

        private int ApplyAffinityMultiplier(double baseDamage, Affinity affinity)
        {
            var multiplier = affinity switch
            {
                Affinity.Weak => 1.5,
                Affinity.Resist => 0.5,
                Affinity.Null => 0.0,
                Affinity.Repel => 1.0,
                Affinity.Drain => 1.0,
                _ => 1.0
            };

            return (int)Math.Floor(baseDamage * multiplier);
        }

        private TurnConsumption CalculateTurnConsumption(Affinity affinity)
        {
            return affinity switch
            {
                Affinity.Weak => TurnConsumption.Weak(),
                Affinity.Resist => TurnConsumption.NeutralOrResist(),
                Affinity.Null => TurnConsumption.Null(),
                Affinity.Repel => TurnConsumption.RepelOrDrain(),
                Affinity.Drain => TurnConsumption.RepelOrDrain(),
                _ => TurnConsumption.NeutralOrResist()
            };
        }

        private int GetAffinityPriority(Affinity affinity)
        {
            return affinity switch
            {
                Affinity.Repel => 6,
                Affinity.Drain => 6,
                Affinity.Null => 5,
                Affinity.Weak => 3,
                Affinity.Neutral => 1,
                Affinity.Resist => 1,
                _ => 0
            };
        }
    }
}