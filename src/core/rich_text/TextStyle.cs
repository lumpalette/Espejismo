using Godot;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a set of style properties that describe how text is rendered.
/// </summary>
/// <remarks>
/// All properties in this struct are nullable; a <see langword="null"/> value indicates that the property must be
/// resolved elsewhere, following a cascade-type resolution of styles.
/// </remarks>
public readonly record struct TextStyle
{
    /// <summary>
    /// Creates a new <see cref="TextStyle"/> instance with all properties unset.
    /// </summary>
    public TextStyle()
    {
    }

    /// <summary>
    /// Gets the font family used for the text.
    /// </summary>
    /// <remarks>
    /// The actual <see cref="Godot.Font"/> used for the text is determined by <see cref="FontStyle"/>.
    /// </remarks>
    public FontFamily? Font { get; init; }

    /// <summary>
    /// Gets the style variant used to select the font from the family.
    /// </summary>
    public FontStyle? FontStyle { get; init; }

    /// <summary>
    /// Gets the size of the text, in pixels.
    /// </summary>
    public ushort? FontSize { get; init; }

    /// <summary>
    /// Gets the color tint of the text.
    /// </summary>
    public Color? Color { get; init; }

    /// <summary>
    /// Gets the additional space added between letters (X) and lines of text (Y), in pixels.
    /// </summary>
    public Vector2I? Spacing { get; init; }

    /// <summary>
    /// Gets the visual effect applied to the text.
    /// </summary>
    public TextEffect? Effect { get; init; }

    /// <summary>
    /// Gets the size of the shadow effect, in pixels.
    /// </summary>
    public ushort? ShadowSize { get; init; }

    /// <summary>
    /// Gets the color for the shadow effect.
    /// </summary>
    public Color? ShadowColor { get; init; }

    /// <summary>
    /// Gets the displacement for the shadow effect, relative to the main text.
    /// </summary>
    public Vector2? ShadowOffset { get; init; }

    /// <summary>
    /// Gets the size for the text outline, in pixels.
    /// </summary>
    public ushort? OutlineSize { get; init; }

    /// <summary>
    /// Gets the color for the text outline.
    /// </summary>
    public Color? OutlineColor { get; init; }

    /// <summary>
    /// Creates a new <see cref="TextStyle"/> by filling the unset properties of this style with those of another.
    /// </summary>
    /// <param name="other">The style that supplies values for the unset properties.</param>
    /// <returns>
    /// A <see cref="TextStyle"/> that takes each property from this style if it is set, otherwise from
    /// <paramref name="other"/>.
    /// </returns>
    public TextStyle MergedWith(in TextStyle other)
    {
        return new TextStyle
        {
            Font = Font ?? other.Font,
            FontSize = FontSize ?? other.FontSize,
            FontStyle = FontStyle ?? other.FontStyle,
            Color = Color ?? other.Color,
            Effect = Effect ?? other.Effect,
            Spacing = Spacing ?? other.Spacing,
            ShadowSize = ShadowSize ?? other.ShadowSize,
            ShadowColor = ShadowColor ?? other.ShadowColor,
            ShadowOffset = ShadowOffset ?? other.ShadowOffset,
            OutlineSize = OutlineSize ?? other.OutlineSize,
            OutlineColor = OutlineColor ?? other.OutlineColor
        };
    }
}
