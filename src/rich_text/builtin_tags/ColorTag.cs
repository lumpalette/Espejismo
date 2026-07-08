using Godot;
using System;

namespace Spectrum.RichText.BuiltinTags;

/// <summary>
///   Represents a tag that changes the color of a specified segment of text.
/// </summary>
/// <remarks>
///   <para>
///     Syntax:<br/>
///     <c>&lt;color value="name|html_code">...&lt;/color></c>
///   </para>
///   <para>
///     where:<br/>
///     • <c>name</c> is the name of one of the colors in the <see cref="Colors"/> class, case-insensitive.<br/>
///     • <c>html_code</c> is a 3, 4, 6 or 8-digit HTML color code, optionally prefixed by a '#' character.
///   </para>
/// </remarks>
public sealed class ColorTag() : TagBehaviour("color", ["value"])
{
	public override bool Begin(ParseContext context, ReadOnlySpan<TagProperty> properties)
	{
		var value = FindProperty(properties, "value");

		if (!TryParseColor(value.Value.ToString(), out Color color))
		{
			return false;
		}

		context.PushStyle(context.TopStyle with { Color = color });
		return true;
	}

	public override void End(ParseContext context)
	{
		context.PopStyle();
	}

	private static bool TryParseColor(string value, out Color color)
	{
		var fallback = new Color(-1f, -2f, -3f, -4f);
		color = Color.FromString(value, fallback);

		if (color == fallback)
		{
			if (!Engine.IsEditorHint())
			{
				GD.PushWarning($"Unknown color value ({value}).");
			}

			return false;
		}

		return true;
	}
}
