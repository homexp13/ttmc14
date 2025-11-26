using Content.Shared._MC.Beacon.Components;

namespace Content.Shared._MC.Beacon.Events;

[ByRefEvent]
public record struct MCBeaconActiveChangedEvent(Entity<MCBeaconComponent> Entity, bool Added);
