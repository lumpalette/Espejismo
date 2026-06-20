using Godot;
using System;

namespace Spectrum.RichText.BuiltinTags;

/// <summary>
///		Represents a tag that changes the color of a specified segment of text.
/// </summary>
public sealed class ColorTag() : TextTag("color", ["value"])
{
	public override bool Begin(ParseContext context, ReadOnlySpan<TagProperty> properties)
	{
		var value = FindProperty(properties, "value");

		var color = Color.FromString(value.Value.ToString(), new Color(0f, 0f, 0f, -1f));

		if (color.A == -1f)
		{
			GD.PushWarning($"Unknown color value ({value.Value}).");
			return false;
		}

		context.PushStyle(context.TopStyle with { Color = color });
		return true;
	}

	public override void End(ParseContext context)
	{
		context.PopStyle();
	}
}
