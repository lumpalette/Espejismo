using System.Collections.Generic;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a rich-text string that can be shaped into renderable glyphs.
/// </summary>
public sealed partial class Text
{
    private readonly Dictionary<TextStyle, ItemStyle> _styleMap = [];

    private readonly ShapeItem[] _shapeItems;

    internal Text(ShapeItem[] items, in TextStyle style)
    {
        _shapeItems = items;

        if (style == default)
        {
            StyleResolver.Run(items, default, _styleMap);
        }
        else
        {
            Style = style;
        }
    }

    public TextStyle Style { get; set; }
}
