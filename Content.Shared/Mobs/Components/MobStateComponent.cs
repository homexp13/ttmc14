using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared._MC.Xeno.Abilities.Psydrain;
using Robust.Shared.GameStates;

namespace Content.Shared.Mobs.Components
{
    /// <summary>
    ///     When attached to an <see cref="DamageableComponent"/>,
    ///     this component will handle critical and death behaviors for mobs.
    ///     Additionally, it handles sending effects to clients
    ///     (such as blur effect for unconsciousness) and managing the health HUD.
    /// </summary>
    [RegisterComponent]
    [NetworkedComponent]
    [AutoGenerateComponentState]
    [Access(typeof(MobStateSystem), typeof(MobThresholdSystem), typeof(MCXenoPsydrainSystem))]
    public sealed partial class MobStateComponent : Component
    {
        //default mobstate is always the lowest state level
        [AutoNetworkedField, ViewVariables]
        public MobState CurrentState { get; set; } = MobState.Alive;

        [DataField, AutoNetworkedField]
        public bool PsyDrained;

        [DataField]
        [AutoNetworkedField]
        public HashSet<MobState> AllowedStates = new()
            {
                MobState.Alive,
                MobState.Critical,
                MobState.Dead
            };
    }
}
