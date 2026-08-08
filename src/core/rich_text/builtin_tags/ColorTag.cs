using Godot;
using System;
using System.Security.Cryptography;

namespace Espejismo.Core.RichText.BuiltinTags;

/// <summary>
/// Represents a tag that changes the color of a specified segment of text.
/// </summary>
/// <remarks>
/// <para>
/// Syntax: <c>&lt;color value="name|html_code"&gt;...&lt;/color&gt;</c>
/// </para>
/// <para>
/// where:<br/>
/// • <c>name</c> is the name of one of the colors in the <see cref="Colors"/> class, case-insensitive.<br/>
/// • <c>html_code</c> is a 3, 4, 6 or 8-digit HTML color code, optionally prefixed by a '#' character.
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class ColorTag() : TextTag([RequiredValue])
{
	private const string RequiredValue = "value";
	
	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		var valueA = FindAttribute(attributes, RequiredValue);

		if (!TryParseColor(valueA.Value.ToString(), out Color color))
		{
			return false;
		}

		builder.PushStyle(builder.TopStyle with { Color = color });
		return true;
	}

	/// <inheritdoc/>
	public override void End(TextBuilder builder)
	{
		builder.PopStyle();
	}

	private static bool TryParseColor(string s, out Color color)
	{
		var fallback = new Color(-1f, -2f, -3f, -4f);
		color = Color.FromString(s, fallback);
		return color != fallback;
	}
}
