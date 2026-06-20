using System;

namespace Spectrum.RichText.BuiltinTags;

/// <summary>
///		Represents a tag that inserts a text command at the current text position.
/// </summary>
/// <param name="name">
///		The name of the command to be inserted.
///	</param>
/// <param name="requiredProperties">
///		The names of the properties required by the command.
///	</param>
public sealed class CommandTag(string name, string[] requiredProperties) : TextTag(name, requiredProperties)
{
	public override bool Begin(ParseContext context, ReadOnlySpan<TagProperty> properties)
	{
		context.AppendCommand(Name, properties);
		return true;
	}
}
