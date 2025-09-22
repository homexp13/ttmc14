using Content.Shared._MC.Nuke.Components;
using Content.Shared._MC.Nuke.UI;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._MC.Nuke.UI;

[UsedImplicitly]
public sealed class MCNukeBui : BoundUserInterface
{
    [Dependency] private readonly IEntityManager _entities = default!;

    [ViewVariables]
    private MCNukeWindow? _window;

    public MCNukeBui(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<MCNukeWindow>();

        _window.AnchorButton.OnPressed += _ => SendMessage(new MCNukeAnchorBuiMessage());

        if (!_entities.TryGetComponent<MCNukeComponent>(Owner, out var component))
            return;

        SetTime(component.Time);
        SetReady(component.Ready);
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (_window is null)
            return;

        switch (state)
        {
            case MCNukeBuiState mainState:
                SetState(mainState);
                break;
        }
    }

    private void SetState(MCNukeBuiState state)
    {
        SetReady(state.Ready);
        SetAnchored(state.Anchored);
    }

    private void SetReady(bool value)
    {
        _window?.SetSettingsVisible(value);
    }

    private void SetAnchored(bool value)
    {
        _window?.SetSettingsAnchored(value);
    }

    private void SetTime(TimeSpan time)
    {
        _window?.SetTime(time);
    }
}
