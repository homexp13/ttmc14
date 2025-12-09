using Content.Shared.Damage;
using Content.Shared.FixedPoint;

namespace Content.Shared._MC.Damage;

public static class MCDamageSpecifier
{
    public static float GetBrute(this DamageSpecifier damageSpecifier)
    {
        return damageSpecifier.DamageDict.GetValueOrDefault(MCDamageableSystem.DamageBruteId, FixedPoint2.Zero).Float();
    }
}
