using Content.Client._MC.ASRS.UI.Views;
using Content.Shared._MC.ASRS.UI.Messages;

namespace Content.Client._MC.ASRS.UI;

public sealed partial class MCASRSBui
{
    private MCASRSPendingOrdersView PendingOrdersView => _window.PendingOrdersView;

    private void InitializeViewPendingOrders()
    {
        PendingOrdersView.Cleared += StoreClear;
        PendingOrdersView.Submitted += SendStoreRequestsMessage;

        StoreRefreshed += OnPendingOrdersStoreRefreshed;
    }

    private void SendStoreRequestsMessage(string reason)
    {
        SendMessage(new MCASRSConsoleStoreRequestsMessage(reason, Store));
        StoreClear();
    }

    private void OnPendingOrdersStoreRefreshed()
    {
        PendingOrdersView.Refresh(this);
    }
}
