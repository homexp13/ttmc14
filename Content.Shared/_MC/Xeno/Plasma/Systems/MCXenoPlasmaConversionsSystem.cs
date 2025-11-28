using Content.Shared._MC.Xeno.Plasma.Components.Conversions;
using Content.Shared.Damage;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;

namespace Content.Shared._MC.Xeno.Plasma.Systems;

public sealed class MCXenoPlasmaConversionsSystem : EntitySystem
{
    [Dependency] private readonly MCXenoPlasmaSystem _mcXenoPlasma = null!;

    private EntityQuery<MCXenoPlasmaOnAttackComponent> _query;

    public override void Initialize()
    {
        base.Initialize();

        _query = GetEntityQuery<MCXenoPlasmaOnAttackComponent>();

        SubscribeLocalEvent<MobStateComponent, DamageChangedEvent>(OnDamage);
        SubscribeLocalEvent<MCXenoPlasmaOnAttackedComponent, DamageChangedEvent>(OnDamaged);
    }

    private void OnDamage(Entity<MobStateComponent> ent, ref DamageChangedEvent args)
    {
        if (ent.Comp.CurrentState == MobState.Dead)
            return;

        if (args.Origin is not {} origin)
            return;

        if (!_query.TryComp(origin, out var component))
            return;

        var damage = args.DamageDelta?.GetTotal().Float() ?? 0;
        _mcXenoPlasma.RegenPlasma(origin, damage * component.Multiplier);
    }

    private void OnDamaged(Entity<MCXenoPlasmaOnAttackedComponent> ent, ref DamageChangedEvent args)
    {
        if (!args.DamageIncreased || args.DamageDelta is null)
            return;

        var damage = args.DamageDelta?.GetTotal().Float() ?? 0;
        _mcXenoPlasma.RegenPlasma(ent.Owner, damage * ent.Comp.Multiplier);
    }
}
