using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._MC.Xeno.Abilities.ResinJelly.Events;

[Serializable, NetSerializable]
public sealed partial class MCXenoResinJellyConsumeDoAfterEvent : SimpleDoAfterEvent;
