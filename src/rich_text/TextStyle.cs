using Godot;

namespace Spectrum.RichText;

/// <summary>
///   Represents a set of style attributes that describe how text is rendered.
/// </summary>
[GlobalClass, Tool]
public partial class TextStyle : Resource
{
	/// <summary>
	///   Gets or sets the font resource used for the text.
	/// </summary>
	[Export]
	public Font? Font
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				EmitChanged();
			}
		}
	}

	/// <summary>
	///   Gets or sets the size of the text, in pixels (px). The default is 8px.
	/// </summary>
	[Export(PropertyHint.Range, $"0, 256, suffix:px")]
	public int FontSize
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				EmitChanged();
			}
		}
	} = 8;

	/// <summary>
	///   Gets or sets the color of the text. The default is white.
	/// </summary>
	[Export]
	public Color Color
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				EmitChanged();
			}
		}
	} = Colors.White;

	/// <summary>
	///   Gets or sets additional space between characters, in pixels. Can be negative.
	/// </summary>
	[Export]
	public int LetterSpacing
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				EmitChanged();
			}
		}
	}

	/// <summary>
	///   Gets or sets additional space between lines of text, in pixels. Can be negative.
	/// </summary>
	[Export]
	public int LineSpacing
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				EmitChanged();
			}
		}
	}
}
