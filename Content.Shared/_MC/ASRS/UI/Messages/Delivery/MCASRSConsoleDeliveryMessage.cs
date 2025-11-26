using Robust.Shared.Serialization;

namespace Content.Shared._MC.ASRS.UI.Messages.Delivery;

[Serializable, NetSerializable]
public sealed class MCASRSConsoleDeliveryMessage : BoundUserInterfaceMessage
{
    public readonly MCASRSRequest Request;
    public readonly NetEntity BeaconUid;

    public MCASRSConsoleDeliveryMessage(MCASRSRequest request, NetEntity beaconUid)
    {
        Request = request;
        BeaconUid = beaconUid;
    }
}
