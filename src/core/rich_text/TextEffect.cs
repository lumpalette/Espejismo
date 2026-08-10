using Godot;

namespace Espejismo.Core.RichText;

/// <summary>
/// Provides the base class for implementing custom visual effects applied to <see cref="Glyph"/> instances.
/// </summary>
[GlobalClass]
public abstract partial class TextEffect : Resource
{
	/// <summary>
	/// Gets a value indicating whether the effect must be reprocessed every frame.
	/// </summary>
	public abstract bool IsAnimated { get; }

	/// <summary>
	/// Computes the visual transformation applied to a single <see cref="Glyph"/>.
	/// </summary>
	/// <param name="trans">
	/// The mutable transform state of the glyph to process.
	/// </param>
	public abstract void Process(ref GlyphTransform trans); // inguesu
}
