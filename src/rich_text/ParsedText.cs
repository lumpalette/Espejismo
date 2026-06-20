using Godot;
using System;

namespace Spectrum.RichText;

/// <summary>
///		Represents the result of a parsing operation.
/// </summary>
public class ParsedText
{
	private readonly TextRun[] _runs;
	private readonly TextCommand[] _commands;

	public ParsedText(ParseContext context)
	{
		ArgumentNullException.ThrowIfNull(context, nameof(context));

		_runs = context.Runs.ToArray();
		_commands = context.Commands.ToArray();
	}
}
