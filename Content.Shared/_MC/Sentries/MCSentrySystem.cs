using Content.Shared._MC.Areas;
using Content.Shared._MC.Chat;
using Content.Shared._MC.Damage.Integrity.Systems;
using Content.Shared._MC.Sentries.Events;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.NPC;
using Content.Shared.Damage;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared._MC.Sentries;

public sealed class MCSentrySystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly FixtureSystem _fixture = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;

    [Dependency] private readonly MCAreasSystem _mcArea = default!;
    [Dependency] private readonly MCIntegritySystem _mcIntegrity = default!;
    [Dependency] private readonly MCSharedRadioSystem _mcRadio = default!;

    [Dependency] private readonly SharedRMCNPCSystem _rmcNpc = default!;
    [Dependency] private readonly RMCMapSystem _rmcMap = default!;

    private readonly HashSet<EntityUid> _toUpdate = new();

    private EntityQuery<MCSentryComponent> _sentryQuery;

    public override void Initialize()
    {
        base.Initialize();

        _sentryQuery = GetEntityQuery<MCSentryComponent>();

        SubscribeLocalEvent<MCSentryComponent, MapInitEvent>(OnMapInit);

        SubscribeLocalEvent<MCSentryComponent, PickupAttemptEvent>(OnPickupAttempt);
        SubscribeLocalEvent<MCSentryComponent, AttemptShootEvent>(OnAttemptShoot);
        SubscribeLocalEvent<MCSentryComponent, CombatModeShouldHandInteractEvent>(OnShouldInteract);
        SubscribeLocalEvent<MCSentryComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<MCSentryComponent, MCSentryDeployDoAfterEvent>(OnDeployDoAfter);

        SubscribeLocalEvent<MCSentryComponent, DamageChangedEvent>(OnDamageChanged);
        SubscribeLocalEvent<MCSentryComponent, DestructionEventArgs>(OnDestruction);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        foreach (var uid in _toUpdate)
        {
            if (!_sentryQuery.TryComp(uid, out var sentryComponent))
                continue;

            UpdateState((uid, sentryComponent));
        }

        _toUpdate.Clear();
    }

    private void OnMapInit(Entity<MCSentryComponent> ent, ref MapInitEvent args)
    {
        _toUpdate.Add(ent);

        UpdateState(ent);
    }

    private void OnPickupAttempt(Entity<MCSentryComponent> sentry, ref PickupAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        if (sentry.Comp.State != MCSentryState.Item)
            args.Cancel();
    }

    private void OnAttemptShoot(Entity<MCSentryComponent> ent, ref AttemptShootEvent args)
    {
        // Since the turret is folded and deployed as a single entity,
        // we prohibit shooting from the hands
        if (!args.Cancelled && args.User != ent.Owner)
            args.Cancelled = true;
    }

    private void OnShouldInteract(Entity<MCSentryComponent> ent, ref CombatModeShouldHandInteractEvent args)
    {
        args.Cancelled = true;
    }

    private void OnUseInHand(Entity<MCSentryComponent> sentry, ref UseInHandEvent args)
    {
        args.Handled = true;

        if (!CanDeployPopup(sentry, args.User, out _, out _))
            return;

        var ev = new MCSentryDeployDoAfterEvent();
        var delay = sentry.Comp.DeployTime;
        var doAfter = new DoAfterArgs(EntityManager, args.User, delay, ev, sentry)
        {
            BreakOnMove = true,
        };

        _doAfter.TryStartDoAfter(doAfter);
    }

    private void OnDeployDoAfter(Entity<MCSentryComponent> sentry, ref MCSentryDeployDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        args.Handled = true;

        if (!CanDeployPopup(sentry, args.User, out var coordinates, out var angle))
            return;

        sentry.Comp.State = MCSentryState.Deployed;
        Dirty(sentry);

        var xform = Transform(sentry);

        _transform.SetCoordinates(sentry, xform, coordinates, angle);
        _transform.AnchorEntity(sentry, xform);
        // _rmcInteraction.SetMaxRotation(sentry.Owner, angle, sentry.Comp.MaxDeviation);

        UpdateState(sentry);
    }

    private void OnDamageChanged(Entity<MCSentryComponent> entity, ref DamageChangedEvent args)
    {
        if (!entity.Comp.AlertMode)
            return;

        if (!args.DamageIncreased)
            return;

        if (entity.Comp.AlertDamageNextTime > _gameTiming.CurTime)
            return;

        entity.Comp.AlertDamageNextTime = _gameTiming.CurTime + entity.Comp.AlertDamageDelay;

        var message = Loc.GetString("mc-sentry-damage-alert",
            ("name", MetaData(entity).EntityName),
            ("coordsMessage", _mcArea.GetAreaCoordsMessage(entity)),
            ("integrityMessage", _mcIntegrity.GetDamageMessage(entity.Owner, "Destruction")));

        _mcRadio.SendRadioMessage(entity, message, entity.Comp.AlertChannel, entity);
    }

    private void OnDestruction(Entity<MCSentryComponent> entity, ref DestructionEventArgs args)
    {
        var message = Loc.GetString("mc-sentry-destroyed-alert", ("name", MetaData(entity).EntityName), ("coordsMessage", _mcArea.GetAreaCoordsMessage(entity)));
        _mcRadio.SendRadioMessage(entity, message, entity.Comp.AlertChannel, entity);
    }

    private void UpdateState(Entity<MCSentryComponent> sentry)
    {
        var fixture = sentry.Comp.DeployFixture is { } fixtureId && TryComp<FixturesComponent>(sentry, out var fixtures)
            ? _fixture.GetFixtureOrNull(sentry, fixtureId, fixtures)
            : null;

        switch (sentry.Comp.State)
        {
            case MCSentryState.Item:
                if (fixture is not null)
                    _physics.SetHard(sentry, fixture, false);

                _rmcNpc.SleepNPC(sentry);
                _appearance.SetData(sentry, MCSentryLayers.Layer, MCSentryState.Item);
                break;

            case MCSentryState.Deployed:
                if (fixture is not null)
                    _physics.SetHard(sentry, fixture, true);

                _rmcNpc.WakeNPC(sentry);
                _appearance.SetData(sentry, MCSentryLayers.Layer, MCSentryState.Deployed);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool CanDeployPopup(
        Entity<MCSentryComponent> sentry,
        EntityUid user,
        out EntityCoordinates coordinates,
        out Angle rotation)
    {
        coordinates = default;
        rotation = default;

        var moverCoordinates = _transform.GetMoverCoordinateRotation(user, Transform(user));
        coordinates = moverCoordinates.Coords;
        rotation = moverCoordinates.worldRot.GetCardinalDir().ToAngle();

        var direction = rotation.GetCardinalDir();
        coordinates = coordinates.Offset(direction.ToVec());

        if (_rmcMap.CanBuildOn(coordinates))
            return true;

        _popup.PopupClient(Loc.GetString("rmc-sentry-need-open-area", ("sentry", sentry)), user, user, PopupType.SmallCaution);
        return false;
    }
}
