using Espejismo.Core.RichText.Parsing;
using Godot;
using System;
using System.Collections.Generic;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a rich-text string that can be shaped into renderable glyphs.
/// </summary>
public sealed partial class Text
{
    private readonly Dictionary<TextStyle, ComputedStyle> _styleMap = [];

    private readonly ShapeItem[] _items;

    internal Text(ShapeItem[] items)
    {
        _items = items;

        // Cannot be initialized in the class body the style map construction relies on _items.
        Style = TextStyle.Empty;
    }

    /// <summary>
    /// Gets a value indicating whether any of the text's properties have been changed and needs reshaping.
    /// </summary>
    public bool IsDirty { get; private set; }

    public TextStyle Style
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            foreach (var item in _items)
            {
                var itemStyle = item switch
                {
                    ShapeItem.Run run => run.Style,
                    ShapeItem.Icon icon => icon.Style,
                    _ => null
                };

                if (itemStyle is null)
                {
                    continue;
                }


            }
        }
    }

    /// <summary>
    /// Parses a rich-text string into a shaped <see cref="Text"/>.
    /// </summary>
    /// <param name="richText">The rich-text string to parse.</param>
    /// <param name="charLimit">The maximum number of characters to parse. If set to -1, no limit is applied.</param>
    /// <returns>The <see cref="Text"/> representation of <paramref name="richText"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="richText"/>  is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="charLimit"/> is less than -1.
    /// </exception>
    public static Text Parse(string richText, int charLimit = -1)
    {
        ArgumentNullException.ThrowIfNull(richText);

        if (richText.Length == 0 || charLimit == 0)
        {
            return new Text([]);
        }

        var document = new Document(richText);
        var builder = new TextBuilder(charLimit);

        // Looks cursed somehow, but whatever, it works.
        new Synthesizer(document, builder, TextConfig.Tags).Run();

        return builder.Build();
    }

    public void Shape()
    {

    }

    private static ComputedStyle ComputeStyle(TextStyle itemStyle, TextStyle baseStyle, TextStyle fallbackStyle)
    {
        var fontStyle = itemStyle.FontStyle ?? baseStyle.FontStyle ?? fallbackStyle.FontStyle!.Value;
        var font = (itemStyle.Font ?? baseStyle.Font ?? fallbackStyle.Font!).GetVariant(fontStyle);

        // The TextServer does not provide a way to change the spacing of a specific text span/run, so we have to use
        // font variations for this.
        var spacing = itemStyle.Spacing ?? baseStyle.Spacing ?? fallbackStyle.Spacing!.Value;

        if (spacing.X != 0)
        {
            font = new FontVariation
            {
                BaseFont = font,
                SpacingGlyph = spacing.X
            };
        }

        return new ComputedStyle
        {
            Font = font,
            FontSize = itemStyle.FontSize ?? baseStyle.FontSize ?? fallbackStyle.FontSize!.Value,
            Leading = spacing.Y,
            SharedProperties = new GlyphStyle(
                Pick(itemStyle.Color, baseStyle.Color, fallbackStyle.Color),
                Pick(itemStyle.ShadowSize, baseStyle.ShadowSize, fallbackStyle.ShadowSize),
                Pick(itemStyle.ShadowColor, baseStyle.ShadowColor, fallbackStyle.ShadowColor),
                Pick(itemStyle.ShadowOffset, baseStyle.ShadowOffset, fallbackStyle.ShadowOffset),
                Pick(itemStyle.OutlineSize, baseStyle.OutlineSize, fallbackStyle.OutlineSize),
                Pick(itemStyle.OutlineColor, baseStyle.OutlineColor, fallbackStyle.OutlineColor))
        };

        static T Pick<T>(in T? item, in T? baseItem, in T? fallbackItem) where T : struct
        {
            return item ?? baseItem ?? fallbackItem!.Value;
        }
    }

    // Resolved style properties of a group of glyphs, associated to a specific item style.
    private readonly struct ComputedStyle
    {
        public Font Font { get; init; }

        public ushort FontSize { get; init; }

        // We store the leading separately from the font because storing it in FontVariation.spacing_top/bottom merges
        // it with the line's ascent/descent, so we're left without a (cheap) way to differentiate between them.
        public int Leading { get; init; }

        public GlyphStyle SharedProperties { get; init; }
    }
}
