using Content.Shared.Radio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._MC.Sentries;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCSentryComponent : Component
{
    [DataField, AutoNetworkedField]
    public MCSentryState State = MCSentryState.Item;

    [DataField, AutoNetworkedField]
    public bool RadialMode;

    [DataField, AutoNetworkedField]
    public TimeSpan DeployTime = TimeSpan.FromSeconds(10);

    [DataField, AutoNetworkedField]
    public string? DeployFixture = "sentry";

    [DataField, AutoNetworkedField]
    public TimeSpan AlertDamageNextTime;

    [DataField, AutoNetworkedField]
    public TimeSpan AlertDamageDelay = TimeSpan.FromSeconds(4);

    [DataField, AutoNetworkedField]
    public TimeSpan AlertNextTime;

    [DataField, AutoNetworkedField]
    public TimeSpan AlertDelay = TimeSpan.FromSeconds(20);

    [DataField, AutoNetworkedField]
    public ProtoId<RadioChannelPrototype> AlertChannel = "MarineCommon";

    [DataField, AutoNetworkedField]
    public bool AlertMode = true;
}

[Serializable, NetSerializable]
public enum MCSentryState
{
    Item,
    Deployed,
}

[Serializable, NetSerializable]
public enum MCSentryLayers
{
    Layer,
}
