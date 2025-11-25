namespace Content.Shared._MC.ASRS.Events;

[ByRefEvent]
public record struct MCASRSBalanceChangedEvent(int Balance, int OldBalance);
