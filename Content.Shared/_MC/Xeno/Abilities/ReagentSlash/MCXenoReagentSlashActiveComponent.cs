using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Abilities.ReagentSlash;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoReagentSlashActiveComponent : Component
{
    [AutoNetworkedField]
    public string Solution;

    [AutoNetworkedField]
    public TimeSpan ExpiresTime;

    [AutoNetworkedField]
    public int MaxCount;

    [AutoNetworkedField]
    public int Count;

    [AutoNetworkedField]
    public FixedPoint2 Amount;
}
