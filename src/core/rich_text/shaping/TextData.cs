using Godot;

namespace Espejismo.Core.RichText.Shaping;

// Helper structure containing the actual shaping data to process, along with the base styling attributes to use.
internal readonly struct TextData
{
	public required ShapeItem[] Items { get; init; }

	public required TextStyle BaseStyle { get; init; }
}
