using Godot;
using Spectrum.RichText.Shaping;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Spectrum.RichText;

/// <summary>
///   Represents a shaped rich-text string produced by a <see cref="TextParser"/> operation.
/// </summary>
public partial class Text
{
	private readonly TextServer _TS = TextServerManager.GetPrimaryInterface();
	private readonly List<TextLine> _lines = [];
	private readonly List<Glyph> _glyphBuffer = [];
	private readonly List<ShapedBlock> _blockBuffer = [];

	private readonly TextRun[] _runs;
	private readonly InlineIcon[] _icons;
	private readonly InlineCommand[] _commands;
	private readonly AlignmentRange[] _alignments;
	private readonly ParsedItem[] _parsedSequence;

	/// <summary>
	///   Initializes a new instance of the <see cref="Text"/> class using the specified <see cref="TextStyle"/>
	///   and <see cref="ParseContext"/> data.
	/// </summary>
	/// <param name="baseStyle">
	///   The base text style to use
	/// </param>
	/// <param name="context">
	///   The final state of the parser.
	/// </param>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="baseStyle"/> or <paramref name="context"/> is <see langword="null"/>.
	/// </exception>
	public Text(TextStyle baseStyle, ParseContext context)
	{
		ArgumentNullException.ThrowIfNull(baseStyle, nameof(baseStyle));
		ArgumentNullException.ThrowIfNull(context, nameof(context));

		Style = baseStyle;

		_runs = context.Runs.ToArray();
		_icons = context.Icons.ToArray();
		_commands = context.Commands.ToArray();
		_alignments = context.AlignmentRanges.ToArray();

		// The parsed sequence is initialized once since the text data is immutable.
		_parsedSequence = new ParsedItem[_runs.Length + _icons.Length];
		GenerateParsedSequence();
	}

	/// <summary>
	///   Gets the sequence of shaped <see cref="Glyph"/> from all lines, in visual order.
	/// </summary>
	/// <remarks>
	///   The text is automatically reshaped if the property is read and the text is dirty.
	/// </remarks>
	public ReadOnlySpan<Glyph> Glyphs
	{
		get
		{
			if (IsDirty)
			{
				Shape();
			}

			return CollectionsMarshal.AsSpan(_glyphBuffer);
		}
	}

	/// <summary>
	///   Gets the number of glyphs in the text.
	/// </summary>
	/// <remarks>
	///   The text is automatically reshaped if the property is read and the text is dirty.
	/// </remarks>
	public int Length
	{
		get
		{
			if (IsDirty)
			{
				Shape();
			}

			return _glyphBuffer.Count;
		}
	}

	/// <summary>
	///   Gets a value indicating whether the text attributes has been modified and needs reshaping.
	/// </summary>
	public bool IsDirty { get; private set; }

	/// <summary>
	///   Gets the source rich-text string without tags or character entities.
	/// </summary>
	public string ParsedText
	{
		get
		{
			if (field is null)
			{
				var sb = new StringBuilder();

				foreach (var item in _parsedSequence)
				{
					if (item.IsRun)
					{
						sb.Append(item.Run.Text);
					}
					else
					{
						sb.Append('￼');
					}
				}

				field = sb.ToString();
			}

			return field;
		}
	}

	/// <summary>
	///   Gets the commands embedded into the parsed text, ordered according to their text position.
	/// </summary>
	public ReadOnlySpan<InlineCommand> Commands => _commands;

	/// <summary>
	///   Gets or sets the base style applied to all glyphs without an override.
	/// </summary>
	/// <exception cref="ArgumentNullException">
	///   Thrown if it is set to <see langword="null"/>.
	/// </exception>
	public TextStyle Style
	{
		get;
		set
		{
			ArgumentNullException.ThrowIfNull(value, nameof(value));

			if (field != value)
			{
				field?.Changed -= OnStyleChanged;
				field = value;
				field.Changed += OnStyleChanged;

				IsDirty = true;
			}
		}
	}

	/// <summary>
	///   Gets or sets the dimensions of the text container, in pixels.
	/// </summary>
	public Vector2 Size
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				IsDirty = true;
			}
		}
	}

	/// <summary>
	///   Gets or sets the horizontal alignment of the text, relative to <see cref="Size"/>.
	/// </summary>
	public HorizontalAlignment HorizontalAlignment
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				
				// Justified alignment requires a full reshape of the text.
				if (field == HorizontalAlignment.Fill)
				{
					IsDirty = true;
				}
			}
		}
	}

	/// <summary>
	///   Gets or sets the vertical alignment of the text, relative to <see cref="Size"/>.
	/// </summary>
	public VerticalAlignment VerticalAlignment { get; set; }

	/// <summary>
	///   Gets or sets the text flow direction, which can be LTR or RTL.
	/// </summary>
	public TextServer.Direction Direction
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				IsDirty = true;
			}
		}
	}

	/// <summary>
	///   Gets or sets whether the text is written horizontally or vertically.
	/// </summary>
	public TextServer.Orientation Orientation
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				IsDirty = true;
			}
		}
	}

	/// <summary>
	///   Transforms the text into a sequence of <see cref="Glyph"/> instances ready for rendering.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///   Thrown if the font of <see cref="Style"/> is <see langword="null"/>.
	/// </exception>
	public void Shape()
	{
		if (!NeedsReshape())
		{
			return;
		}

		new TextShaper
		{
			Server = _TS,
			Text = new TextData
			{
				ParsedSequence = _parsedSequence,
				BaseStyle = Style
			},
			Buffer = new BufferData
			{
				Lines = _lines,
				Glyphs = _glyphBuffer,
				Blocks = _blockBuffer
			},
			Layout = new LayoutOptions
			{
				Width = Orientation == TextServer.Orientation.Vertical ? Size.Y : Size.X,
				BaseAlignment = HorizontalAlignment,
				Alignments = _alignments,
				Direction = Direction,
				Orientation = Orientation
			}
		}.Shape();

		IsDirty = false;
	}

	public void Draw(Rid canvasItem)
	{
		if (Length == 0 || !canvasItem.IsValid)
		{
			return;
		}

		var isVertical = Orientation == TextServer.Orientation.Vertical;

		// 1. Calculate vertical alignment offset or line gaps (if any).
		var contentH = 0f;

		foreach (var line in _lines)
		{
			contentH += line.Ascent + line.Descent;
		}

		var containerW = isVertical ? Size.Y : Size.X;
		var containerH = isVertical ? Size.X : Size.Y;

		var startOffset = 0f;
		var lineGap = 0f;

		if (containerH > 0f)
		{
			switch (VerticalAlignment)
			{
				case VerticalAlignment.Center:
					startOffset = (containerH - contentH) / 2f;
					break;
				case VerticalAlignment.Bottom:
					startOffset = containerH - contentH;
					break;
				case VerticalAlignment.Fill:
					if (_lines.Count > 1 && contentH < containerH)
					{
						lineGap = (containerH - contentH) / (_lines.Count - 1);
						GD.Print(lineGap);
					}
					break;
			}
		}

		// 2. Draw every line with their respective alignment.
		var pen = Vector2.Zero;

		if (isVertical)
		{
			pen.X = startOffset;
		}
		else
		{
			pen.Y = startOffset;
		}

		foreach (var line in _lines)
		{
			// Start drawing from the baseline.
			if (isVertical)
			{
				pen.X += line.Ascent;
			}
			else
			{
				pen.Y += line.Ascent;
			}

			// 3. Apply horizontal alignment to pen position.
			var alignmentOffset = 0f;

			if (containerW > 0f && line.Alignment != HorizontalAlignment.Fill)
			{
				if (line.Alignment == HorizontalAlignment.Center)
				{
					alignmentOffset = (containerW - line.Width) / 2f;
				}

				if (line.Alignment == HorizontalAlignment.Right)
				{
					alignmentOffset = containerW - line.Width;
				}
			}

			if (isVertical)
			{
				pen.Y = alignmentOffset;
			}
			else
			{
				pen.X = alignmentOffset;
			}

			// 4. Draw the glyphs within the line range.
			foreach (ref readonly var g in Glyphs.Slice(line.Start, line.Length))
			{
				if (g.Font.IsValid)
				{
					_TS.FontDrawGlyph(g.Font, canvasItem, g.FontSize, pen + g.Offset, g.Index, g.Color);
				}
				else
				{
					_TS.DrawHexCodeBox(canvasItem, g.FontSize, pen, g.Index, g.Color);
				}

				if (isVertical)
				{
					pen.Y += g.Advance;
				}
				else
				{
					pen.X += g.Advance;
				}
			}

			// 5. Advance the pen position.
			if (isVertical)
			{
				pen.X += line.Descent + lineGap;
			}
			else
			{
				pen.Y += line.Descent + lineGap;
			}
		}
	}

	// Combines the text runs and icons into a single, sorted sequence.
	private void GenerateParsedSequence()
	{
		var runIndex = 0;
		var iconIndex = 0;
		var sequenceIndex = 0;

		while (sequenceIndex < _parsedSequence.Length)
		{
			var r = (runIndex < _runs.Length) ? _runs[runIndex].SequenceIndex : int.MaxValue;
			var i = (iconIndex < _icons.Length) ? _icons[iconIndex].SequenceIndex : int.MaxValue;

			_parsedSequence[sequenceIndex++] = (r < i)
				? new ParsedItem(_runs[runIndex++])
				: new ParsedItem(_icons[iconIndex++]);
		}
	}

	private bool NeedsReshape()
	{
		if (!IsDirty)
		{
			return false;
		}

		if (Style.Font is null)
		{
			throw new InvalidOperationException("Font from base style is null");
		}
		
		return true;
	}

	private void OnStyleChanged()
	{
		IsDirty = true;
	}
}
