using Espejismo.Core.RichText;
using Godot;
using System;

namespace Espejismo.UI;

[GlobalClass, Tool]
public partial class TextRenderer : Control
{
	private Text? _data;

	[Export(PropertyHint.MultilineText)]
	public string Text
	{
		get;
		set
		{
			ArgumentNullException.ThrowIfNull(value, nameof(value));

			if (field != value)
			{
				field = value;
				RegenerateData();
			}
		}
	}

	[Export]
	public StyleTemplate? Style
	{
		get;
		set
		{
			if (field != value)
			{
				
			}
		}
	}

	[Export]
	public HorizontalAlignment HorizontalAlignment { get; set; }

	[Export]
	public VerticalAlignment VerticalAlignment { get; set; }

	private void RegenerateData()
	{
		
	}
}
