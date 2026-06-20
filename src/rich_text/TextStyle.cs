using Godot;

namespace Spectrum.RichText;

/// <summary>
///		Represents a set of visual properties that describe how text is displayed.
/// </summary>
public partial class TextStyle : Resource
{
	/// <summary>
	///		Gets the font resource used for the text.
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
	///		Gets the size of the text, in pixels (px). The default is 8px.
	/// </summary>
	[Export(PropertyHint.Range, $"0, 256, suffix:px")]
	public uint FontSize
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
	///		Gets the color of the text. The default is white.
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
}
