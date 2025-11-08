using System;
using System.Collections.Generic;
using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Offensive
{
    public class GunSkill : ISkill
    {
        private readonly int _power;
        private readonly DamageCalculator _damageCalculator;
        private readonly AffinityHandler _affinityHandler;

        public string Name { get; }
        public int Cost { get; }
        public HitRange HitRange { get; }
        public TargetType TargetType { get; }
        public Element Element => Element.Gun;

        public GunSkill(
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
            HitRange = hitRange ?? throw new ArgumentNullException(nameof(hitRange));
            _damageCalculator = damageCalculator ?? throw new ArgumentNullException(nameof(damageCalculator));
            _affinityHandler = new AffinityHandler();
        }

        public bool CanExecute(Unit user, GameState gameState)
        {
            return user.IsAlive() && user.CurrentStats.HasSufficientMP(Cost);
        }

        public SkillResult Execute(Unit user, List<Unit> targets, GameState gameState)
        {
            user.ConsumeMP(Cost);

            var hits = HitRange.CalculateHits(gameState.GetCurrentPlayerSkillCount(), targetType:TargetType);

            var effects = new List<SkillEffect>();
            var highestPriorityAffinity = Affinity.Neutral;

            foreach (var target in targets)
            {
                for (int i = 0; i < hits; i++)
                {
                    var effect = ExecuteSingleHit(user, target);
                    effects.Add(effect);

                    if (_affinityHandler.GetAffinityPriority(effect.AffinityResult) > _affinityHandler.GetAffinityPriority(highestPriorityAffinity))
                    {
                        highestPriorityAffinity = effect.AffinityResult;
                    }
                }
            }

            gameState.IncrementSkillCount();
            var turnConsumption = _affinityHandler.CalculateTurnConsumption(highestPriorityAffinity);
            return new SkillResult(effects, turnConsumption, new List<string>());
        }

        private SkillEffect ExecuteSingleHit(Unit user, Unit target)
        {
            var baseDamage = CalculateGunSkillDamage(user);
            var affinity = target.Affinities.GetAffinity(Element.Gun);
            var finalDamage = _affinityHandler.ApplyAffinityMultiplier(baseDamage, affinity);

            if (affinity == Affinity.Null)
            {
                return BuildEffect(target, 0, affinity, target.CurrentStats.CurrentHP, target.CurrentStats.MaxHP, false, 0);
            }

            if (affinity == Affinity.Repel)
            {
                user.TakeDamage(finalDamage);
                return BuildEffect(target, finalDamage, affinity, user.CurrentStats.CurrentHP, user.CurrentStats.MaxHP, false, 0);
            }

            if (affinity == Affinity.Drain)
            {
                target.Heal(finalDamage);
                return BuildEffect(target, 0, affinity, target.CurrentStats.CurrentHP, target.CurrentStats.MaxHP, false, finalDamage);
            }

            target.TakeDamage(finalDamage);
            var died = !target.IsAlive();

            return BuildEffect(target, finalDamage, affinity, target.CurrentStats.CurrentHP, target.CurrentStats.MaxHP, died, 0);
        }

        private SkillEffect BuildEffect(Unit target, int damage, Affinity affinity, int finalHP, int maxHP, bool died, int healing)
        {
            var builder = new SkillEffectBuilder()
                .ForTarget(target)
                .WithAffinity(affinity)
                .WithFinalHP(finalHP, maxHP)
                .WithElement(Element.Gun)
                .AsOffensive()
                .TargetDied(died);

            if (healing > 0)
            {
                builder = builder.WithHealing(healing);
            }
            else
            {
                builder = builder.WithDamage(damage);
            }

            return builder.Build();
        }

        private double CalculateGunSkillDamage(Unit attacker)
        {
            var skl = attacker.CurrentStats.Skl;
            var damage = Math.Sqrt(skl * _power);
            return damage;
        }
    }
}
