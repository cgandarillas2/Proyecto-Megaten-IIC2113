using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Services;

/// <summary>
/// Service responsible for sorting and ordering targets for multi-hit skills.
/// Extracted from SkillController to keep algorithms in the model layer.
/// </summary>
public class TargetSorter
{
    public UnitsCollection ApplyMultiTargetSort(ISkill skill, UnitsCollection targets, int skillCount)
    {
        var targetsList = targets.ToList();
        int hits = skill.HitRange.CalculateHits(skillCount);
        int targetCount = targetsList.Count;
        int startIndex = skillCount % targetCount;

        bool isRightDirection = (startIndex % 2 == 0);

        var selectedTargets = new List<Unit>();
        int currentIndex = startIndex;

        selectedTargets.Add(targetsList[startIndex]);

        for (int step = 0; step < hits - 1; step++)
        {
            if (isRightDirection)
            {
                currentIndex = (currentIndex + 1) % targetCount;
            }
            else
            {
                currentIndex = (currentIndex - 1 + targetCount) % targetCount;
            }

            selectedTargets.Add(targetsList[currentIndex]);
        }

        return new UnitsCollection(selectedTargets);
    }

    public UnitsCollection OrderByBoardPosition(UnitsCollection targets, GameState gameState)
    {
        var opponentBoard = gameState.GetOpponent().ActiveBoard;
        var unitPositions = new Dictionary<Unit, int>();

        for (int position = 0; position < 4; position++)
        {
            var unit = opponentBoard.GetUnitAt(position);
            if (!unit.IsEmpty())
            {
                unitPositions[unit] = position;
            }
        }

        var sortedTargets = targets.ToList();

        for (int i = 0; i < sortedTargets.Count - 1; i++)
        {
            for (int j = i + 1; j < sortedTargets.Count; j++)
            {
                int posI = unitPositions.ContainsKey(sortedTargets[i])
                    ? unitPositions[sortedTargets[i]]
                    : int.MaxValue;
                int posJ = unitPositions.ContainsKey(sortedTargets[j])
                    ? unitPositions[sortedTargets[j]]
                    : int.MaxValue;

                if (posJ < posI)
                {
                    Unit temp = sortedTargets[i];
                    sortedTargets[i] = sortedTargets[j];
                    sortedTargets[j] = temp;
                }
            }
        }

        return new UnitsCollection(sortedTargets);
    }
}
