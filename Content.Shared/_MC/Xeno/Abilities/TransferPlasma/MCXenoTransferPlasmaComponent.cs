using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Abilities.TransferPlasma;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoTransferPlasmaComponent : Component
{
    [DataField, AutoNetworkedField]
    public float Amount = 100f;

    [DataField, AutoNetworkedField]
    public TimeSpan Delay = TimeSpan.FromSeconds(2);

    [DataField, AutoNetworkedField]
    public float Range = 2;
}
