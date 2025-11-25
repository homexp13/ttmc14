using Content.Shared._MC.ASRS.Components;
using Robust.Shared.Serialization;

namespace Content.Shared._MC.ASRS.UI.Messages;

[Serializable, NetSerializable]
public sealed class MCASRSConsoleStoreRequestsMessage : BoundUserInterfaceMessage
{
    private readonly string _reason;
    private readonly Dictionary<MCASRSEntry, int> _contents;

    public MCASRSConsoleStoreRequestsMessage(string reason, Dictionary<MCASRSEntry, int> contents)
    {
        _reason = reason;
        _contents = contents;
    }

    public MCASRSRequest ToRequest(string requester)
    {
        return new MCASRSRequest(requester, _reason, _contents, GetTotalCost());
    }

    private int GetTotalCost()
    {
        var totalCost = 0;
        foreach (var (entry, count) in _contents)
        {
            totalCost += entry.Cost * count;
        }

        return totalCost;
    }
}
