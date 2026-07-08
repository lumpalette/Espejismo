using Godot;
using System;

namespace Spectrum.RichText.Shaping;

internal readonly ref struct LayoutOptions
{
	public required float Width { get; init; }

	public HorizontalAlignment BaseAlignment { get; init; }

	public ReadOnlySpan<AlignmentRange> Alignments { get; init; }

	public TextServer.Direction Direction { get; init; }

	public TextServer.Orientation Orientation { get; init; }
}
