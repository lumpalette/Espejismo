using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
///   Represents a self-closing tag that inserts a <see cref="TextMarker"/> at the tag's position.
/// </summary>
/// <remarks>
/// <para>
///   <b>Type:</b> Void Element.
/// </para>
/// <para>
///   <b>Attributes:</b> Varies (depends on the marker's purpose).
/// </para>
/// <para>
///   <b>Example:</b> <c>"Embedded&lt;wait time=1s/&gt; marker."</c>
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class MarkerTag() : TextTag(requiredAttributes: [])
{
	[Export]
	private string _name = string.Empty;

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		builder.AppendMarker(_name ?? "<null>", attributes);
		return true;
	}
}
