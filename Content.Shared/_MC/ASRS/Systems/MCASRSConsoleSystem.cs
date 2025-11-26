using System.Linq;
using Content.Shared._MC.ASRS.Components;

namespace Content.Shared._MC.ASRS.Systems;

public sealed partial class MCASRSConsoleSystem : EntitySystem
{
    [Dependency] private readonly MCASRSDropSystem _mcAsrsDrop = null!;

    private event Action<Entity<MCASRSConsoleComponent>>? OnRequestUpdated;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCASRSConsoleComponent, ComponentInit>(OnInit);

        InitializeBalance();
        InitializeBeacon();
        InitializeUI();
    }

    private void OnInit(Entity<MCASRSConsoleComponent> entity, ref ComponentInit args)
    {
        Cache(entity);
        Refresh(entity);
    }

    private void TryAddRequest(Entity<MCASRSConsoleComponent> entity, MCASRSRequest request)
    {
        if (entity.Comp.Requests.Count >= entity.Comp.RequestsLimit)
            return;

        if (!ValidateRequest(entity, request))
            return;

        entity.Comp.Requests.Add(request);
        Refresh(entity);
    }

    private void TryDelivery(Entity<MCASRSConsoleComponent> entity, MCASRSRequest request, NetEntity beaconUid)
    {
        if (!entity.Comp.RequestsAwaitingDelivery.Contains(request))
            return;

        if (!_mcBeacon.Active(beaconUid, entity.Comp.DeliveryCategory))
            return;

        Delivery(entity, request, GetEntity(beaconUid));
    }

    private void TryApprove(Entity<MCASRSConsoleComponent> entity, MCASRSRequest request)
    {
        if (!ContainsRequest(entity, request))
            return;

        if (!TryRemoveBalance(request))
            return;

        Approve(entity, request);
    }

    private void TryApprove(Entity<MCASRSConsoleComponent> entity, List<MCASRSRequest> requests)
    {
        if (!ContainsRequests(entity, requests))
            return;

        if (!TryRemoveBalance(requests))
            return;

        Approve(entity, requests);
    }

    private void Delivery(Entity<MCASRSConsoleComponent> entity, MCASRSRequest request, EntityUid beaconUid)
    {
        entity.Comp.RequestsAwaitingDelivery.Remove(request);
        Refresh(entity);

        _mcAsrsDrop.Drop(request, beaconUid);
    }

    private void Approve(Entity<MCASRSConsoleComponent> entity, MCASRSRequest request)
    {
        entity.Comp.Requests.Remove(request);
        entity.Comp.RequestsAwaitingDelivery.Add(request);
        HistoryWrite(entity.Comp.RequestsApprovedHistory, request, entity.Comp.RequestsHistoryLimit);
        Refresh(entity);
    }

    private void Approve(Entity<MCASRSConsoleComponent> entity, List<MCASRSRequest> requests)
    {
        foreach (var request in new List<MCASRSRequest>(requests))
        {
            Approve(entity, request);
        }
    }

    private void Deny(Entity<MCASRSConsoleComponent> entity, MCASRSRequest request)
    {
        entity.Comp.Requests.Remove(request);
        HistoryWrite(entity.Comp.RequestsDenyHistory, request, entity.Comp.RequestsHistoryLimit);
        Refresh(entity);
    }

    private void Deny(Entity<MCASRSConsoleComponent> entity, List<MCASRSRequest> requests)
    {
        foreach (var request in new List<MCASRSRequest>(requests))
        {
            Deny(entity, request);
        }
    }

    private void RefreshAll(bool dirty = true)
    {
        var query = EntityQueryEnumerator<MCASRSConsoleComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            Refresh((uid, component), dirty);
        }
    }

    private void Refresh(Entity<MCASRSConsoleComponent> entity, bool dirty = true)
    {
        if (dirty)
            Dirty(entity);

        OnRequestUpdated?.Invoke(entity);
    }

    private static void HistoryWrite(List<MCASRSRequest> container, MCASRSRequest element, int limit)
    {
        if (container.Count > limit)
        {
            container.RemoveAt(0);
            return;
        }

        container.Add(element);
    }

    private static bool ContainsRequest(Entity<MCASRSConsoleComponent> entity, MCASRSRequest request)
    {
        return entity.Comp.Requests.Contains(request);
    }

    private static bool ContainsRequests(Entity<MCASRSConsoleComponent> entity, List<MCASRSRequest> requests)
    {
        return entity.Comp.Requests.All(requests.Contains);
    }

    private static bool ValidateRequest(Entity<MCASRSConsoleComponent> entity, MCASRSRequest request)
    {
        return request.Reason != string.Empty && request.Contents.Keys.All(entry => entity.Comp.CachedEntries.Contains(entry));
    }

    private static void Cache(Entity<MCASRSConsoleComponent> entity)
    {
        entity.Comp.CachedEntries.Clear();
        foreach (var category in entity.Comp.Categories)
        {
            entity.Comp.CachedEntries.AddRange(category.Entries);
        }
    }
}
