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
	private readonly List<Glyph> _glyphs = [];
	private readonly List<LineSpan> _lines = [];
	private readonly List<Paragraph> _paragraphs = [];
	private readonly List<TextMarker> _markers = [];

	private readonly ShapeItem[] _items;

	internal Text(ShapeItem[] items, TextStyle style)
	{
		_items = items;
		Style = style;
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
	/// Gets the total number of <see cref="LineSpan"/> instances in the text.
	/// </summary>
	/// <remarks>
	/// A text reshape is triggered when accessing this property and <see cref="IsDirty"/> is <see langword="true"/>.
	/// </remarks>
	public int LineCount
	{
		get
		{
			Shape();
			return _lines.Count;
		}
	}

	/// <summary>
	/// Gets the <see cref="TextMarker"/> instances embedded into the shaped text.
	/// </summary>
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
	/// <remarks>
	/// A text reshape is triggered when accessing this property and <see cref="IsDirty"/> is <see langword="true"/>.
	/// </remarks>
	public TextStyle Style
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

		// This looks way cursed that the thing I did with Synthesizer, but I don't care I'm done.
		new Shaper(
			TS: _TS,
			text: new TextData
			{
				Items = _items,
				BaseStyle = Style,
			},
			layout: new LayoutOptions
			{
				MaxWidth = Width,
				BaseAlignment = Alignment,
				Direction = TextServer.Direction.Auto,
				Orientation = TextServer.Orientation.Horizontal
			},
			output: new OutputBuffers
			{
				Glyphs = _glyphs,
				Lines = _lines,
				Paragraphs = _paragraphs,
				Markers = _markers
			}).Shape();
	}

	private void Invalidate()
	{
		IsDirty = true;
	}
}
