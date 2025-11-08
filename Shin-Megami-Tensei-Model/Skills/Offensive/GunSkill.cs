using System;
using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Offensive
{
    public class GunSkill : OffensiveSkill
    {
        public override Element Element => Element.Gun;

        public GunSkill(
            string name,
            int cost,
            int power,
            TargetType targetType,
            HitRange hitRange,
            DamageCalculator damageCalculator)
            : base(name, cost, power, targetType, hitRange, damageCalculator)
        {
        }

        protected override double CalculateDamage(Unit attacker)
        {
            var skl = attacker.CurrentStats.Skl;
            var damage = Math.Sqrt(skl * _power);
            return damage;
        }
    }
}
