using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Abilities.EvisceratingCharge;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoEvisceratingChargeComponent : Component
{
    [DataField, AutoNetworkedField]
    public float Distance = 4;

    [DataField, AutoNetworkedField]
    public int Speed = 25;
}
