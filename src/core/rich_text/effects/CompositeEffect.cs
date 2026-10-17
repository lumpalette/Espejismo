using Godot;
using System;

namespace Espejismo.Core.RichText.Effects;

/// <summary>
/// A text effect that applies a sequence of <see cref="TextEffect"/> instances to a <see cref="Glyph"/>.
/// </summary>
[GlobalClass, Tool]
public sealed partial class CompositeEffect : TextEffect
{
    [Export]
    private TextEffect?[] _effects = [];

    /// <summary>
    /// Creates a new <see cref="CompositeEffect"/> instance with no <see cref="TextEffect"/> instances assigned.
    /// </summary>
    public CompositeEffect()
    {
    }

    /// <summary>
    /// Creates a new <see cref="CompositeEffect"/> instance with the specified <see cref="TextEffect"/> instances.
    /// </summary>
    /// <param name="effects">The effects to apply, in order. <see langword="null"/> entries are ignored.</param>
    public CompositeEffect(params ReadOnlySpan<TextEffect?> effects)
    {
        var count = 0;

        foreach (var effect in effects)
        {
            if (effect is not null)
            {
                count++;
            }
        }

        _effects = new TextEffect?[count];
        count = 0;

        foreach (var effect in effects)
        {
            if (effect is not null)
            {
                _effects[count++] = effect;
            }
        }
    }

    /// <summary>
    /// Gets the <see cref="TextEffect"/> instances that make up this effect.
    /// </summary>
    public ReadOnlySpan<TextEffect?> Effects => _effects;

    /// <summary>
    /// Combines two <see cref="TextEffect"/> instances as a single effect.
    /// </summary>
    /// <param name="a">The first effect to combine.</param>
    /// <param name="b">The second effect to combine.</param>
    /// <returns>
    /// A new combined <see cref="TextEffect"/>, flattening any nested composite. If either <paramref name="a"/> or
    /// <paramref name="b"/> are <see langword="null"/>, the other one is returned. If both are not specified, returns
    /// <see langword="null"/>.
    /// </returns>
    public static TextEffect? Combine(TextEffect? a, TextEffect? b)
    {
        // Return the same effect if the other one is null.
        if (a is null)
        {
            return b;
        }

        if (b is null)
        {
            return a;
        }

        // Both effects are not null, so we need to merge them.
        var compA = a as CompositeEffect;
        var compB = b as CompositeEffect;

        if (compA is not null && compB is not null)
        {
            return new CompositeEffect([.. compA.Effects, .. compB.Effects]);
        }

        if (compA is not null)
        {
            return new CompositeEffect([.. compA.Effects, b]);
        }

        if (compB is not null)
        {
            return new CompositeEffect([a, .. compB.Effects]);
        }

        return new CompositeEffect(a, b);
    }

    /// <summary>
    /// Computes and applies the visual transformations of every effect.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if all effects are applied successfully; otherwise, <see langword="false"/>.
    /// </returns>
    /// <inheritdoc/>
    public override bool TryApply(ref GlyphTransform trans)
    {
        var status = true;

        foreach (var effect in Effects)
        {
            // This can happen for editor-made tags.
            if (effect is not null)
            {
                status &= effect.TryApply(ref trans);
            }
        }

        return status;
    }

    /// <exception cref="InvalidOperationException">
    /// Thrown if one of the nested effects directly contains the same instance, creating a cyclic reference.
    /// </exception>
    /// <inheritdoc/>
    public override TextEffect Setup(ReadOnlySpan<TagAttribute> attributes)
    {
        // I optimized the parsing layer so I could do shit like this without consecuences.
        var effects = new TextEffect?[_effects.Length];
        var changed = false;

        for (var i = 0; i < effects.Length; i++)
        {
            effects[i] = _effects[i]?.Setup(attributes);

            // I don't know why the fuck would you do this, but this check avoids nesting the same composite effect
            // into itself, which would provoke an infinite loop.
            if (ReferenceEquals(effects[i], this))
            {
                throw new InvalidOperationException(
                    "Cyclical nesting is not supported for CompositeEffect instances.");
            }

            changed |= !ReferenceEquals(effects[i], _effects[i]);
        }

        return changed ? new CompositeEffect(effects) : this;
    }
}
