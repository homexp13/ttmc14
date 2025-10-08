namespace Content.Shared._MC.Xeno.Biomass;

public partial class MCXenoBiomassSystem : EntitySystem
{
    public void AddBiomassValue(Entity<MCXenoBiomassComponent> entity, int value)
    {
        SetBiomassValue(entity, GetBiomassValue(entity) + value);
    }

    public void SetBiomassValue(Entity<MCXenoBiomassComponent> entity, int value)
    {
        entity.Comp.CurrentBiomass = value;
        Dirty(entity);
    }

    public int GetBiomassValue(Entity<MCXenoBiomassComponent> entity)
    {
        return entity.Comp.CurrentBiomass;
    }
}
