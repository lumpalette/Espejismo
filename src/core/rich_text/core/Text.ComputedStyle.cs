using Godot;

namespace Espejismo.Core.RichText;

public sealed partial class Text
{
    private static ComputedStyle ComputeStyle(TextStyle style, TextStyle baseStyle, TextStyle fallbackStyle)
    {
        var fontStyle = Pick(style.FontStyle, baseStyle.FontStyle, fallbackStyle.FontStyle);
        var fontSize = Pick(style.FontSize, baseStyle.FontSize, fallbackStyle.FontSize);

        // The TextServer does not provide a way to change the spacing of a specific text span/run, so we have to use
        // font variations for this.
        var font = (style.Font ?? baseStyle.Font ?? fallbackStyle.Font!).GetVariant(fontStyle);
        var spacing = Pick(style.Spacing, baseStyle.Spacing, fallbackStyle.Spacing);

        if (spacing.X != 0)
        {
            font = new FontVariation
            {
                BaseFont = font,
                SpacingGlyph = spacing.X
            };
        }

        var glyphStyle = new GlyphStyle(
            Pick(style.Color, baseStyle.Color, fallbackStyle.Color),
            Pick(style.ShadowSize, baseStyle.ShadowSize, fallbackStyle.ShadowSize),
            Pick(style.ShadowColor, baseStyle.ShadowColor, fallbackStyle.ShadowColor),
            Pick(style.ShadowOffset, baseStyle.ShadowOffset, fallbackStyle.ShadowOffset),
            Pick(style.OutlineSize, baseStyle.OutlineSize, fallbackStyle.OutlineSize),
            Pick(style.OutlineColor, baseStyle.OutlineColor, fallbackStyle.OutlineColor));

        return new ComputedStyle(font, fontSize, spacing.Y, glyphStyle);

        static T Pick<T>(in T? item, in T? baseItem, in T? fallbackItem) where T : struct
        {
            return item ?? baseItem ?? fallbackItem!.Value;
        }
    }

    // Resolved style of a group of glyphs associated to a specific shape item.
    private class ComputedStyle(Font font, ushort fontSize, int leading, GlyphStyle shared)
    {
        public Font Font { get; } = font;

        public ushort FontSize { get; } = fontSize;

        public int Leading { get; } = leading;

        public GlyphStyle Shared { get; } = shared;
    }
}
