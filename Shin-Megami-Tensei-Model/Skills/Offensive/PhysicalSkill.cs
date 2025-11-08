using System;
using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Offensive
{
    public class PhysicalSkill : OffensiveSkill
    {
        public override Element Element => Element.Phys;

        public PhysicalSkill(
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
            var str = attacker.CurrentStats.Str;
            var damage = Math.Sqrt(str * _power);
            return damage;
        }
    }
}
