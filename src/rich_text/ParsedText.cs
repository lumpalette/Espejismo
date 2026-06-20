using System;

namespace Spectrum.RichText;

/// <summary>
///		Represents the result of a rich-text parsing operation, containing the parsed text runs, icons and commands.
/// </summary>
public class ParsedText
{
	private readonly TextRun[] _runs;
	private readonly TextCommand[] _commands;

	/// <summary>
	///		Initializes a new instance of the <see cref="ParsedText"/> class from the specified parse state.
	/// </summary>
	/// <param name="context">
	///		The finished state of a parser.
	/// </param>
	/// <exception cref="ArgumentNullException">
	///		Thrown if <paramref name="context"/> is <see langword="null"/>.
	/// </exception>
	public ParsedText(ParseContext context)
	{
		ArgumentNullException.ThrowIfNull(context, nameof(context));

		_runs = context.Runs.ToArray();
		_commands = context.Commands.ToArray();
	}

	/// <summary>
	///		Gets all the parsed segments of printable text, with their respective style properties.
	/// </summary>
	public ReadOnlySpan<TextRun> Runs => _runs;

	/// <summary>
	///		Gets all the parsed commands embedded into the text at specific positions.
	/// </summary>
	public ReadOnlySpan<TextCommand> Commands => _commands;
}
