using System.Collections.Generic;

namespace Spectrum.RichText.Shaping;

internal readonly struct BufferData
{
	public List<TextLine> Lines { get; init; }

	public List<Glyph> Glyphs { get; init; }

	public List<ShapedBlock> Blocks { get; init; }
}
