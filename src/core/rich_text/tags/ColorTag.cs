using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
/// A text tag that changes the color of a specific segment of text.
/// </summary>
/// <remarks>
/// <b>Attributes:</b>
/// <list type="bullet">
///     <item>
///         <term><c>&lt;main&gt;</c></term>
///         <description>
///             The color to apply, in the same color formats that <see cref="Color.FromString(string, Color)"/>
///             accepts.
///         </description>
///     </item>
/// </list>
/// </remarks>
[GlobalClass, Tool]
public sealed partial class ColorTag : TextTag
{
    /// <inheritdoc/>
    public override bool IsVoid => false;

    /// <inheritdoc/>
    public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
    {
        if (!attributes.TryGetValue("<main>", out Color main))
        {
            return false;
        }

        _ = builder.PushStyle(builder.TopStyle with { Color = main });
        return true;
    }

    /// <inheritdoc/>
    public override void End(TextBuilder builder)
    {
        _ = builder.PopStyle();
    }
}
