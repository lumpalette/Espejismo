using System;

namespace Spectrum.RichText;

/// <summary>
///   Represents shaped rich-text string produced by a <see cref="TextParser"/> operation.
/// </summary>
public sealed class Text
{
	/// <summary>
	///   Initializes a new instance of the <see cref="Text"/> class using the specified <see cref="TextStyle"/>
	///   and <see cref="ParseContext"/> data.
	/// </summary>
	/// <param name="style">
	///   The text style to use
	/// </param>
	/// <param name="context">
	///   The final state of the parser.
	/// </param>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="style"/> or <paramref name="context"/> is <see langword="null"/>.
	/// </exception>
	public Text(TextStyle style, ParseContext context)
	{
		ArgumentNullException.ThrowIfNull(style, nameof(style));
		ArgumentNullException.ThrowIfNull(context, nameof(context));


	}


}
