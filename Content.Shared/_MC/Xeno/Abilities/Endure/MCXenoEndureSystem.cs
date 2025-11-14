using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Network;

namespace Content.Shared._MC.Xeno.Abilities.Endure;

public sealed class MCXenoEndureSystem : MCXenoAbilitySystem
{
    [Dependency] private readonly INetManager _net = null!;
    [Dependency] private readonly MobStateSystem _mobState = null!;
    [Dependency] private readonly SharedAuraSystem _rmcAura = null!;
    [Dependency] private readonly SharedRMCEmoteSystem _rmcEmote = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCXenoEndureComponent, MCXenoEndureActionEvent>(OnAction);

        SubscribeLocalEvent<MCXenoEndureActiveComponent, ComponentRemove>(OnActiveRemove);
        SubscribeLocalEvent<MCXenoEndureActiveComponent, UpdateMobStateEvent>(OnActiveUpdateMobState,
            after: [typeof(MobThresholdSystem), typeof(SharedXenoPheromonesSystem)]);
    }

    private void OnActiveRemove(Entity<MCXenoEndureActiveComponent> entity, ref ComponentRemove args)
    {
        _mobState.UpdateMobState(entity);
    }

    private void OnAction(Entity<MCXenoEndureComponent> entity, ref MCXenoEndureActionEvent args)
    {
        if (args.Handled)
            return;

        if (!RMCActions.TryUseAction(entity, args.Action, entity))
            return;

        args.Handled = true;

        _rmcAura.GiveAura(entity, entity.Comp.ActivationAuraColor, entity.Comp.Duration);
        _rmcEmote.TryEmoteWithChat(entity, entity.Comp.ActivationEmote);

        EnsureComp<MCXenoEndureActiveComponent>(entity);

        if (_net.IsClient)
            return;

        RemCompDeferredDelayed<MCXenoEndureActiveComponent>(entity, entity.Comp.Duration);
    }

    private void OnActiveUpdateMobState(Entity<MCXenoEndureActiveComponent> entity, ref UpdateMobStateEvent args)
    {
        if (args.Component.CurrentState == MobState.Dead || args.State == MobState.Dead)
            return;

        args.State = MobState.Alive;
    }
}
