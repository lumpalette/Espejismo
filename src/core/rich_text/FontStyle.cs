using System;

namespace Espejismo.Core.RichText;

/// <summary>
/// Specifies the style variants of a text font.
/// </summary>
[Flags]
public enum FontStyle
{
    /// <summary>
    /// No style applied.
    /// </summary>
    None = 0,

    /// <summary>
    /// Bold text.
    /// </summary>
    Bold = 1 << 0,

    /// <summary>
    /// Italic text.
    /// </summary>
    Italic = 1 << 1
}
