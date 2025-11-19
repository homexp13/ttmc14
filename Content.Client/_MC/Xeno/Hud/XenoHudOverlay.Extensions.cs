using System.Numerics;
using Content.Shared._MC.Xeno.Sunder;
using Content.Shared.Rounding;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Utility;
using Content.Shared._RMC14.Xenonids;

// ReSharper disable once CheckNamespace
namespace Content.Client._RMC14.Xenonids.Hud;

public sealed partial class XenoHudOverlay
{
    private readonly EntityQuery<MCXenoSunderComponent> _mcXenoSunderQuery;

    private void UpdateSunder(Entity<XenoComponent, SpriteComponent> entity, DrawingHandleWorld handle)
    {
        var (uid, xeno, sprite) = entity;
        if (!_mcXenoSunderQuery.TryComp(uid, out var sunderComponent))
            return;

        var level = ContentHelpers.RoundToLevels(sunderComponent.Value, 100, 11);
        var name = level > 0 ? $"{level * 10}" : "0";
        var state = $"xenoarmor{name}";
        var icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), state);
        var texture = _sprite.GetFrame(icon, _timing.CurTime);

        var bounds = sprite.Bounds;
        var yOffset = (bounds.Height + sprite.Offset.Y) / 2f - (float) texture.Height / EyeManager.PixelsPerMeter * bounds.Height + xeno.HudOffset.Y;
        var xOffset = (bounds.Width + sprite.Offset.X) / 2f - (float) texture.Width / EyeManager.PixelsPerMeter * bounds.Width + xeno.HudOffset.X;

        var position = new Vector2(xOffset, yOffset);
        handle.DrawTexture(texture, position);
    }
}
