using Content.Shared._MC.Beacon;
using Robust.Shared.Serialization;

namespace Content.Shared._MC.ASRS.Ui;

[Serializable, NetSerializable]
public sealed class MCASRSConsoleBuiState : BoundUserInterfaceState
{
    public readonly int Points;

    public readonly List<MCBeaconSystem.NetBeaconWithName> Beacons;

    public readonly List<MCASRSRequest> Requests;
    public readonly List<MCASRSRequest> RequestsAwaitingDelivery;

    public readonly List<MCASRSRequest> RequestsApprovedHistory;
    public readonly List<MCASRSRequest> RequestsDeniedHistory;

    public MCASRSConsoleBuiState(int points,
        List<MCBeaconSystem.NetBeaconWithName> beacons,
        List<MCASRSRequest> requests,
        List<MCASRSRequest> requestsAwaitingDelivery,
        List<MCASRSRequest> requestsDeniedHistory,
        List<MCASRSRequest> requestsApprovedHistory)
    {
        Points = points;
        Beacons = beacons;
        Requests = requests;
        RequestsAwaitingDelivery = requestsAwaitingDelivery;
        RequestsApprovedHistory = requestsApprovedHistory;
        RequestsDeniedHistory = requestsDeniedHistory;
    }
}
