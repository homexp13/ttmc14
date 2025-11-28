namespace Content.Shared._MC.Xeno.Abilities.TransferPlasma;

public sealed class MCXenoTransferPlasmaSystem : MCXenoAbilitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCXenoTransferPlasmaComponent, MCXenoTransferPlasmaAction>(OnAction);
    }

    private void OnAction(Entity<MCXenoTransferPlasmaComponent> entity, ref MCXenoTransferPlasmaAction args)
    {

    }
}
