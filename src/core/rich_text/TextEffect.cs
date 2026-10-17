using Godot;
using System;

namespace Espejismo.Core.RichText;

/// <summary>
/// Serves as the base class for implementing custom visual effects applied to individual glyphs.
/// </summary>
[GlobalClass, Tool]
public abstract partial class TextEffect : TextResource
{
    /// <summary>
    /// Computes the visual transformation of the effect and applies it to a glyph.
    /// </summary>
    /// <param name="trans">The mutable state of the glyph to transform.</param>
    /// <returns>
    /// <see langword="true"/> if the effect is successfully applied; otherwise, <see langword="false"/>.
    /// </returns>
    public abstract bool TryApply(ref GlyphTransform trans); // inguesumaiz

    /// <summary>
    /// Returns a <see cref="TextEffect"/> initialized according to the specified attributes.
    /// </summary>
    /// <param name="attributes">The attributes passed to the tag that applies this effect.</param>
    /// <returns>
    /// A <see cref="TextEffect"/> of the same type as this instance, reflecting <paramref name="attributes"/>.
    /// </returns>
    /// <remarks>
    /// The default implementation returns the current instance. Override it to return a new, configured instance
    /// instead of modifying the shared resource.
    /// </remarks>
    public virtual TextEffect Setup(ReadOnlySpan<TagAttribute> attributes)
    {
        return this;
    }
}
