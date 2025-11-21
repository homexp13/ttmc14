using Content.Shared._MC.ASRS;
using Content.Shared._MC.ASRS.Components;
using Content.Shared._MC.ASRS.Ui;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._MC.ASRS.Ui;

[UsedImplicitly]
public sealed class MCASRSBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [Dependency] private readonly IEntityManager _entity = null!;

    [ViewVariables]
    private MCASRSWindow? _window;

    private readonly Dictionary<MCASRSEntry, int> _store = new();
    private readonly List<MCASRSRequest> _requests = new();
    private int _storeCount;
    private int _storeCost;

    private bool StoreEmpty => _store.Count == 0;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<MCASRSWindow>();

        if (!_entity.TryGetComponent<MCASRSConsoleComponent>(Owner, out var computer))
            return;

        foreach (var category in computer.Categories)
        {
            var categoryButton = new MCASRSCategoryButton();
            categoryButton.SetName(category.Name);
            categoryButton.Button.Disabled = category.Entries.Count == 0;

            categoryButton.Button.OnPressed += _ => OpenCategory(category);

            _window.CategoryView.CategoriesContainer.AddChild(categoryButton);
        }

        _window.CategoryView.PendingOrderButton.OnPressed += _ => OpenPendingOrders();
        _window.CategoryView.RequestsButton.OnPressed += _ => OpenRequest();

        _window.PendingOrdersView.SubmitRequestButton.OnPressed += _ => SendSubmitRequest();
        _window.PendingOrdersView.ClearCartButton.OnPressed += _ => ClearStore();
        _window.PendingOrdersView.ReasonBar.OnTextChanged += _ => RefreshStoreReason();
    }

    protected override void UpdateState(BoundUserInterfaceState uiState)
    {
        base.UpdateState(uiState);

        if (uiState is not MCASRSConsoleBuiState state)
            return;

        _requests.Clear();
        _requests.AddRange(state.Requests);
        RefreshRequests();
    }

    private void OpenPendingOrders()
    {
        if (_window is null)
            return;

        _window.OrdersView.Visible = false;
        _window.RequestsView.Visible = false;

        var view = _window.PendingOrdersView;
        view.Visible = true;
        view.Container.Children.Clear();

        foreach (var (entry, count) in _store)
        {
            var categoryButton = new MCASRSOrderButton();
            categoryButton.SetLabel(entry);
            categoryButton.SetCount(count);
            categoryButton.OnCountChanged += value => OnOrderCountChanged(entry, value);

            view.Container.Children.Add(categoryButton);
        }

        RefreshStoreReason();
    }

    private void OpenCategory(MCASRSCategory category)
    {
        if (_window is null)
            return;

        _window.PendingOrdersView.Visible = false;
        _window.RequestsView.Visible = false;

        var view = _window.OrdersView;
        view.Visible = true;
        view.CategoryNameLabel.SetMessage(category.Name);
        view.Container.Children.Clear();

        foreach (var entry in category.Entries)
        {
            var categoryButton = new MCASRSOrderButton();
            categoryButton.SetLabel(entry);
            categoryButton.SetCount(_store.GetValueOrDefault(entry));
            categoryButton.OnCountChanged += value => OnOrderCountChanged(entry, value);

            view.Container.Children.Add(categoryButton);
        }
    }

    private void OpenRequest()
    {
        if (_window is null)
            return;

        _window.OrdersView.Visible = false;
        _window.PendingOrdersView.Visible = false;

        var view = _window.RequestsView;
    }

    private void SendSubmitRequest()
    {
        if (_window is null)
            return;

        var reason = _window.PendingOrdersView.ReasonBar.Text;
        var request = new MCASRSConsoleSendRequestMessage(reason, _store);

        SendMessage(request);
        ClearStore();
    }

    private void OnOrderCountChanged(MCASRSEntry entry, int value)
    {
        try
        {
            if (value == 0)
            {
                _store.Remove(entry);
                return;
            }

            if (_store.TryAdd(entry, value))
                return;

            _store[entry] = value;
        }
        finally
        {
            RefreshStore();
        }
    }

    private void RefreshStore()
    {
        if (_window is null)
            return;

        _storeCost = 0;
        _storeCount = 0;
        foreach (var (entry, count) in _store)
        {
            _storeCost += entry.Cost * count;
            _storeCount += count;
        }

        _window.CategoryView.PendingOrderButton.Disabled = StoreEmpty;
        _window.CategoryView.CostLabel.SetMessage(_storeCost.ToString());
        _window.CategoryView.ItemsLabel.SetMessage(_storeCount.ToString());
    }

    private void RefreshStoreReason()
    {
        if (_window is null)
            return;

        _window.PendingOrdersView.SubmitRequestButton.Disabled = _window.PendingOrdersView.ReasonBar.Text == string.Empty;
    }

    private void RefreshRequests()
    {
        if (_window is null)
            return;

        _window.CategoryView.RequestsButton.Disabled = _requests.Count == 0;
    }

    private void ClearStore()
    {
        if (_window is null)
            return;

        _store.Clear();

        RefreshStore();
        OpenPendingOrders();
    }
}
