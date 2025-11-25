using Content.Shared._RMC14.Marines.Roles.Ranks;

namespace Content.Shared._MC.ASRS.Systems;

public sealed partial class MCASRSConsoleSystem
{
    [Dependency] private readonly SharedRankSystem _rmcRank = null!;

    private string GetRequesterName(EntityUid userUid)
    {
        return _rmcRank.GetSpeakerFullRankName(userUid) ?? Name(userUid);
    }
}
