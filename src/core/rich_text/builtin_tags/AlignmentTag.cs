using Godot;
using System;

namespace Espejismo.Core.RichText.BuiltinTags;

/// <summary>
/// Represents a tag that changes the horizontal alignment of a text paragraph.
/// </summary>
/// <remarks>
/// Syntax: <c>&lt;alignment&gt;...&lt;/alignment&gt;</c>
/// </remarks>
[GlobalClass]
public sealed partial class AlignmentTag() : TextTag([])
{
	[Export]
	private HorizontalAlignment _alignment;

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		builder.PushAlignment(_alignment);
		return true;
	}

	/// <inheritdoc/>
	public override void End(TextBuilder builder)
	{
		builder.PopAlignment();
	}
}
