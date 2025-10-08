using Content.Shared._MC.Xeno.Biomass;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Physics.Systems;

namespace Content.Shared._MC.Xeno.Abilities.Psydrain;

public sealed class MCXenoPsydrainSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly RMCActionsSystem _rmcActions = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedRMCFlammableSystem _flammable = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedXenoHiveSystem _xenoHive = default!;
    [Dependency] private readonly XenoPlasmaSystem _xenoPlasma = default!;
    [Dependency] private readonly SharedJitteringSystem _jittering = default!;
    [Dependency] private readonly MCXenoBiomassSystem _mcXenoBiomassSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MCXenoPsydrainComponent, MCXenoPsydrainActionEvent>(OnAction);
        SubscribeLocalEvent<MCXenoPsydrainComponent, MCXenoPsydrainDoAfterEvent>(OnDoAfter);
    }

    private void OnAction(Entity<MCXenoPsydrainComponent> entity, ref MCXenoPsydrainActionEvent args)
    {
        var target = args.Target;
        var isHuman = HasComp<HumanoidAppearanceComponent>(target);

        if (!_rmcActions.TryUseAction(entity, args.Action, entity) || args.Handled)
            return;

        if (!_xenoPlasma.HasPlasmaPopup(entity.Owner, entity.Comp.PlasmaNeed))
            return;

        if (!TryComp<MobStateComponent>(target, out var mobState))
            return;

        if (!_xenoHive.HasHive(entity.Owner))
        {
            var dontHaveHive = Loc.GetString("psydrain-dont-have-hive");
            _popup.PopupEntity(dontHaveHive, entity, entity, PopupType.MediumXeno);
            return;
        }

        if (!isHuman)
        {
            var notHuman = Loc.GetString("psydrain-not-human");
            _popup.PopupEntity(notHuman, entity, entity, PopupType.MediumXeno);
            return;
        }

        if (mobState.PsyDrained)
        {
            var someoneDrained = Loc.GetString("someone-already-psydrained");
            _popup.PopupEntity(someoneDrained, entity, entity, PopupType.MediumXeno);
            return;
        }

        if (!_mobState.IsDead(target))
        {
            var notDead = Loc.GetString("psydrain-not-dead");
            _popup.PopupEntity(notDead, entity, entity, PopupType.MediumXeno);
            return;
        }

        if (_flammable.IsOnFire(entity.Owner))
        {
            var ourFire = Loc.GetString("psydrain-our-fire");
            _popup.PopupEntity(ourFire, entity, entity, PopupType.MediumXeno);
            return;
        }

        args.Handled = true;

        var ev = new MCXenoPsydrainDoAfterEvent();
        var doAfter = new DoAfterArgs(EntityManager, entity, entity.Comp.Delay, ev, entity, target)
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            BlockDuplicate = true,
            CancelDuplicate = true,
            RequireCanInteract = true,
            BreakOnRest = true
        };

        var userPopup = Loc.GetString("being-psydrained", ("entity", entity), ("target", target));

        _popup.PopupEntity(userPopup, entity, entity, PopupType.MediumXeno);
        _audio.PlayPvs(entity.Comp.SoundDrain, entity);

        if (!_doAfter.TryStartDoAfter(doAfter))
        {
            var cancelDoAfterOwner = Loc.GetString("doAfter-canceled-owner");

            _popup.PopupEntity(cancelDoAfterOwner, entity, entity, PopupType.MediumXeno);
            _audio.Stop(entity);
        }
    }

    private void OnDoAfter(Entity<MCXenoPsydrainComponent> entity, ref MCXenoPsydrainDoAfterEvent args)
    {
        if (args.Target is not { } target)
            return;

        if (args.Handled)
            return;

        if (!TryComp<MobStateComponent>(target, out var mobState) || !TryComp<MCXenoBiomassComponent>(entity, out var biomass))
            return;

        if (mobState.PsyDrained)
        {
            var someoneDrained = Loc.GetString("someone-already-psydrained");
            _popup.PopupEntity(someoneDrained, entity, entity, PopupType.MediumXeno);
            return;
        }

        args.Handled = true;
        _audio.PlayLocal(entity.Comp.SoundDrainEnd, entity, entity);

        var userPopup = Loc.GetString("end-drain-owner", ("target", target));
        _popup.PopupEntity(userPopup, entity, entity, PopupType.MediumXeno);

        _jittering.DoJitter(entity, entity.Comp.JitteringDelayOwner, true, entity.Comp.AmplitudeOwner, entity.Comp.FrequencyOwner);
        _jittering.DoJitter(target, entity.Comp.JitteringDelayTarget, true, entity.Comp.AmplitudeTarget, entity.Comp.FrequencyTarget);

        _damageable.TryChangeDamage(target, entity.Comp.CloneDamage);
        mobState.PsyDrained = true;

        var biomassEntity = (target, biomass);
        _xenoHive.AddLarvaPointsOwner(entity, entity.Comp.LarvaPointsGain);
        _xenoPlasma.TryRemovePlasma(entity.Owner, entity.Comp.PlasmaNeed);
        _xenoHive.AddPsypointsFromOwner(entity, entity.Comp.PsypointType, entity.Comp.PsypointGain);
        _mcXenoBiomassSystem.AddBiomassValue(biomassEntity, entity.Comp.BiomassGain);

        _adminLogger.Add(LogType.Action,
            LogImpact.Medium,
            $"{ToPrettyString(entity.Owner):player} successfully used Psy Drain on {ToPrettyString(target):target} " +
            $"at {Transform(target).Coordinates:coordinates}. " +
            $"Larva points gained: {entity.Comp.LarvaPointsGain}, " +
            $"Psy points gained: {entity.Comp.PsypointGain}, " +
            $"Damage applied: {entity.Comp.CloneDamage}");
    }
}
