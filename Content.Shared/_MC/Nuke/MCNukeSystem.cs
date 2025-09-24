using System.Linq;
using Content.Shared._MC.Nuke.Components;
using Content.Shared._MC.Nuke.UI;
using Robust.Shared.Containers;

namespace Content.Shared._MC.Nuke;

public sealed class MCNukeSystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _userInterface = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCNukeComponent, EntInsertedIntoContainerMessage>(OnRefreshReady);
        SubscribeLocalEvent<MCNukeComponent, EntRemovedFromContainerMessage>(OnRefreshReady);

        SubscribeLocalEvent<MCNukeComponent, MCNukeAnchorBuiMessage>(OnAnchorMessage);
    }

    private void OnAnchorMessage(Entity<MCNukeComponent> ent, ref MCNukeAnchorBuiMessage args)
    {
        if (!ent.Comp.Ready)
            return;

        var transform = Transform(ent);
        var value = !transform.Anchored;

        try
        {
            if (value)
            {
                _transform.AnchorEntity(ent);
                return;
            }

            _transform.Unanchor(ent);
        }
        finally
        {
            RefreshUi(ent);
        }
    }

    private void OnRefreshReady<T>(Entity<MCNukeComponent> ent, ref T _)
    {
        ent.Comp.Ready = IsReady(ent);
        Dirty(ent);

        RefreshUi(ent);
    }

    private bool IsReady(Entity<MCNukeComponent> ent)
    {
        return ent.Comp.Slots.All(e => _container.TryGetContainer(ent, e, out var container) && container.Count >= 1);
    }

    private void RefreshUi(Entity<MCNukeComponent> ent)
    {
        var transform = Transform(ent);
        _userInterface.SetUiState(ent.Owner, MCNukeUi.Key, new MCNukeBuiState(ent.Comp.Ready, transform.Anchored));
    }
}
