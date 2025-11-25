using Robust.Shared.Serialization;

namespace Content.Shared._MC.ASRS.Ui;

[Serializable, NetSerializable]
public sealed class MCASRSConsoleBuiState : BoundUserInterfaceState
{
    public readonly int Points;

    public readonly List<MCASRSRequest> Requests;
    public readonly List<MCASRSRequest> RequestsAwaitingDelivery;

    public readonly List<MCASRSRequest> RequestsApprovedHistory;
    public readonly List<MCASRSRequest> RequestsDeniedHistory;

    public MCASRSConsoleBuiState(int points,
        List<MCASRSRequest> requests,
        List<MCASRSRequest> requestsAwaitingDelivery,
        List<MCASRSRequest> requestsDeniedHistory,
        List<MCASRSRequest> requestsApprovedHistory)
    {
        Points = points;
        Requests = requests;
        RequestsAwaitingDelivery = requestsAwaitingDelivery;
        RequestsApprovedHistory = requestsApprovedHistory;
        RequestsDeniedHistory = requestsDeniedHistory;
    }
}
