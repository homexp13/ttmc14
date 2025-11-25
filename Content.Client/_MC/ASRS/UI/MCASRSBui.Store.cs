using Content.Shared._MC.ASRS.Components;

namespace Content.Client._MC.ASRS.UI;

public sealed partial class MCASRSBui
{
    private event Action? StoreRefreshed;

    public int StoreCost { get; private set; }
    public int StoreCount { get; private set; }

    public Dictionary<MCASRSEntry, int> Store { get; } = new();
    public bool StoreEmpty => Store.Count == 0;

    private void InitializeStore()
    {
        OrdersView.OrderCountChanged += StoreSetCount;
        PendingOrdersView.OrderCountChanged += StoreSetCount;
    }

    private void StoreSetCount(MCASRSEntry entry, int count)
    {
        if (count <= 0)
        {
            StoreRemove(entry);
            return;
        }

        Store[entry] = count;
        StoreRefresh();
    }

    private void StoreRemove(MCASRSEntry entry)
    {
        Store.Remove(entry);
        StoreRefresh();
    }

    private void StoreClear()
    {
        Store.Clear();
        StoreRefresh();
    }

    private void StoreRefresh()
    {
        StoreCost = 0;
        StoreCount = 0;

        foreach (var (entry, count) in Store)
        {
            StoreCost += entry.Cost * count;
            StoreCount += count;
        }

        StoreRefreshed?.Invoke();
    }
}
