using Content.Client._MC.ASRS.UI.Views;
using Content.Shared._MC.ASRS.Components;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._MC.ASRS.UI;

[UsedImplicitly]
public sealed partial class MCASRSBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [Dependency] private readonly IEntityManager _entity = null!;

    [ViewVariables]
    private MCASRSWindow _window = null!;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<MCASRSWindow>();

        if (!_entity.TryGetComponent<MCASRSConsoleComponent>(Owner, out var component))
            return;

        InitializeStore();

        InitializeViewCategory(component);
        InitializeViewPendingOrders();
        InitializeViewRequests();
    }

    private void OpenView(MCASRSView view)
    {
        _window.OpenView(this, view);
    }
}
