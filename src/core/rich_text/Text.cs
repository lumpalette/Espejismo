using Espejismo.Core.RichText.Parsing;
using Espejismo.Core.RichText.Shaping;
using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a rich-text string that can be shaped into renderable glyphs.
/// </summary>
public partial class Text
{
	private readonly TextServer _TS = TextServerManager.GetPrimaryInterface();
	private readonly Dictionary<TextStyle, ResolvedStyle> _styleMap = [];
	private readonly List<Glyph> _glyphs = [];
	private readonly List<LineSpan> _lines = [];
	private readonly List<TextMarker> _markers = [];
	private readonly List<Paragraph> _paragraphs = [];

	private readonly ShapeItem[] _items;
	
	internal Text(ShapeItem[] items, TextStyle style)
	{
		_items = items;
		
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
	/// Gets a value indicating whether text's attributes have been changed and needs reshaping.
	/// </summary>
	public bool IsDirty { get; private set; } = true;

	/// <summary>
	/// Gets the shaped <see cref="Glyph"/> instances, in visual order (LTR).
	/// </summary>
	/// <remarks>
	/// A text reshape is triggered when accessing this property and <see cref="IsDirty"/> is <see langword="true"/>.
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
	/// Gets the total number of shaped <see cref="Glyph"/> instances.
	/// </summary>
	/// <remarks>
	/// A text reshape is triggered when accessing this property and <see cref="IsDirty"/> is <see langword="true"/>.
	/// </remarks>
	public int Length
	{
		get
		{
			Shape();
			return _glyphs.Count;
		}
	}

	/// <summary>
	/// Gets the shaped <see cref="LineSpan"/> instances, in visual order (top-to-bottom).
	/// </summary>
	/// <remarks>
	/// A text reshape is triggered when accessing this property and <see cref="IsDirty"/> is <see langword="true"/>.
	/// </remarks>
	public ReadOnlySpan<LineSpan> Lines
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
	/// A text reshape is triggered when accessing this property and <see cref="IsDirty"/> is <see langword="true"/>.
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
	/// Gets or sets the base style applied to all the text.
	/// </summary>
	public TextStyle Style
	{
		get;
		set
		{
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
	/// Gets or sets the horizontal alignment of the text.
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
	/// Parses a rich-text string into a <see cref="Text"/> with the specified <see cref="TextStyle"/> applied.
	/// </summary>
	/// <param name="richText">
	/// The rich-text formatted string to parse.
	/// </param>
	/// <param name="style">
	/// The style to apply to the resulting text.
	/// </param>
	/// <returns>
	/// The <see cref="Text"/> representation of <paramref name="richText"/>. 
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="richText"/> is <see langword="null"/>.
	/// </exception>
	public static Text Parse(string richText, TextStyle style)
	{
		ArgumentNullException.ThrowIfNull(richText, nameof(richText));

		var document = new Document(richText);
		var builder = new TextBuilder();

		// Looks cursed somehow, but whatever, it works.
		new Synthesizer(document, builder).Read();
		
		return builder.Build(style);
	}

	/// <summary>
	/// Shapes the stored shape items into a sequence of <see cref="Glyph"/> instances.
	/// </summary>
	public void Shape()
	{
		if (!IsDirty)
		{
			return;
		}

		IsDirty = false;

		// The shaper doesn't automatically clear the output.
		_glyphs.Clear();
		_lines.Clear();
		_markers.Clear();
		_paragraphs.Clear();
		
		// Now it looks nicer, cool I guess.
		var shaper = new Shaper
		{
			// Input.
			TS        = _TS,
			Items     = _items,
			StyleMap = _styleMap,
			
			// Layout options.
			MaxWidth      = Width,
			BaseAlignment = Alignment,
			Direction     = TextServer.Direction.Auto,
			Orientation   = TextServer.Orientation.Horizontal, // for now, only horizontal scripts are supported.

			// Output.
			Glyphs     = _glyphs,
			Lines      = _lines,
			Markers    = _markers,
			Paragraphs = _paragraphs,

			// Fallback values.
			DefaultFont     = ResourceDB.DefaultStyle.Font,
			DefaultFontSize = ResourceDB.DefaultStyle.FontSize
		};
		
		shaper.Shape();
	}

	private void Invalidate()
	{
		IsDirty = true;
	}

	private void GenerateStyleMap()
	{
		_styleMap.Clear();

		foreach (var item in _items)
		{
			if (item.Type is not (ShapeItemType.Run or ShapeItemType.Texture) || _styleMap.ContainsKey(item.Style))
			{
				continue;
			}

			// when eres un fokin nerd y el orden de los parámetros importa.
			var merged = item.Style.MergedWith(Style);
			var resolved = ResourceDB.DefaultStyle.CreateFrom(merged);

			var font = resolved.Font!;
			var spcX = resolved.LetterSpacing!.Value;
			var spcY = resolved.LineSpacing!.Value;

			if (spcX != 0 || spcY != 0)
			{
				font = new FontVariation
				{
					BaseFont = font,
					SpacingGlyph = spcX,
					SpacingBottom = spcY
				};
			}

			_styleMap[item.Style] = new ResolvedStyle
			{
				Font = font,
				FontSize = resolved.FontSize!.Value,
				Style = new GlyphStyle
				{
					Color = resolved.Color!.Value,
					Effect = resolved.Effect,
					ShadowSize = resolved.ShadowSize!.Value,
					ShadowColor = resolved.ShadowColor!.Value,
					ShadowOffset = resolved.ShadowOffset!.Value,
					OutlineSize = resolved.OutlineSize!.Value,
					OutlineColor = resolved.OutlineColor!.Value
				}
			};
		}
	}
}
