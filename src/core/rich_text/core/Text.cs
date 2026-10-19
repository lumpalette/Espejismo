using Espejismo.Core.RichText.Parsing;
using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a rich-text string that can be shaped into renderable glyphs.
/// </summary>
public sealed partial class Text
{
    private readonly TextServer _server = TextServerManager.GetPrimaryInterface();
    private readonly Dictionary<TextStyle, ComputedStyle> _styleMap = [];
    private readonly List<Glyph> _glyphs = [];
    private readonly List<LineLayout> _lines = [];
    private readonly List<TextMarker> _markers = [];

    private readonly ShapeItem[] _shapeItems;

    internal Text(ShapeItem[] items, TextStyle style)
    {
        _shapeItems = items;
        Style = style;
    }

    /// <summary>
    /// Gets the shaped <see cref="Glyph"/> instances, in visual order (LTR).
    /// </summary>
    /// <remarks>
    /// A reshape is triggered whenever the property is accessed and the text is dirty.
    /// </remarks>
    public ReadOnlySpan<Glyph> Glyphs
    {
        get
        {
            Shape();
            return CollectionsMarshal.AsSpan(_glyphs);
        }
    }

    /// <summary>
    /// Gets the <see cref="LineLayout"/> of each shaped line, in visual order.
    /// </summary>
    /// <remarks>
    /// A reshape is triggered whenever the property is accessed and the text is dirty.
    /// </remarks>
    public ReadOnlySpan<LineLayout> Lines
    {
        get
        {
            Shape();
            return CollectionsMarshal.AsSpan(_lines);
        }
    }

    /// <summary>
    /// Gets the <see cref="TextMarker"/> instances embedded into the shaped text.
    /// </summary>
    /// <remarks>
    /// A reshape is triggered whenever the property is accessed and the text is dirty.
    /// </remarks>
    public ReadOnlySpan<TextMarker> Markers
    {
        get
        {
            Shape();
            return CollectionsMarshal.AsSpan(_markers);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the text properties has been changed and needs reshaping.
    /// </summary>
    public bool IsDirty { get; private set; }

    /// <summary>
    /// Gets or sets the base style currently applied to the text.
    /// </summary>
    /// <remarks>
    /// This is used to resolve styles for text runs and/or icons that have incomplete styles applied. Unset properties
    /// in this style cascade back to the default value specified in <see cref="TextConfig.DefaultStyle"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if it is set to a <see langword="null"/> value.</exception>
    public TextStyle Style
    {
        get;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (field == value)
            {
                return;
            }

            field = value;

            Invalidate();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the text has at least one <see cref="TextEffect"/> applied.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is used to determine whether the text is non-static, and it should be redrawn every frame.
    /// </para>
    /// <para>
    /// The value of this property changes whenever <see cref="Style"/> is changed.
    /// </para>
    /// </remarks>
    public bool IsAnimated { get; private set; }

    /// <summary>
    /// Gets or sets the maximum width allowed for a line of text, in pixels.
    /// </summary>
    public float Width
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                Invalidate();
            }
        }
    }

    /// <summary>
    /// Gets or sets the base horizontal alignment of the text.
    /// </summary>
    public HorizontalAlignment Alignment
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                Invalidate();
            }
        }
    }

    /// <summary>
    /// Parses a string into a <see cref="Text"/> with the specified <see cref="TextStyle"/> applied.
    /// </summary>
    /// <param name="richText">The rich-text formatted string to parse.</param>
    /// <param name="style">The style to apply to the resulting text.</param>
    /// <param name="charLimit">The maximum number of characters to parse. If set to -1, no limit is applied.</param>
    /// <returns>The <see cref="Text"/> representation of <paramref name="richText"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="richText"/> or <paramref name="style"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="charLimit"/> is less than -1.
    /// </exception>
    public static Text Parse(string richText, TextStyle style, int charLimit = -1)
    {
        ArgumentNullException.ThrowIfNull(richText);
        ArgumentNullException.ThrowIfNull(style);

        if (richText.Length == 0 || charLimit == 0)
        {
            return new Text([], style);
        }

        var document = new Document(richText);
        var builder = new TextBuilder(charLimit);

        // Looks cursed somehow, but whatever, it works.
        new Synthesizer(document, builder, TextConfig.Tags).Run();

        return builder.Build(style);
    }

    /// <summary>
    /// Shapes the text into a sequence of renderable glyphs and lines.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is not necessary to manually call this method; the text is shaped automatically when accessing its data.
    /// </para>
    /// <para>
    /// The method does nothing if the text is not dirty.
    /// </para>
    /// </remarks>
    public void Shape()
    {
        if (!IsDirty)
        {
            return;
        }
        /*
        new Shaper
        {
            // Text data.
            ShapeItems = _shapeItems,
            Server = _server,
            StyleMap = _styleMap,

            // Layout options.
            MaxWidth = Width,
            BaseAlignment = Alignment,

            // Fallback values.
            FallbackFont = ?,
            FallbackFontSize = ?,
            FallbackLeading = ?,

            // Output, the lists are cleared by the shaper.
            Glyphs = _glyphs,
            Lines = _lines,
            Markers = _markers
        }.Run();
        */
        IsDirty = false;
    }

    private void Invalidate()
    {
        IsDirty = true;
    }
}
