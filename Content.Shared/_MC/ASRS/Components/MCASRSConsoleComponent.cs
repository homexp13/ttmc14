using System.Linq;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._MC.ASRS.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCASRSConsoleComponent : Component
{
    [DataField, AutoNetworkedField, AlwaysPushInheritance]
    public List<MCASRSCategory> Categories = new();

    [DataField, AutoNetworkedField, AlwaysPushInheritance]
    public List<MCASRSRequest> Requests = new();

    [AutoNetworkedField]
    public List<MCASRSEntry> CachedEntries = new();
}

[DataDefinition, Serializable, NetSerializable]
public sealed partial class MCASRSCategory
{
    [DataField]
    public string Name = string.Empty;

    [DataField]
    public List<MCASRSEntry> Entries = new();
}

[DataDefinition, Serializable, NetSerializable]
public sealed partial class MCASRSEntry
{
    [DataField]
    public string? Name;

    [DataField]
    public int Cost;

    [DataField]
    public EntProtoId? Crate;

    [DataField]
    public List<EntProtoId> Entities = new();

    private bool Equals(MCASRSEntry other)
    {
        return Name == other.Name && Cost == other.Cost && Nullable.Equals(Crate, other.Crate) && Entities.SequenceEqual(other.Entities);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is MCASRSEntry other && Equals(other);
    }

    public override int GetHashCode()
    {
        // ReSharper disable NonReadonlyMemberInGetHashCode
        return HashCode.Combine(Name, Cost, Crate, Entities);
        // ReSharper restore NonReadonlyMemberInGetHashCode
    }
}
