using Content.Shared._MC.ASRS.Components;
using Content.Shared._MC.ASRS.Ui;
using Content.Shared._MC.ASRS.UI.Messages;
using Content.Shared._MC.ASRS.UI.Messages.Approve;
using Content.Shared._MC.ASRS.UI.Messages.Delivery;
using Content.Shared._MC.ASRS.UI.Messages.Deny;

namespace Content.Shared._MC.ASRS.Systems;

public sealed partial class MCASRSConsoleSystem
{
    private static readonly Enum UIKey = MCASRSConsoleUI.Key;

    [Dependency] private readonly SharedUserInterfaceSystem _userInterface = null!;

    private void InitializeUI()
    {
        SubscribeLocalEvent<MCASRSConsoleComponent, MCASRSConsoleStoreRequestsMessage>(OnRequestMessage);
        SubscribeLocalEvent<MCASRSConsoleComponent, MCASRSConsoleDeliveryMessage>(OnDeliveryMessage);
        SubscribeLocalEvent<MCASRSConsoleComponent, MCASRSConsoleApproveMessage>(OnApproveMessage);
        SubscribeLocalEvent<MCASRSConsoleComponent, MCASRSConsoleApproveAllMessage>(OnApproveAllMessage);
        SubscribeLocalEvent<MCASRSConsoleComponent, MCASRSConsoleDenyMessage>(OnDenyMessage);
        SubscribeLocalEvent<MCASRSConsoleComponent, MCASRSConsoleDenyAllMessage>(OnDenyAllMessage);

        OnRequestUpdated += RefreshUI;
    }

    private void OnRequestMessage(Entity<MCASRSConsoleComponent> entity, ref MCASRSConsoleStoreRequestsMessage args)
    {
        TryAddRequest(entity, args.ToRequest(GetRequesterName(args.Actor)));
    }

    private void OnDeliveryMessage(Entity<MCASRSConsoleComponent> entity, ref MCASRSConsoleDeliveryMessage args)
    {
        TryDelivery(entity, args.Request, args.BeaconUid);
    }

    private void OnApproveAllMessage(Entity<MCASRSConsoleComponent> entity, ref MCASRSConsoleApproveAllMessage args)
    {
        TryApprove(entity, entity.Comp.Requests);
    }

    private void OnApproveMessage(Entity<MCASRSConsoleComponent> entity, ref MCASRSConsoleApproveMessage args)
    {
        TryApprove(entity, args.Request);
    }

    private void OnDenyMessage(Entity<MCASRSConsoleComponent> entity, ref MCASRSConsoleDenyMessage args)
    {
        Deny(entity, args.Request);
    }

    private void OnDenyAllMessage(Entity<MCASRSConsoleComponent> entity, ref MCASRSConsoleDenyAllMessage args)
    {
        Deny(entity, entity.Comp.Requests);
    }

    private void RefreshUI(Entity<MCASRSConsoleComponent> entity)
    {
        _userInterface.SetUiState(entity.Owner, UIKey, GetUIState(entity));
    }

    private MCASRSConsoleBuiState GetUIState(Entity<MCASRSConsoleComponent> entity)
    {
        return new MCASRSConsoleBuiState(
            GetBalance(),
            _mcBeacon.GetNetActiveBeaconsWithName(entity.Comp.DeliveryCategory),
            entity.Comp.Requests,
            entity.Comp.RequestsAwaitingDelivery,
            entity.Comp.RequestsDenyHistory,
            entity.Comp.RequestsApprovedHistory);
    }
}
