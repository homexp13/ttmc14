using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Weapon.Vali;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCWeaponValiComponent : Component
{
    [DataField, AutoNetworkedField]
    public List<ProtoId<ReagentPrototype>> AllowedReagents = new();
}
