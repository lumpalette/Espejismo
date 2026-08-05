using Godot;

namespace Espejismo.Core.RichText;

/// <summary>
/// Provides the base class for implementing custom visual effects applied to <see cref="Glyph"/> instances.
/// </summary>
[GlobalClass]
public abstract partial class TextEffect : Resource
{
	// TODO: write a method and probably a helper structure for updating the glyph effect.
}
