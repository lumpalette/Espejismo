using Godot;
using System;

namespace Espejismo.Core.RichText;

/// <summary>
/// Provides the base class for implementing custom visual effects applied to <see cref="Glyph"/> instances.
/// </summary>
[GlobalClass]
public abstract partial class TextEffect : Resource
{
	/// <summary>
	/// Computes the visual transformation applied to a single <see cref="Glyph"/>.
	/// </summary>
	/// <param name="trans">
	/// The mutable state of the glyph to process.
	/// </param>
	public abstract void Process(ref GlyphTransform trans); // inguesumaiz

	/// <summary>
	/// Returns a <see cref="TextEffect"/> initialized according to the specified attributes.
	/// </summary>
	/// <param name="attributes">
	/// The attributes passed to the tag that applies this effect.
	/// </param>
	/// <returns>
	/// A <see cref="TextEffect"/> of the same type as this instance, reflecting <paramref name="attributes"/>.
	/// </returns>
	public virtual TextEffect Setup(ReadOnlySpan<TagAttribute> attributes)
	{
		return this;
	}
}
