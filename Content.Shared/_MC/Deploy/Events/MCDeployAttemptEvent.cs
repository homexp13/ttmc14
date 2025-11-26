using Robust.Shared.Map;

namespace Content.Shared._MC.Deploy.Events;

[ByRefEvent]
public struct MCDeployAttemptEvent
{
    public readonly EntityCoordinates Coordinates;

    public bool Cancelled;

    public MCDeployAttemptEvent(EntityCoordinates coordinates)
    {
        Coordinates = coordinates;
    }
}
