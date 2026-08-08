using Godot;
using System;

namespace Espejismo.Core.RichText.BuiltinTags;

/// <summary>
/// Represents a self-closing tag that inserts an icon at the tag's position.
/// </summary>
/// <remarks>
/// <para>
/// Syntax: <c>&lt;icon name="name" [align="top|center|bottom"] [size="W×H"]/&gt;</c>
/// </para>
/// <para>
/// Where:<br/>
/// • <c>name</c> is the name of the <see cref="Texture2D"/> resource, as defined in <see cref="ResourceDB"/>.<br/>
/// • <c>WxH</c> are the dimensions of the texture rect, specified as <c>(width × height)</c>.
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class IconTag() : TextTag([RequiredName])
{
	private const string RequiredName = "name";
	private const string OptionalAlign = "align";
	private const string OptionalSize = "size";

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		var nameA = FindAttribute(attributes, RequiredName);
		var alignA = FindAttribute(attributes, OptionalAlign);
		var sizeA = FindAttribute(attributes, OptionalSize);

		if (!ResourceDB.TryGetResource<Texture2D>(nameA.Value, out var icon))
		{
			return false;
		}

		var alignment = InlineAlignment.Center;

		if (alignA.IsDefined && !TryParseAlignment(alignA.Value, out alignment))
		{
			return false;
		}

		var size = icon.GetSize();

		if (sizeA.IsDefined && !TryParseSize(sizeA.Value, out size))
		{
			return false;
		}

		builder.AppendIcon(icon, alignment, size);
		return true;
	}

	private static bool TryParseAlignment(ReadOnlySpan<char> s, out InlineAlignment alignment)
	{
		switch (s)
		{
			case "top":
				alignment = InlineAlignment.Top;
				return true;
			case "center":
				alignment = InlineAlignment.Center;
				return true;
			case "bottom":
				alignment = InlineAlignment.Bottom;
				return true;
		}

		alignment = default;
		return false;
	}

	private static bool TryParseSize(ReadOnlySpan<char> s, out Vector2 size)
	{
		var multiplyIndex = s.IndexOf('x');

		if (multiplyIndex == -1)
		{
			size = Vector2.Zero;
			return false;
		}

		var wSpan = s[..multiplyIndex].Trim();
		var hSpan = s[(multiplyIndex + 1)..].Trim();

		if (!int.TryParse(wSpan, out var w) || !int.TryParse(hSpan, out var h))
		{
			size = Vector2.Zero;
			return false;
		}

		size = new Vector2(w, h);
		return true;
	}
}
