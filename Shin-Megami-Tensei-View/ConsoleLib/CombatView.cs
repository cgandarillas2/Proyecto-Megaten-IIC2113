using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Stats;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class CombatView
{
    private readonly View _view;

    public CombatView(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void ShowRoundHeader(string samuraiName, string playerName)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Ronda de {samuraiName} ({playerName})");
    }

    public void ShowTurnConsumption(TurnConsumptionResult result)
    {
        _view.WriteSeparation();
        _view.WriteLine(
            $"Se han consumido {result.FullTurnsConsumed} Full Turn(s) " +
            $"y {result.BlinkingTurnsConsumed} Blinking Turn(s)");
        _view.WriteLine($"Se han obtenido {result.BlinkingTurnsGained} Blinking Turn(s)");
    }

    public void ShowSurrender(string actorName, string playerName)
    {
        _view.WriteSeparation();
        _view.WriteLine($"{actorName} ({playerName}) se rinde");
    }

    public void ShowSummonSuccess(string monsterName)
    {
        _view.WriteSeparation();
        _view.WriteLine($"{monsterName} ha sido invocado");
    }

    public void ShowAttackMessage(string attackerName, string targetName, string verb)
    {
        _view.WriteLine($"{attackerName} {verb} {targetName}");
    }

    public void ShowAffinityMessage(string targetName, Affinity affinity, string attackerName)
    {
        var message = affinity switch
        {
            Affinity.Weak => $"{targetName} es débil contra el ataque de {attackerName}",
            Affinity.Resist => $"{targetName} es resistente el ataque de {attackerName}",
            Affinity.Null => $"{targetName} bloquea el ataque de {attackerName}",
            _ => null
        };

        if (message != null)
        {
            _view.WriteLine(message);
        }
    }

    public void ShowDamageMessage(string targetName, int damage)
    {
        _view.WriteLine($"{targetName} recibe {damage} de daño");
    }

    public void ShowRepelMessage(string targetName, int damage, string attackerName)
    {
        _view.WriteLine($"{targetName} devuelve {damage} daño a {attackerName}");
    }

    public void ShowDrainMessage(string targetName, int damage)
    {
        _view.WriteLine($"{targetName} absorbe {damage} daño");
    }

    public void ShowFinalHP(string unitName, int currentHP, int maxHP)
    {
        _view.WriteLine($"{unitName} termina con HP:{currentHP}/{maxHP}");
    }
}