using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
///   Represents a tag that changes the horizontal alignment of a text paragraph.
/// </summary>
/// <remarks>
/// <para>
///   <b>Type:</b> Normal Element.
/// </para>
/// <para>
///   <b>Attributes:</b> None.
/// </para>
/// <para>
///   <b>Example:</b> <c>"&lt;center&gt;This paragraph is centered.&lt;/center&gt;"</c>
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class AlignmentTag() : TextTag(requiredAttributes: [])
{
	/// <summary>
	///   Gets the type of alignment applied by the tag, configured through the editor.
	/// </summary>
	[Export]
	public HorizontalAlignment Alignment { get; private set; }

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		builder.PushAlignment(Alignment);
		return true;
	}

	/// <inheritdoc/>
	public override void End(TextBuilder builder)
	{
		builder.PopAlignment();
	}
}
