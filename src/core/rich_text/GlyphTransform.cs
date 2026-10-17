using Godot;

namespace Espejismo.Core.RichText;

/// <summary>
/// Provides mutable access to specific properties of a <see cref="RichText.Glyph"/> for applying text effects.
/// </summary>
public ref struct GlyphTransform
{
    private readonly ref readonly Glyph _glyph;

    /// <summary>
    /// Creates a new <see cref="GlyphTransform"/> instance.
    /// </summary>
    /// <param name="glyph">The glyph being transformed.</param>
    /// <param name="elapsedTime">Time elapsed since the glyph rendering started.</param>
    /// <param name="linePosition">The index of the glyph within to the source line.</param>
    /// <param name="lineLength">The number of glyphs in the source line.</param>
    public GlyphTransform(in Glyph glyph, double elapsedTime, int linePosition, int lineLength)
    {
        _glyph = ref glyph;

        Index = glyph.Index;
        Color = glyph.Style.Color;
        ShadowColor = glyph.Style.ShadowColor;
        OutlineColor = glyph.Style.OutlineColor;

        ElapsedTime = elapsedTime;
        LinePosition = linePosition;
        LineLength = lineLength;
    }

    /// <summary>
    /// Gets the glyph to which the transformation is applied.
    /// </summary>
    public readonly ref readonly Glyph Glyph => ref _glyph;

    /// <summary>
    /// Gets the number of seconds since the text started rendering.
    /// </summary>
    public double ElapsedTime { get; }

    /// <summary>
    /// Gets the index of the glyph within the source line.
    /// </summary>
    public int LinePosition { get; }

    /// <summary>
    /// Gets the number of glyphs in the source line.
    /// </summary>
    public int LineLength { get; }

    /// <summary>
    /// Gets or sets the index of the glyph in the font, used when drawing.
    /// </summary>
    /// <remarks>
    /// You can change this property by using <see cref="TextServer.FontGetGlyphIndex"/> along with
    /// <see cref="Glyph.FontRid"/> and <see cref="Glyph.FontSize"/> to generate a new, valid index.
    /// </remarks>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the color tint applied to the glyph.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Gets or sets the color for the shadow effect.
    /// </summary>
    /// <remarks>
    /// The shadow effect will not be rendered if <see cref="GlyphStyle.ShadowSize"/> is set to 0; you can change the
    /// shadow size by either changing the source <see cref="TextStyle"/> or by using a <see cref="Tags.ShadowTag"/>.
    /// </remarks>
    public Color ShadowColor { get; set; }

    /// <summary>
    /// Gets or sets the color for the glyph outline.
    /// </summary>
    /// <remarks>
    /// The outline will not be rendered if <see cref="GlyphStyle.OutlineSize"/> is set to 0; you can change the
    /// outline size by either changing the source <see cref="TextStyle"/> or by using an
    /// <see cref="Tags.OutlineTag"/>.
    /// </remarks>
    public Color OutlineColor { get; set; }

    /// <summary>
    /// Gets or sets the displacement applied to the glyph's draw position, in pixels.
    /// </summary>
    public Vector2 Offset { get; set; }

    /// <summary>
    /// Gets or sets the visibility state of the glyph.
    /// </summary>
    public GlyphVisibility Visibility { get; set; }
}
