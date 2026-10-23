using Godot;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a set of style properties that describe how text is rendered.
/// </summary>
public sealed record class TextStyle
{
    /// <summary>
    /// Creates a new <see cref="TextStyle"/> instance with all its properties unset.
    /// </summary>
    /// <remarks>
    /// Consider using <see cref="Empty"/> instead of creating empty styles to avoid unnecessary allocations.
    /// </remarks>
    public TextStyle()
    {
    }

    /// <summary>
    /// Gets a style with all its properties unset.
    /// </summary>
    public static TextStyle Empty { get; } = new();

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
    /// Gets a value that indicates whether every property of the style is set.
    /// </summary>
    public bool IsComplete => this != Empty;
}
