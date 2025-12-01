using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Chemistry.Effects.Reagents;

public sealed partial class MCReagentTransvitox : MCReagentEffect
{
    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return "";
    }

    protected override void Effect(EntityEffectReagentArgs args, Solution solution, ReagentPrototype reagent)
    {

    }

    protected override void GetDamage(EntityUid uid, Solution solution, ReagentPrototype reagent)
    {
        MCDamageable.AdjustToxLoss(uid, 10);
    }
}
