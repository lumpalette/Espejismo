using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
/// A text tag that changes the <see cref="RichText.FontStyle"/> of a specific segment of text.
/// </summary>
/// <remarks>
/// <b>Attributes:</b> None.
/// </remarks>
[GlobalClass, Tool]
public sealed partial class FontStyleTag : TextTag
{
    /// <inheritdoc/>
    public override bool IsVoid => false;

    /// <summary>
    /// Gets the style variation applied by the tag, configured through the editor.
    /// </summary>
    [Export]
    public FontStyle FontStyle { get; private set; }

    /// <inheritdoc/>
    public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
    {
        var style = builder.TopStyle;

        _ = builder.PushStyle(style with { FontStyle = (style.FontStyle ?? FontStyle.None) | FontStyle });
        return true;
    }

    /// <inheritdoc/>
    public override void End(TextBuilder builder)
    {
        _ = builder.PopStyle();
    }
}
