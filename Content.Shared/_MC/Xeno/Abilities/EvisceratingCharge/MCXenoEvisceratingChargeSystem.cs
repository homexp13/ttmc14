using Robust.Shared.Physics.Components;

namespace Content.Shared._MC.Xeno.Abilities.EvisceratingCharge;

public sealed class MCXenoEvisceratingChargeSystem : MCXenoAbilitySystem<MCXenoEvisceratingChargeComponent, MCXenoEvisceratingChargeEvent>
{
    private EntityQuery<PhysicsComponent> _physicsQuery;

    public override void Initialize()
    {
        base.Initialize();

        _physicsQuery = GetEntityQuery<PhysicsComponent>();
    }

    protected override void OnUse(Entity<MCXenoEvisceratingChargeComponent> entity, ref MCXenoEvisceratingChargeEvent args)
    {

    }
}
