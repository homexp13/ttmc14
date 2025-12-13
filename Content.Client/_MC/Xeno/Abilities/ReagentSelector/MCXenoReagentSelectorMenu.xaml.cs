using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._MC.Xeno.Abilities.ReagentSelector;

public sealed partial class MCXenoReagentSelectorMenu : RadialMenu
{
    public MCXenoReagentSelectorMenu()
    {
        RobustXamlLoader.Load(this);
    }
}
