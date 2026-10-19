using Godot;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a set of style properties associated with a group of <see cref="Glyph"/> instances.
/// </summary>
public class GlyphStyle
{
    internal GlyphStyle(
        Color color,
        TextEffect? effect,
        ushort shadowSize,
        Color shadowColor,
        Vector2 shadowOffset,
        ushort outlineSize,
        Color outlineColor)
    {
        Color = color;
        Effect = effect;
        ShadowSize = shadowSize;
        ShadowColor = shadowColor;
        ShadowOffset = shadowOffset;
        OutlineSize = outlineSize;
        OutlineColor = outlineColor;
    }

    /// <summary>
    /// Gets the color tint applied to the glyphs.
    /// </summary>
    public Color Color { get; }

    /// <summary>
    /// Gets the visual effect applied to the glyphs.
    /// </summary>
    public TextEffect? Effect { get; }

    /// <summary>
    /// Gets the size of the shadow effect, in pixels.
    /// </summary>
    public ushort ShadowSize { get; }

    /// <summary>
    /// Gets the color of the shadow effect.
    /// </summary>
    public Color ShadowColor { get; }

    /// <summary>
    /// Gets the displacement applied to the shadow effect, relative to the glyph position.
    /// </summary>
    public Vector2 ShadowOffset { get; }

    /// <summary>
    /// Gets the size of the glyph outline, in pixels.
    /// </summary>
    public ushort OutlineSize { get; }

    /// <summary>
    /// Gets the color of the glyph outline.
    /// </summary>
    public Color OutlineColor { get; }
}
