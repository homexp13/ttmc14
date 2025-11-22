using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Damage;

public sealed class MCDamageableSystem : EntitySystem
{
    private readonly ProtoId<DamageTypePrototype> _damageBruteId = "MCBrute";
    private readonly ProtoId<DamageTypePrototype> _damageBurnId = "MCBurn";
    private readonly ProtoId<DamageTypePrototype> _damageToxinId = "MCToxin";
    private readonly ProtoId<DamageTypePrototype> _damageOxygenId = "MCOxygen";

    private DamageTypePrototype _damageBrute = null!;
    private DamageTypePrototype _damageBurn = null!;
    private DamageTypePrototype _damageToxin = null!;
    private DamageTypePrototype _damageOxygen = null!;

    [Dependency] private readonly IPrototypeManager _prototype = null!;
    [Dependency] private readonly DamageableSystem _damageable = null!;

    public override void Initialize()
    {
        base.Initialize();

        _damageBrute = _prototype.Index(_damageBruteId);
        _damageBurn = _prototype.Index(_damageBurnId);
        _damageToxin = _prototype.Index(_damageToxinId);
        _damageOxygen = _prototype.Index(_damageOxygenId);
    }

    public void AdjustToxLoss(EntityUid uid, float damage)
    {
        _damageable.TryChangeDamage(uid, new DamageSpecifier(_damageToxin, FixedPoint2.New(damage)), ignoreResistances: true);
    }

    public void AdjustOxyLoss(EntityUid uid, float damage)
    {
        _damageable.TryChangeDamage(uid, new DamageSpecifier(_damageOxygen, FixedPoint2.New(damage)), ignoreResistances: true);
    }
}
