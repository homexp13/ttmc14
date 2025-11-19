using Robust.Shared.GameStates;

namespace Content.Shared._MC.ASRS.Systems;

public sealed class MCASRSSystem : MCEntitySystemSingleton<MCASRSSingletonComponent>
{
    public override void Initialize()
    {
        base.Initialize();
    }
}

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCASRSSingletonComponent : Component
{
    [DataField, AutoNetworkedField]
    public int Points;
}
