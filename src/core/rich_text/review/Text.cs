using Espejismo.Core.RichText.Shaping;
using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Espejismo.Core.RichText.Preview;

/// <summary>
/// Represents a rich-text string that can be shaped into renderable glyphs.
/// </summary>
public sealed partial class Text : IDisposable
{
    private readonly TextServer _TS = TextServerManager.GetPrimaryInterface();
    private readonly Dictionary<TextStyle, ResolvedStyle> _styleMap = [];
    private readonly List<Glyph> _glyphs = [];
    private readonly List<LineLayout> _lines = [];
    private readonly List<TextMarker> _markers = [];

    private readonly ShapeItem[] _items;
    private readonly Rid _shaped;

    private bool _disposed;
    private Font? _fallbackFont;
    private ushort _fallbackFontSize;
    private int _fallbackLeading;

    internal Text(ShapeItem[] items, TextStyle style)
    {
        _items = items;
        _shaped = _TS.CreateShapedText();

        if (style == default)
        {
            GenerateStyleMap();
        }
        else
        {
            Style = style;
        }
    }

    /// <summary>
    /// Releases unmanaged <see cref="TextServer"/> resources used by the text.
    /// </summary>
    ~Text()
    {
        _TS.FreeRid(_shaped);
    }

    /// <summary>
    /// Gets a value indicating whether text's attributes have been changed and needs reshaping.
    /// </summary>
    public bool IsDirty { get; private set; } = true;

    /// <summary>
    /// Gets the shaped glyphs, in visual order (LTR).
    /// </summary>
    /// <remarks>
    /// A text reshape is triggered when accessing this property and <see cref="IsDirty"/> is <see langword="true"/>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown if the text has already been disposed.</exception>
    public ReadOnlySpan<Glyph> Glyphs
    {
        get
        {
            Shape();
            return CollectionsMarshal.AsSpan(_glyphs);
        }
    }

    /// <summary>
    /// Gets the total number of shaped glyphs.
    /// </summary>
    /// <remarks>
    /// A text reshape is triggered when accessing this property and <see cref="IsDirty"/> is <see langword="true"/>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown if the text has already been disposed.</exception>
    public int Length
    {
        get
        {
            Shape();
            return _glyphs.Count;
        }
    }

    /// <summary>
    /// Gets the layout of each shaped line, in visual order (top-to-bottom).
    /// </summary>
    /// <remarks>
    /// <para>
    /// You can use this property along with <see cref="Glyphs"/> to access the glyphs from a specific line of text.
    /// </para>
    /// <para>
    /// A text reshape is triggered when accessing this property and <see cref="IsDirty"/> is <see langword="true"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown if the text has already been disposed.</exception>
    public ReadOnlySpan<LineLayout> Lines
    {
        get
        {
            Shape();
            return CollectionsMarshal.AsSpan(_lines);
        }
    }

    /// <summary>
    /// Gets the markers embedded into the text.
    /// </summary>
    /// <remarks>
    /// A text reshape is triggered when accessing this property and <see cref="IsDirty"/> is <see langword="true"/>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown if the text has already been disposed.</exception>
    public ReadOnlySpan<TextMarker> Markers
    {
        get
        {
            Shape();
            return CollectionsMarshal.AsSpan(_markers);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the text has at least one assigned <see cref="TextEffect"/>.
    /// </summary>
    /// <remarks>
    /// Use this property to skip continuous redraws of static text. The value of this property is recalculated
    /// whenever <see cref="Style"/> changes.
    /// </remarks>
    public bool HasEffects { get; private set; }

    /// <summary>
    /// Gets or sets the base style applied to all the text.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the text has already been disposed.</exception>
    public TextStyle Style
    {
        get;
        set
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (field != value)
            {
                field = value;
                Invalidate();
                GenerateStyleMap();
            }
        }
    }

    /// <summary>
    /// Gets or sets the maximum width allowed for a text line, in pixels.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the text has already been disposed.</exception>
    public float Width
    {
        get;
        set
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (field != value)
            {
                field = value;
                Invalidate();
            }
        }
    }

    /// <summary>
    /// Gets the total size occupied by the shaped content, in pixels.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="Vector2.X"/> component is the width of the widest line, and the <see cref="Vector2.Y"/>
    /// component is the sum of the heights of all the lines. The resulting vector is recomputed per access.
    /// </para>
    /// <para>
    /// For lines using <see cref="HorizontalAlignment.Fill"/>, the line's width reflects the width it was fit to,
    /// rather than the natural width of its content.
    /// </para>
    /// <para>
    /// A text reshape is triggered when accessing this property and <see cref="IsDirty"/> is <see langword="true"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown if the text has already been disposed.</exception>
    public Vector2 ContentSize
    {
        get
        {
            Shape();

            var size = Vector2.Zero;
            var lines = Lines;

            if (lines.Length > 0)
            {
                foreach (ref readonly var line in lines)
                {
                    if (line.Width > size.X)
                    {
                        size.X = line.Width;
                    }

                    size.Y += line.Height;
                }

                size.Y -= lines[^1].Leading;
            }

            return size;
        }
    }

    /// <summary>
    /// Gets or sets the base horizontal alignment of the text.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the text has already been disposed.</exception>
    public HorizontalAlignment Alignment
    {
        get;
        set
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (field != value)
            {
                field = value;
                Invalidate();
            }
        }
    }

    /// <summary>
    /// Shapes the stored shape items into a sequence of glyphs.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the text has already been disposed.</exception>
    public void Shape()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!IsDirty)
        {
            return;
        }

        new Shaper
        {
            // Input.
            TS = _TS,
            Items = _items,
            StyleMap = _styleMap,

            // Layout options.
            MaxWidth = Width,
            Alignment = Alignment,

            // Output.
            Shaped = _shaped,
            Glyphs = _glyphs,
            Lines = _lines,
            Markers = _markers,

            // Fallback values.
            FallbackFont = _fallbackFont!,
            FallbackFontSize = _fallbackFontSize,
            FallbackLeading = _fallbackLeading
        }.Shape();

        IsDirty = false;
    }

    /// <summary>
    /// Releases unmanaged <see cref="TextServer"/> resources used by the text.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            Clear();
            _TS.FreeRid(_shaped);

            GC.SuppressFinalize(this);

            _disposed = true;
        }
    }

    private void Clear()
    {
        _glyphs.Clear();
        _lines.Clear();
        _markers.Clear();
    }

    private void Invalidate()
    {
        Clear();
        _TS.ShapedTextClear(_shaped);
        IsDirty = true;
    }

    private void GenerateStyleMap()
    {
        _styleMap.Clear();
        HasEffects = false;

        var baseStyle = TextConfig.DefaultStyle.Create(Style);

        _fallbackFont = baseStyle.Font!.GetVariant(baseStyle.FontStyle!.Value);
        _fallbackFontSize = baseStyle.FontSize!.Value;
        _fallbackLeading = baseStyle.Spacing!.Value.Y;

        foreach (var item in _items)
        {
            if (item.Type is not (ShapeItemType.Run or ShapeItemType.Icon))
            {
                continue;
            }

            var itemStyle = (item.Type == ShapeItemType.Run) ? item.Run!.Value.Style : item.Icon!.Value.Style;

            if (_styleMap.ContainsKey(itemStyle))
            {
                continue;
            }

            // We don't pass the line spacing to the font, as it is stored inside the LineSpan.Leading separately.
            var mergedStyle = itemStyle.MergedWith(baseStyle) with
            {
                FontStyle = (baseStyle.FontStyle ?? FontStyle.None) | (itemStyle.FontStyle ?? FontStyle.None),
                Effect = Effects.CompositeEffect.Combine(baseStyle.Effect, itemStyle.Effect)
            };

            var font = mergedStyle.Font!.GetVariant(mergedStyle.FontStyle!.Value);
            var spacing = mergedStyle.Spacing!.Value;

            if (spacing.X != 0)
            {
                font = new FontVariation
                {
                    BaseFont = font,
                    SpacingGlyph = spacing.X,
                };
            }

            _styleMap[itemStyle] = new ResolvedStyle
            {
                Font = font,
                FontSize = mergedStyle.FontSize!.Value,
                Leading = spacing.Y,
                Style = new GlyphStyle
                {
                    Color = mergedStyle.Color!.Value,
                    Effect = mergedStyle.Effect,
                    ShadowSize = mergedStyle.ShadowSize!.Value,
                    ShadowColor = mergedStyle.ShadowColor!.Value,
                    ShadowOffset = mergedStyle.ShadowOffset!.Value,
                    OutlineSize = mergedStyle.OutlineSize!.Value,
                    OutlineColor = mergedStyle.OutlineColor!.Value
                }
            };

            if (mergedStyle.Effect is not null)
            {
                HasEffects = true;
            }
        }
    }
}
