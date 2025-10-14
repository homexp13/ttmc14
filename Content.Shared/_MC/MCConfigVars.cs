using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._MC;

[CVarDefs]
public sealed class MCConfigVars : CVars
{
    /**
     * Respawn
     */

    public static readonly CVarDef<float> MCRespawnMarinesActionCooldownMinutes =
        CVarDef.Create("mc.respawn_marines_action_delay_minutes", 10f, CVar.SERVER | CVar.REPLICATED);

    /**
     * Stamina
     */

    public static readonly CVarDef<bool> MCStaminaDamageOnRun =
        CVarDef.Create("mc.stamina_damage_on_run", false, CVar.SERVER | CVar.REPLICATED);
}
