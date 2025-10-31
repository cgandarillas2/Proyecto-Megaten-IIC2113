using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Action;

public class UseSkillAction: IAction
{
    private readonly ISkill _skill;

        public UseSkillAction(ISkill skill)
        {
            _skill = skill ?? throw new ArgumentNullException(nameof(skill));
        }

        public bool CanExecute(Unit actor, GameState gameState)
        {
            return _skill.CanExecute(actor, gameState);
        }

        public ActionResult Execute(Unit actor, Unit target, GameState gameState)
        {
            var targets = new List<Unit> { target };
            return ExecuteMultiTarget(actor, targets, gameState);
        }

        public ActionResult ExecuteMultiTarget(Unit actor, List<Unit> targets, GameState gameState)
        {
            var skillResult = _skill.Execute(actor, targets, gameState);
            
            var totalDamage = CalculateTotalDamage(skillResult);
            var highestPriorityAffinity = GetHighestPriorityAffinity(skillResult);
            
            return ActionResult.Successful(
                skillResult.TurnConsumption, 
                totalDamage, 
                highestPriorityAffinity
            );
        }

        public SkillResult ExecuteAndGetResult(Unit actor, List<Unit> targets, GameState gameState)
        {
            return _skill.Execute(actor, targets, gameState);
        }

        public ISkill GetSkill()
        {
            return _skill;
        }

        private int CalculateTotalDamage(SkillResult skillResult)
        {
            return skillResult.Effects.Sum(effect => effect.DamageDealt);
        }

        private Affinity GetHighestPriorityAffinity(SkillResult skillResult)
        {
            if (skillResult.Effects.Count == 0)
            {
                return Affinity.Neutral;
            }

            var highestPriority = Affinity.Neutral;
            var highestPriorityValue = 0;

            foreach (var effect in skillResult.Effects)
            {
                var priority = GetAffinityPriority(effect.AffinityResult);
                if (priority > highestPriorityValue)
                {
                    highestPriorityValue = priority;
                    highestPriority = effect.AffinityResult;
                }
            }

            return highestPriority;
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