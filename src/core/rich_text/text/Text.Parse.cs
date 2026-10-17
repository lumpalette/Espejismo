using Espejismo.Core.RichText.Parsing;
using System;

namespace Espejismo.Core.RichText;

public sealed partial class Text
{
    /// <summary>
    /// Parses a string into a <see cref="Text"/> with the specified <see cref="TextStyle"/> applied.
    /// </summary>
    /// <param name="richText">The rich-text formatted string to parse.</param>
    /// <param name="style">The style to apply to the resulting text.</param>
    /// <param name="charLimit">The maximum number of characters to parse. If set to -1, no limit is applied.</param>
    /// <returns>The <see cref="Text"/> representation of <paramref name="richText"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="richText"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="charLimit"/> is less than -1.
    /// </exception>
    public static Text Parse(string richText, TextStyle style, int charLimit = -1)
    {
        ArgumentNullException.ThrowIfNull(richText);

        if (richText.Length == 0 || charLimit == 0)
        {
            return new Text([], style);
        }

        var document = new Document(richText);
        var builder = new TextBuilder(charLimit);

        // Looks cursed somehow, but whatever, it works.
        new Synthesizer(document, builder).Read();

        return builder.Build(style);
    }
}
