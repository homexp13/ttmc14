using System.Linq;
using Content.Shared._MC.ASRS.Events;

namespace Content.Shared._MC.ASRS.Systems;

public sealed partial class MCASRSConsoleSystem
{
    [Dependency] private readonly MCASRSSystem _mcAsrs = null!;

    private void InitializeBalance()
    {
        SubscribeLocalEvent<MCASRSBalanceChangedEvent>(OnBalanceChanged);
    }

    private void OnBalanceChanged(ref MCASRSBalanceChangedEvent ev)
    {
        RefreshAll(dirty: false);
    }

    private bool TryRemoveBalance(List<MCASRSRequest> requests)
    {
        return _mcAsrs.TryRemoveBalance(requests.Sum(request => request.TotalCost));
    }

    private bool TryRemoveBalance(MCASRSRequest request)
    {
        return _mcAsrs.TryRemoveBalance(request.TotalCost);
    }

    private int GetBalance()
    {
        return _mcAsrs.GetBalance();
    }
}
