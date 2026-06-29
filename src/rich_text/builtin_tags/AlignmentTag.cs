using Godot;
using System;

namespace Spectrum.RichText.BuiltinTags;

/// <summary>
/// Represents a tag that changes the horizontal alignment of a text paragraph.
/// </summary>
/// <remarks>
/// Syntax:<br/>
/// <c>&lt;align type="left|center|right|fill">...&lt;/align></c>
/// </remarks>
public class AlignmentTag() : TagBehaviour("align", ["type"])
{
	public override bool Begin(ParseContext context, ReadOnlySpan<TagProperty> properties)
	{
		var type = FindProperty(properties, "type");
		
		HorizontalAlignment alignment;

		switch (type.Value)
		{
			case "left":
				alignment = HorizontalAlignment.Left;
				break;
			case "center":
				alignment = HorizontalAlignment.Center;
				break;
			case "right":
				alignment = HorizontalAlignment.Right;
				break;
			case "fill":
				alignment = HorizontalAlignment.Fill;
				break;
			default:
				GD.PushWarning($"Unknown horizontal alignment flag ({type.Value})");
				return false;
		}

		context.BeginAlignment(alignment);
		return true;
	}

	public override void End(ParseContext context)
	{
		context.EndAlignment();
	}
}
