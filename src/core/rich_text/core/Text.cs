using Espejismo.Core.RichText.Parsing;
using System;
using System.Collections.Generic;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a rich-text string that can be shaped into renderable glyphs.
/// </summary>
public sealed partial class Text
{
    private readonly ShapeItem[] _items;
    private readonly ComputedStyle[] _styles;

    private bool _styleDirty;

    internal Text(ShapeItem[] items)
    {
        _items = items;
        _styles = new ComputedStyle[items.Length];
    }

    /// <summary>
    /// Gets a value indicating whether any of the text's properties have been changed and needs reshaping.
    /// </summary>
    public bool IsDirty { get; private set; }

    /// <summary>
    /// Gets or sets the current base style that is applied to all text.
    /// </summary>
    public TextStyle Style
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                InvalidateStyle();
                InvalidateShape();
            }
        }
    } = TextStyle.Empty;

    /// <summary>
    /// Gets or sets the current base visual effect that is applied to all text.
    /// </summary>
    public TextEffect? Effect { get; set; }

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
        if (!IsDirty)
        {
            return;
        }

        Restyle();


    }

    private void InvalidateShape()
    {
        IsDirty = true;
    }

    private void InvalidateStyle()
    {
        _styleDirty = true;
    }

    private void Restyle()
    {
        if (!_styleDirty)
        {
            return;
        }

        var uniqueStyles = new Dictionary<TextStyle, ComputedStyle>();

        for (var i = 0; i < _items.Length; i++)
        {
            var style = _items[i] switch
            {
                ShapeItem.Run run => run.Style,
                ShapeItem.Icon icon => icon.Style,
                _ => null
            };

            // An item without a style; ignore it.
            if (style is null)
            {
                continue;
            }

            if (!uniqueStyles.TryGetValue(style, out var computed))
            {
                uniqueStyles[style] = computed = ComputeStyle(style, Style, TextConfig.FallbackStyle.Value);
            }

            _styles[i] = computed;
        }

        _styleDirty = false;
    }
}
