using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Abilities.ReagentSlash;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoReagentSlashActiveComponent : Component
{
    [DataField, AutoNetworkedField]
    public string Solution;

    [DataField, AutoNetworkedField]
    public TimeSpan ExpiresTime;

    [DataField, AutoNetworkedField]
    public int Count;

    [DataField, AutoNetworkedField]
    public FixedPoint2 Amount;
}
