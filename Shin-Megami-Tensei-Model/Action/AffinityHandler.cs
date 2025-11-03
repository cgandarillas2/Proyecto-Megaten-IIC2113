using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Action;

/// <summary>
/// Maneja la lógica relacionada con afinidades en el combate:
/// - Aplicación de daño según afinidad (Null, Repel, Drain, Normal)
/// - Cálculo de multiplicadores de daño
/// - Determinación del consumo de turnos
/// </summary>
/// 
public class AffinityHandler
{
    public void ApplyDamageByAffinity(Unit attacker, Unit target, int damage, Affinity affinity)
    {
        switch (affinity)
        {
            case Affinity.Null:
                // No damage
                return;
                
            case Affinity.Repel:
                attacker.TakeDamage(damage);
                return;
                
            case Affinity.Drain:
                target.Heal(damage);
                return;
                
            default:
                target.TakeDamage(damage);
                return;
        }
    }
    
    public int ApplyAffinityMultiplier(double baseDamage, Affinity affinity)
    {
        var multiplier = GetAffinityMultiplier(affinity);
        return (int)Math.Floor(baseDamage * multiplier);
    }

    public TurnConsumption CalculateTurnConsumption(Affinity affinity)
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

    private double GetAffinityMultiplier(Affinity affinity)
    {
        return affinity switch
        {
            Affinity.Weak => 1.5,
            Affinity.Resist => 0.5,
            Affinity.Null => 0.0,
            Affinity.Repel => 1.0,
            Affinity.Drain => 1.0,
            _ => 1.0
        };
    }
}