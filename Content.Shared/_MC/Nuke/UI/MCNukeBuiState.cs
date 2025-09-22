using Robust.Shared.Serialization;

namespace Content.Shared._MC.Nuke.UI;

[Serializable, NetSerializable]
public sealed class MCNukeBuiState : BoundUserInterfaceState
{
    public readonly bool Ready;
    public readonly bool Anchored;

    public MCNukeBuiState(bool ready, bool anchored)
    {
        Ready = ready;
        Anchored = anchored;
    }
}
