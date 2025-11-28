using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.FixedPoint;

namespace Content.Shared._MC.Xeno.Plasma.Systems;

public sealed class MCXenoPlasmaSystem : EntitySystem
{
    [Dependency] private readonly XenoPlasmaSystem _xenoPlasma = null!;

    private EntityQuery<XenoPlasmaComponent> _query;

    public override void Initialize()
    {
        base.Initialize();

        _query = GetEntityQuery<XenoPlasmaComponent>();
    }

    public void RegenPlasma(EntityUid entity, float amount)
    {
        if (!_query.TryComp(entity, out var component))
            return;

        _xenoPlasma.RegenPlasma((entity, component), amount);
    }

    public void RegenPlasma(Entity<XenoPlasmaComponent?> entity, float amount)
    {
        if (!_query.Resolve(entity, ref entity.Comp, false))
            return;

        _xenoPlasma.RegenPlasma(entity, amount);
    }

    public void RemovePlasma(EntityUid uid, float amount)
    {
        if (!_query.TryComp(uid, out var component))
            return;

        _xenoPlasma.RemovePlasma((uid, component), amount);
    }

    public void RemovePlasma(Entity<XenoPlasmaComponent?> entity, float amount)
    {
        if (!_query.Resolve(entity, ref entity.Comp, false))
            return;

        _xenoPlasma.RemovePlasma((entity, entity.Comp), amount);
    }

    public bool TryRemovePlasma(EntityUid uid, float amount)
    {
        if (!_query.TryComp(uid, out var component))
            return false;

        var previous = component.Plasma;
        if (previous == FixedPoint2.Zero)
            return false;

        RemovePlasma((uid, component), amount);
        return previous >= amount;
    }

    public object GetPlasma(EntityUid uid)
    {
        return !_query.TryComp(uid, out var component)
            ? 0
            : component.Plasma;
    }

    public float GetMaxPlasma(EntityUid uid)
    {
        return !_query.TryComp(uid, out var component)
            ? 0
            : component.MaxPlasma;
    }

    public float GetPlasmaNormalized(EntityUid uid)
    {
        if (!_query.TryComp(uid, out var component))
            return 0;

        if (component.MaxPlasma == 0)
            return 0;

        var plasma = component.Plasma;
        return float.Clamp(plasma.Float() / component.MaxPlasma, 0, 1);
    }
}
