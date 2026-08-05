using System.Collections.Generic;

namespace Espejismo.Core.RichText.Shaping;

// Helper structure that contains the buffers to store the end result of the shaping process.
internal readonly struct OutputBuffers
{
	public required List<Glyph> Glyphs { get; init; }

	public required List<LineSpan> Lines { get; init; }

	public required List<TextMarker> Markers { get; init; }

	public required List<Paragraph> Paragraphs { get; init; }

}
