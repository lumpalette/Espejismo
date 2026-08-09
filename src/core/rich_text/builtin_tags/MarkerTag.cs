using Godot;
using System;

namespace Espejismo.Core.RichText.BuiltinTags;

/// <summary>
/// Represents a self-closing tag that inserts a <see cref="TextMarker"/> at the tag's position.
/// </summary>
/// <remarks>
/// <para>
/// Syntax: <c>&lt;name [attr1=val1] [attr2=val2] .../&gt;</c>
/// </para>
/// <para>
/// Where:
/// <list type="bullet">
///   <item>
///     <term><c>name</c></term>
///     <description>The name of the marker, as specified in the editor.</description>
///   </item>
///   <item>
///     <term><c>attrn</c></term>
///     <description>The nth attribute name.</description>
///   </item>
///   <item>
///     <term><c>valn</c></term>
///     <description>The nth attribute value. Omitted if the associated attribute is boolean.</description>
///   </item>
/// </list>
/// </para>
/// </remarks>
public sealed partial class MarkerTag() : TextTag([])
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
