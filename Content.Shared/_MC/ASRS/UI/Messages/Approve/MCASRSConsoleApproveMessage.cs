using Robust.Shared.Serialization;

namespace Content.Shared._MC.ASRS.UI.Messages.Approve;

[Serializable, NetSerializable]
public sealed class MCASRSConsoleApproveMessage : BoundUserInterfaceMessage
{
    public readonly MCASRSRequest Request;

    public MCASRSConsoleApproveMessage(MCASRSRequest request)
    {
        Request = request;
    }
}
