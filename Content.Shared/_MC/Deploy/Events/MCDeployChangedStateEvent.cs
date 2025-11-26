namespace Content.Shared._MC.Deploy.Events;

[ByRefEvent]
public readonly record struct MCDeployChangedStateEvent(MCDeployState State, MCDeployState PreviousState);
