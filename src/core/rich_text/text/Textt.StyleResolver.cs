using Godot;
using System.Collections.Generic;
using System.Diagnostics;

namespace Espejismo.Core.RichText;

public sealed partial class Text
{
    private static class StyleResolver
    {
        public static void Run(ShapeItem[] items, in TextStyle baseStyle, Dictionary<TextStyle, ItemStyle> styleMap)
        {
            styleMap.Clear();

            foreach (var item in items)
            {
                if (item is not (ShapeItem.Run or ShapeItem.Icon))
                {
                    continue;
                }

                var itemStyle = GetStyleFromItem(item);

                if (styleMap.ContainsKey(itemStyle))
                {
                    continue;
                }

                // MergedWith() does not merge neither the font styles nor the effects, so we have to do it ourselves.
                var mergedStyle = itemStyle.MergedWith(baseStyle) with
                {
                    FontStyle = (baseStyle.FontStyle ?? FontStyle.None) | (itemStyle.FontStyle ?? FontStyle.None),
                    Effect = baseStyle.Effect.CombinedWith(itemStyle.Effect)
                };

                styleMap[itemStyle] = ResolveStyle(mergedStyle);
            }
        }

        private static TextStyle GetStyleFromItem(ShapeItem item)
        {
            return item switch
            {
                ShapeItem.Run run => run.Style,
                ShapeItem.Icon ico => ico.Style,
                _ => throw new UnreachableException()
            };
        }

        private static ItemStyle ResolveStyle(in TextStyle style)
        {
            var font = style.Font!.GetVariant(style.FontStyle!.Value);
            var spacing = style.Spacing!.Value;

            // The TextServer does not provide a way to change the spacing of a specific segment of text, so we have to
            // use font variations for this.
            if (spacing.X != 0)
            {
                font = new FontVariation
                {
                    BaseFont = font,
                    SpacingGlyph = spacing.X,
                };
            }

            // We store the leading separately from the font because storing it in FontVariation.spacing_bottom merges
            // the line's descent with the leading, so we're left without any (cheap) way to differentiate between
            // these two values.
            return new ItemStyle(
                font: font,
                fontSize: style.FontSize!.Value,
                leading: spacing.Y,
                style: new GlyphStyle
                {
                    Color = style.Color!.Value,
                    Effect = style.Effect,
                    ShadowSize = style.ShadowSize!.Value,
                    ShadowColor = style.ShadowColor!.Value,
                    ShadowOffset = style.ShadowOffset!.Value,
                    OutlineSize = style.OutlineSize!.Value,
                    OutlineColor = style.OutlineColor!.Value
                }
            );
        }
    }

    // A fully resolved style that belongs to a specific shape item and its contents.
    private readonly struct ItemStyle(Font font, ushort fontSize, int leading, GlyphStyle style)
    {
        public Font Font { get; } = font;

        public ushort FontSize { get; } = fontSize;

        public int Leading { get; } = leading;

        public GlyphStyle Style { get; } = style;
    }
}
