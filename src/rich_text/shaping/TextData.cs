using System;

namespace Spectrum.RichText.Shaping;

internal readonly ref struct TextData
{
	public required ReadOnlySpan<ParsedItem> ParsedSequence { get; init; }

	public required TextStyle BaseStyle { get; init; }
}
