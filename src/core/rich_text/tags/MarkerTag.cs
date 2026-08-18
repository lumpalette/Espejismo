using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
///   A self-closing text tag that inserts a <see cref="TextMarker"/> at the tag's position.
/// </summary>
/// <remarks>
/// <para>
///   <b>Type:</b> Void Element.
/// </para>
/// <para>
///   <b>Attributes:</b> Varies (depends on the marker's purpose).
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class MarkerTag : TextTag
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
