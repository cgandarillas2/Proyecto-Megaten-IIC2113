using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class SkillResultView
{
    private readonly View _view;
    
    public SkillResultView(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }
    
    public void Present(Unit actor, SkillResult result)
    {
        var effectsByTarget = new List<EffectGroup>();
        for (int i = 0; i < result.Effects.Count; i++)
        {
            SkillEffect effect = result.Effects[i];
            Unit target = effect.Target;

            EffectGroup existingGroup = null;
            for (int j = 0; j < effectsByTarget.Count; j++)
            {
                if (effectsByTarget[j].Key == target)
                {
                    existingGroup = effectsByTarget[j];
                    break;
                }
            }

            if (existingGroup == null)
            {
                existingGroup = new EffectGroup(target);
                effectsByTarget.Add(existingGroup);
            }

            existingGroup.AddEffect(effect);
        }

        EffectGroup actorEffects = null;
        for (int i = 0; i < effectsByTarget.Count; i++)
        {
            if (effectsByTarget[i].Key == actor)
            {
                actorEffects = effectsByTarget[i];
                break;
            }
        }

        var otherEffects = new List<EffectGroup>();
        for (int i = 0; i < effectsByTarget.Count; i++)
        {
            if (effectsByTarget[i].Key != actor)
            {
                otherEffects.Add(effectsByTarget[i]);
            }
        }
        
        /*var orderedOtherEffects = OrderEffectsByBoardPosition(otherEffects, gameState);*/

        Unit lastRepelTarget = null;
        for (int i = otherEffects.Count - 1; i >= 0; i--)
        {
            bool hasRepel = false;
            var effects = otherEffects[i].GetEffects();
            for (int j = 0; j < effects.Count; j++)
            {
                if (effects[j].AffinityResult == Affinity.Repel)
                {
                    hasRepel = true;
                    break;
                }
            }

            if (hasRepel)
            {
                lastRepelTarget = otherEffects[i].Key;
                break;
            }
        }

        Unit lastDrainSkillTarget = null;
        for (int i = otherEffects.Count - 1; i >= 0; i--)
        {
            bool hasDrainEffect = false;
            var effects = otherEffects[i].GetEffects();
            for (int j = 0; j < effects.Count; j++)
            {
                if (effects[j].IsDrainEffect())
                {
                    hasDrainEffect = true;
                    break;
                }
            }

            if (hasDrainEffect)
            {
                lastDrainSkillTarget = otherEffects[i].Key;
                break;
            }
        }

        foreach (var targetGroup in otherEffects)
        {
            bool isLastRepelTarget = targetGroup.Key == lastRepelTarget;
            bool isLastDrainTarget = targetGroup.Key == lastDrainSkillTarget;
            DisplayTargetEffects(actor, targetGroup.Key.Name, targetGroup.GetEffects(), isLastRepelTarget, isLastDrainTarget);
        }

        if (actorEffects != null)
        {
            DisplayTargetEffects(actor, actorEffects.Key.Name, actorEffects.GetEffects(), false, false);
        }
        
        foreach (var message in result.Messages)
        {
            _view.WriteLine(message);
        }
    }

    private void DisplayTargetEffects(Unit actor, string targetName, SkillEffectsCollection effects, bool isLastRepelTarget, bool isLastDrainTarget)
    {
        if (effects.Count == 0)
        {
            return;
        }

        var lastEffect = effects[^1];
        if (lastEffect.IsHealEffect() || lastEffect.IsReviveEffect())
        {
            DisplayHealEffects(actor, targetName, effects, lastEffect);
            return;
        }

        if (lastEffect.IsDrainHealEffect())
        {
            DisplayDrainHealthEffect(actor, lastEffect);
            return;
        }
        
        if (lastEffect.IsDrainEffect())
        {
            DisplayDrainSkillEffects(actor, targetName, effects, isLastDrainTarget);
            return;
        }

        bool hasRepel = false;
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].AffinityResult == Affinity.Repel)
            {
                hasRepel = true;
                break;
            }
        }

        bool hasDrain = false;
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].AffinityResult == Affinity.Drain)
            {
                hasDrain = true;
                break;
            }
        }

        bool isAlmighty = false;
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].Element == Element.Almighty)
            {
                isAlmighty = true;
                break;
            }
        }

        if (isAlmighty)
        {
            DisplayAlmightyEffects(actor, targetName, effects, isLastRepelTarget);
            
        }

        if (hasRepel)
        {
            DisplayRepelEffects(actor, targetName, effects, isLastRepelTarget);
            return;
        }

        if (hasDrain)
        {
            DisplayDrainAffinityEffects(actor, targetName, effects);
            return;
        }
        
        foreach (var effect in effects)
        {
            if (!isAlmighty)
            {
                DisplaySingleHit(actor, targetName, effect);
            }
            
        }

        _view.WriteLine($"{targetName} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }

    private void DisplayHealEffects(Unit actor, string targetName, SkillEffectsCollection effects, SkillEffect lastEffect)
    {
        int totalHealing = 0;
        for (int i = 0; i < effects.Count; i++)
        {
            totalHealing += effects[i].HealingDone;
        }

        if (lastEffect.IsReviveEffect())
        {
            _view.WriteLine($"{actor.Name} revive a {targetName}");
        }
        else
        {
            _view.WriteLine($"{actor.Name} cura a {targetName}");
        }
        
        _view.WriteLine($"{targetName} recibe {totalHealing} de HP");
        _view.WriteLine($"{targetName} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }

    private void DisplayDrainHealthEffect(Unit actor, SkillEffect lastEffect)
    {
        _view.WriteLine($"{actor.Name} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }

    private void DisplayRepelEffects(Unit actor, string targetName, SkillEffectsCollection effects, bool isLastRepelTarget)
    {
        var lastEffect = effects[^1];
        
        foreach (var effect in effects)
        {
            var attackVerb = GetAttackVerbByElement(effect.Element);
            _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");
            
            if (effect.AffinityResult == Affinity.Repel && effect.IsInstantKill)
            {
                _view.WriteLine($"{targetName} devuelve el ataque a {actor.Name} y lo elimina");
                /*_view.WriteLine($"{actor.Name} termina con HP:{actor.CurrentStats.CurrentHP}/{actor.CurrentStats.MaxHP}");*/
            }
            else
            {
                _view.WriteLine($"{targetName} devuelve {effect.DamageDealt} daño a {actor.Name}");  
            }
            
            /*var finalHp = lastEffect.FinalHP;
            var maxHp = lastEffect.MaxHP;

            if (effect.AffinityResult == Affinity.Repel)
            {
                finalHp = actor.CurrentStats.CurrentHP;
                maxHp = actor.CurrentStats
            }*/
        }

        

        // Mostrar HP final del atacante solo si esta es la última unidad que repele
        if (isLastRepelTarget)
        {
            _view.WriteLine($"{actor.Name} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
        }
    }
    
    private void DisplayDrainSkillEffects(Unit actor, string targetName, SkillEffectsCollection effects, bool isLastTarget)
    {
        var lastEffect = effects[^1];

        foreach (var effect in effects)
        {
            // Siempre usar "lanza un ataque todo poderoso" para DrainSkills (son Almighty)
            _view.WriteLine($"{actor.Name} lanza un ataque todo poderoso a {targetName}");
            
            // Determinar qué stats se drenan basándose en el EffectType
            bool drainsHP = effect.EffectType == SkillEffectType.DrainHP || 
                           effect.EffectType == SkillEffectType.DrainBoth;
            bool drainsMP = effect.EffectType == SkillEffectType.DrainMP ||
                           effect.EffectType == SkillEffectType.DrainBoth;

            if (drainsHP)
            {
                _view.WriteLine($"El ataque drena {effect.HPDrained} HP de {targetName}");
                _view.WriteLine($"{targetName} termina con HP:{effect.FinalHP}/{effect.MaxHP}");
                
                // Mostrar HP del atacante solo si es la última unidad objetivo
                // NOTA: Requiere agregar ActorFinalHP y ActorMaxHP a SkillEffect
                if (isLastTarget)
                {
                    _view.WriteLine($"{actor.Name} termina con HP:{actor.CurrentStats.CurrentHP}/{actor.CurrentStats.MaxHP}");
                }
            }
            
            if (drainsMP)
            {
                _view.WriteLine($"El ataque drena {effect.MPDrained} MP de {targetName}");
                _view.WriteLine($"{targetName} termina con MP:{effect.FinalMP}/{effect.MaxMP}");
                
                // Mostrar MP del atacante solo si es la última unidad objetivo
                // NOTA: Requiere agregar ActorFinalMP y ActorMaxMP a SkillEffect
                if (isLastTarget)
                {
                    _view.WriteLine($"{actor.Name} termina con MP:{actor.CurrentStats.CurrentMP}/{actor.CurrentStats.MaxMP}");
                }
            }
        }
    }

    
    private void DisplayAlmightyEffects(Unit actor, string targetName, SkillEffectsCollection effects, bool isLastRepelTarget)
    {
        var lastEffect = effects[^1];

        foreach (var effect in effects)
        {
            var attackVerb = GetAttackVerbByElement(effect.Element);
            _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");
            _view.WriteLine($"{targetName} recibe {effect.DamageDealt} de daño");
        }

        // Mostrar HP final del atacante solo si esta es la última unidad que repele
        if (isLastRepelTarget)
        {
            _view.WriteLine($"{actor.Name} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
        }
    }

    private void DisplayDrainAffinityEffects(Unit actor, string targetName, SkillEffectsCollection effects)
    {
        var lastEffect = effects[^1];

        foreach (var effect in effects)
        {
            var attackVerb = GetAttackVerbByElement(effect.Element);
            _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");
            _view.WriteLine($"{targetName} absorbe {effect.HealingDone} daño");
        }

        // Mostrar HP final del target (solo una vez al final)
        _view.WriteLine($"{targetName} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }

    private void DisplaySingleHit(Unit actor, string targetName, SkillEffect effect)
    {
        var attackVerb = GetAttackVerbByElement(effect.Element);
        _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");

        if (effect.EffectType == SkillEffectType.InstantKill)
        {
            DisplayInstantKillSkillEffects(actor, targetName, effect);
        }
        else
        {
            DisplayAffinityResult(actor, targetName, effect);
            DisplayFinishMessage(targetName, effect);
        }
        
        
    }

    private void DisplayInstantKillSkillEffects(Unit actor, string targetName, SkillEffect effect)
    {
        var isInstaKillAndDied = effect.EffectType == SkillEffectType.InstantKill && effect.TargetDied;

        if (isInstaKillAndDied)
        {

            DisplayAffinityResult(actor, targetName, effect);
            _view.WriteLine($"{targetName} ha sido eliminado");
        }
        else
        {
            if (effect.AffinityResult == Affinity.Null)
            {
                _view.WriteLine($"{targetName} bloquea el ataque de {actor.Name}");
            }
            else
            {
                _view.WriteLine($"{actor.Name} ha fallado el ataque");
            }
            
        }
        
    }

    private void DisplayAffinityResult(Unit actor, string targetName, SkillEffect effect)
    {
        if (effect.AffinityResult == Affinity.Weak)
        {
            _view.WriteLine($"{targetName} es débil contra el ataque de {actor.Name}");
        }
        else if (effect.AffinityResult == Affinity.Resist)
        {
            _view.WriteLine($"{targetName} es resistente el ataque de {actor.Name}");
        }
        else if (effect.AffinityResult == Affinity.Null)
        {
            _view.WriteLine($"{targetName} bloquea el ataque de {actor.Name}");
        }
    }

    private void DisplayFinishMessage(string targetName, SkillEffect effect)
    {
        if (effect.AffinityResult != Affinity.Null && effect.DamageDealt > 0)
        {
            _view.WriteLine($"{targetName} recibe {effect.DamageDealt} de daño");
        }
    }

    private string GetAttackVerbByElement(Element element)
    {
        return element switch
        {
            Element.Phys => "ataca a",
            Element.Gun => "dispara a",
            Element.Fire => "lanza fuego a",
            Element.Ice => "lanza hielo a",
            Element.Elec => "lanza electricidad a",
            Element.Force => "lanza viento a",
            Element.Light => "ataca con luz a",
            Element.Dark => "ataca con oscuridad a",
            Element.Almighty => "lanza un ataque todo poderoso a",
            _ => "ataca a"
        };
    }

    private class EffectGroup
    {
        public Unit Key { get; }
        private readonly SkillEffectsCollection _effects;

        public EffectGroup(Unit key)
        {
            Key = key;
            _effects = new SkillEffectsCollection();
        }

        public void AddEffect(SkillEffect effect)
        {
            _effects.Add(effect);
        }

        public SkillEffectsCollection GetEffects()
        {
            return _effects;
        }
    }
}