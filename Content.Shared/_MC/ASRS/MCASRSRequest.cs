using Content.Shared._MC.ASRS.Components;
using Robust.Shared.Serialization;

namespace Content.Shared._MC.ASRS;

[DataDefinition, Serializable, NetSerializable]
public sealed partial class MCASRSRequest
{
    [DataField]
    public string Requester;

    [DataField]
    public string Reason;

    [DataField]
    public Dictionary<MCASRSEntry, int> Contents;
}
