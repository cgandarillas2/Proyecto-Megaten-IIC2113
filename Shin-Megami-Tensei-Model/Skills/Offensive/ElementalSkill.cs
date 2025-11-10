using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Offensive
{
    public class ElementalSkill : OffensiveSkill
    {
        private readonly Element _element;

        public override Element Element => _element;

        public ElementalSkill(
            string name,
            int cost,
            int power,
            Element element,
            TargetType targetType,
            HitRange hitRange,
            DamageCalculator damageCalculator)
            : base(name, cost, power, targetType, hitRange, damageCalculator)
        {
            _element = element;
        }

        protected override double CalculateDamage(Unit attacker)
        {
            return _damageCalculator.CalculateMagicalDamage(attacker, _power);
        }
    }
}
