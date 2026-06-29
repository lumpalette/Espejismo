using System;

namespace Spectrum.RichText.BuiltinTags;

/// <summary>
/// Represents a tag that inserts a text command at the current text position.
/// </summary>
/// <remarks>
/// <para>
/// Syntax:<br/>
/// <c>&lt;name [arg1=[val1]] [arg2=[val2]] [...]/></c>
/// </para>
/// <para>
/// where:<br/>
/// • <c>name</c> is the specified <paramref name="name"/> for the command.<br/>
/// • <c>argn</c> is the name of the nth required property, specified in the <paramref name="requiredProperties"/>
///   array.<br/>
/// • <c>valn</c> is the value of the nth of the nth required property, if applicable.
/// </para>
/// </remarks>
/// <param name="name">
/// The name of the command to be inserted.
/// </param>
/// <param name="requiredProperties">
/// The names of the properties required by the command.
/// </param>
public sealed class CommandTag(string name, string[] requiredProperties) : TagBehaviour(name, requiredProperties)
{
	public override bool Begin(ParseContext context, ReadOnlySpan<TagProperty> properties)
	{
		context.AppendCommand(Name, properties);
		return true;
	}
}
