using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
/// A text tag that changes the <see cref="TextEffect"/> of a specific segment of text.
/// </summary>
/// <remarks>
/// <b>Attributes:</b> Varies (depends on the specific effect).
/// </remarks>
[GlobalClass, Tool]
public sealed partial class EffectTag : TextTag
{
    /// <inheritdoc/>
    public override bool IsVoid => false;

    /// <summary>
    /// Gets the effect applied by the tag, configured through the editor.
    /// </summary>
    [Export]
    public TextEffect? Effect { get; private set; }

    /// <inheritdoc/>
    public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
    {
        if (Effect is not null)
        {
            _ = builder.PushEffect(Effect.Setup(attributes));
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public override void End(TextBuilder builder)
    {
        _ = builder.PopStyle();
    }
}
