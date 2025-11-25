using Content.Client._MC.ASRS.UI.Views;
using Content.Shared._MC.ASRS;
using Content.Shared._MC.ASRS.UI.Messages.Approve;
using Content.Shared._MC.ASRS.UI.Messages.Deny;

namespace Content.Client._MC.ASRS.UI;

public sealed partial class MCASRSBui
{
    private MCASRSRequestsView RequestsView => _window.RequestsView;
    private MCASRSRequestsView RequestsAwaitingDeliveryView => _window.RequestsAwaitingDeliveryView;
    private MCASRSRequestsView RequestsApprovedHistoryRequestsView => _window.RequestsApprovedHistoryView;
    private MCASRSRequestsView RequestsDenyHistoryRequestsView => _window.RequestsDeniedHistoryView;

    private void InitializeViewRequests()
    {
        RequestsAwaitingDeliveryView.Reinitialize(false);
        RequestsApprovedHistoryRequestsView.Reinitialize(false);
        RequestsDenyHistoryRequestsView.Reinitialize(false);

        RequestsView.ApprovedAll += SendApproveAll;
        RequestsView.DenyAll += SendDenyAll;
        RequestsView.Approved += SendApprove;
        RequestsView.Denied += SendDeny;

        StateRefreshed += RefreshRequests;
    }

    private void RefreshRequests()
    {
        RefreshRequests(RequestsView, Requests);
        RefreshRequests(RequestsAwaitingDeliveryView, RequestsAwaitingDelivery);
        RefreshRequests(RequestsApprovedHistoryRequestsView, RequestsApprovedHistory);
        RefreshRequests(RequestsDenyHistoryRequestsView, RequestsDeniedHistory);
    }

    private void RefreshRequests(MCASRSRequestsView view, IReadOnlyList<MCASRSRequest> requests)
    {
        view.Refresh(this, requests);
    }

    private void SendApprove(MCASRSRequest request)
    {
        SendMessage(new MCASRSConsoleApproveMessage(request));
    }

    private void SendDeny(MCASRSRequest request)
    {
        SendMessage(new MCASRSConsoleDenyMessage(request));
    }

    private void SendApproveAll()
    {
        SendMessage(new MCASRSConsoleApproveAllMessage());
    }

    private void SendDenyAll()
    {
        SendMessage(new MCASRSConsoleDenyAllMessage());
    }
}
