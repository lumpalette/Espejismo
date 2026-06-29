using Godot;
using System;

namespace Spectrum.RichText;

/// <summary>
/// Represents shaped rich-text string produced by a <see cref="TextParser"/> operation.
/// </summary>
public class Text
{
	private readonly TextRun[] _runs;
	private readonly InlineIcon[] _icons;
	private readonly InlineCommand[] _commands;
	private readonly AlignmentBlock[] _alignmentBlocks;

	private bool _dirty = true;
	private Rid _shapedBuffer;
	private Glyph[] _glyphs = [];

	/// <summary>
	/// Initializes a new instance of the <see cref="Text"/> class from the specified parse state.
	/// </summary>
	/// <param name="context">
	/// The finished state of a parser.
	/// </param>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="style"/> or <paramref name="context"/> is <see langword="null"/>.
	/// </exception>
	public Text(TextStyle style, ParseContext context)
	{
		ArgumentNullException.ThrowIfNull(style, nameof(style));
		ArgumentNullException.ThrowIfNull(context, nameof(context));
		
		_runs = context.Runs.ToArray();
		_icons = context.Icons.ToArray();
		_commands = context.Commands.ToArray();
		_alignmentBlocks = context.AlignmentOverrides.ToArray();

		Style = style;
		Style.Changed += OnStyleChanged;
	}

	/// <summary>
	/// Gets all the parsed commands embedded into the text at specific positions.
	/// </summary>
	public ReadOnlySpan<InlineCommand> Commands => _commands;

	/// <summary>
	/// Gets or sets the base style for every <see cref="Glyph"/>.
	/// </summary>
	public TextStyle Style
	{
		get;
		set
		{
			ArgumentNullException.ThrowIfNull(value, nameof(value));

			if (ReferenceEquals(field, value))
			{
				return;
			}

			field.Changed -= OnStyleChanged;
			field = value;
			field.Changed += OnStyleChanged;

			_dirty = true;
		}
	}

	/// <summary>
	/// Gets the default horizontal alignment for the text.
	/// </summary>
	public HorizontalAlignment HorizontalAlignment
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				_dirty = true;
			}
		}
	}

	/// <summary>
	/// Gets the vertical alignment for the text.
	/// </summary>
	public VerticalAlignment VerticalAlignment
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				_dirty = true;
			}
		}
	}

	public void Shape()
	{
		
	}

	private void OnStyleChanged()
	{
		_dirty = true;
	}
}
