using System;
using System.Collections.Generic;
using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Offensive
{
    public abstract class OffensiveSkill : ISkill
    {
        protected readonly int _power;
        protected readonly DamageCalculator _damageCalculator;
        private readonly AffinityHandler _affinityHandler;

        public string Name { get; }
        public int Cost { get; }
        public HitRange HitRange { get; }
        public TargetType TargetType { get; }
        public abstract Element Element { get; }

        protected OffensiveSkill(
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

        public SkillResult Execute(Unit user, UnitsCollection targets, GameState gameState)
        {
            user.ConsumeMP(Cost);

            var hits = HitRange.CalculateHits(gameState.GetCurrentPlayerSkillCount(), targetType: TargetType);
            gameState.IncrementSkillCount();

            var effects = new List<SkillEffect>();
            var highestPriorityAffinity = Affinity.Neutral;
            var isFirstHit = true;

            foreach (var target in targets)
            {
                for (int i = 0; i < hits; i++)
                {
                    var effect = ExecuteSingleHit(user, target, isFirstHit);
                    effects.Add(effect);

                    if (isFirstHit)
                    {
                        ConsumeChargeIfApplicable(user);
                        isFirstHit = false;
                    }

                    if (_affinityHandler.GetAffinityPriority(effect.AffinityResult) > _affinityHandler.GetAffinityPriority(highestPriorityAffinity))
                    {
                        highestPriorityAffinity = effect.AffinityResult;
                    }
                }
            }

            var turnConsumption = _affinityHandler.CalculateTurnConsumption(highestPriorityAffinity);
            return new SkillResult(new SkillEffectsCollection(effects), turnConsumption, StringCollection.Empty());
        }

        protected abstract double CalculateDamage(Unit attacker);

        private SkillEffect ExecuteSingleHit(Unit user, Unit target, bool isFirstHit)
        {
            var baseDamage = CalculateDamage(user);
            var affinity = target.Affinities.GetAffinity(Element);
            var damageAfterAffinity = _affinityHandler.ApplyAffinityMultiplier(baseDamage, affinity);

            var finalDamage = _damageCalculator.ApplyBuffMultipliers(
                damageAfterAffinity,
                user,
                target,
                Element,
                affinity,
                isFirstHit);

            return _affinityHandler.CreateSkillEffect(user, target, finalDamage, affinity, Element);
        }

        private void ConsumeChargeIfApplicable(Unit user)
        {
            var isPhysicalAttack = Element == Element.Phys || Element == Element.Gun;
            if (isPhysicalAttack && user.BuffState.IsPhysicalCharged)
            {
                user.ConsumePhysicalCharge();
            }

            var isMagicalAttack = Element == Element.Fire || Element == Element.Ice ||
                                 Element == Element.Elec || Element == Element.Force;
            if (isMagicalAttack && user.BuffState.IsMagicalCharged)
            {
                user.ConsumeMagicalCharge();
            }
        }
    }
}