using Content.Shared._MC.Xeno.Abilities.ReagentSelector.UI;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Xeno.Abilities.ReagentSelector;

public sealed class MCXenoReagentSelectorSystem : MCXenoAbilitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _userInterface = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCXenoReagentSelectorComponent, MCXenoReagentSelectorBuiMsg>(OnSelectMessage);
        SubscribeLocalEvent<MCXenoReagentSelectorComponent, MCXenoReagentSelectorActionEvent>(OnAction);
    }

    public ProtoId<ReagentPrototype>? GetReagent(Entity<MCXenoReagentSelectorComponent?> entity)
    {
        return !Resolve(entity, ref entity.Comp)
            ? null
            : entity.Comp.SelectedEntry?.ReagentId;
    }

    private void OnSelectMessage(Entity<MCXenoReagentSelectorComponent> entity, ref MCXenoReagentSelectorBuiMsg args)
    {
        Select(entity, args.Id);
        _userInterface.CloseUi(entity.Owner, MCXenoReagentSelectorUI.Key, entity);
    }

    private void OnAction(Entity<MCXenoReagentSelectorComponent> entity, ref MCXenoReagentSelectorActionEvent args)
    {
        args.Handled = true;
        _userInterface.TryOpenUi(entity.Owner, MCXenoReagentSelectorUI.Key, entity);
    }

    private void Select(Entity<MCXenoReagentSelectorComponent> entity, string id)
    {
        if (!entity.Comp.Entries.TryGetValue(id, out var entry))
            return;

        entity.Comp.SelectedEntry = entry;
        DirtyField(entity, entity.Comp, nameof(MCXenoReagentSelectorComponent.SelectedEntry));

        foreach (var action in RMCActions.GetActionsWithEvent<MCXenoReagentSelectorActionEvent>(entity))
        {
            Actions.SetIcon(action.Owner, entry.Sprite);
        }
    }
}
