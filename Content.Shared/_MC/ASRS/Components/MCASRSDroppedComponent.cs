using Robust.Shared.GameStates;
using Robust.Shared.Map;

namespace Content.Shared._MC.ASRS.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCASRSDroppedComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityCoordinates TargetCoordinates;

    [DataField, AutoNetworkedField]
    public EntityUid? EffectUid;

    [DataField, AutoNetworkedField]
    public TimeSpan DropTime;
}
